using Nabbix.Items;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Nabbix.Tests
{
    public class NabbixAgentTests : IDisposable
    {
        NabbixAgent _agent;

        public NabbixAgentTests()
        {
            _agent = new NabbixAgent("127.0.0.1", 0, new MyCounter());
            _agent.Start();
        }

        public void Dispose()
        {
            _agent.Stop();
        }

        public class MyCounter
        {
            [NabbixItem("always_return_constant")]
            public long ReturnConstant => 1234;
        }

        private string CreateRequest()
        {
            var sb = new StringBuilder();
            sb.Append("always_return_constant");
            sb.Append((char)10);

            return sb.ToString();
        }

        private string ReadResponse(NetworkStream stream)
        {
            byte[] data = new byte[1024];
            int totalRead = 0;

            int read;
            while ((read = stream.Read(data, totalRead, data.Length - totalRead)) > 0)
            {
                totalRead += read;

                if (!stream.DataAvailable)
                    break;
            }

            return Encoding.ASCII.GetString(data, 0, totalRead);
        }

        [Fact]
        public void Integration_OneConnection_SendReceiveResults()
        {
            var port = _agent.LocalEndpointPort;
            
            for (int i = 0; i < 3; i++)
            {
                var client = new TcpClient("127.0.0.1", port);
                using (NetworkStream stream = client.GetStream())
                {
                    var request = CreateRequest();
                    var bytes = Encoding.ASCII.GetBytes(request);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();

                    string results = ReadResponse(stream);
                    Assert.EndsWith("1234", results);
                }
            }
        }
       
    }
}
