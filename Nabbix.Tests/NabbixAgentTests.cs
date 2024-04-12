using Nabbix.Items;
using System;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Nabbix.Tests
{
    public class NabbixAgentTests : IDisposable
    {
        private readonly NabbixAgent _agent;

        private const string OneCharacterKey = "h";
        private const string TwoCharacterKey = "he";
        private const string ThreeCharacterKey = "hel";
        private const string FourCharacterKey = "ruok";
        private const string FiveCharacterKey = "hello";
        private const string FakeZabbixHeader = "ZBXD1";
        private const string MediumSizedKey = "hello world";

        private const string LongKey =
            "In cyberspace where lines of code do dwell, /A timeless phrase in bytes and bits unfurled, /It whispers softly, breaking coder's spell, /The universal hymn: \"Hello, World!\"/From languages diverse, its voice resounds, /In Python's embrace or C's stern gaze, /In Java's tortuous syntax, or HTML's bounds, /In every tongue, its message finds its ways. /In but a single line, its tale is told, /A greeting, a beginning, and a sign, /In zeroes and ones, its truth behold, /A spark ignites, a code divine. //So let it ring in every realm unfurled, /The programmer's anthem: \"Hello, World!\"";

        public NabbixAgentTests()
        {
            _agent = new NabbixAgent("127.0.0.1", 0, new MyCounter());
            _agent.Start();
        }

        public void Dispose()
        {
            _agent.Stop();
        }

        private class MyCounter
        {
            [NabbixItem(OneCharacterKey)]
            // ReSharper disable UnusedMember.Local
            public long OneCharacterReturn => 1234;

            [NabbixItem(TwoCharacterKey)]
            public long ReturnConstant => 1234;

            [NabbixItem(ThreeCharacterKey)]
            public long ThreeCharacterReturn => 1234;

            [NabbixItem(FourCharacterKey)]
            public long FourCharacterReturn => 1234;

            [NabbixItem(FiveCharacterKey)]
            public long FiveCharacterReturn => 1234;

            [NabbixItem(FakeZabbixHeader)]
            public long FakeZabbixHeaderReturn => 1234;

            [NabbixItem(MediumSizedKey)]
            public long MediumSizedKeyReturn => 1234;

            [NabbixItem(LongKey)]
            public long LongSizedKeyPattern => 1234;
            // ReSharper enable UnusedMember.Local

        }

        private static byte[] CreateOldProtocolRequest(string key)
        {
            var sb = new StringBuilder();
            sb.Append(key);
            sb.Append((char)10);

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        private static byte[] CreateNewProtocolRequest(string key)
        {
            byte[] header = Encoding.ASCII.GetBytes("ZBXD\x01");
            byte[] dataLen = BitConverter.GetBytes((long)key.Length);
            byte[] content = Encoding.ASCII.GetBytes(key);
            byte[] message = new byte[header.Length + dataLen.Length + content.Length];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(dataLen, 0, message, header.Length, dataLen.Length);
            Buffer.BlockCopy(content, 0, message, header.Length + dataLen.Length, content.Length);

            return message;
        }

        private static string ReadResponse(NetworkStream stream)
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

        [Theory]
        [InlineData(true, MediumSizedKey)]
        [InlineData(true, OneCharacterKey)]
        [InlineData(true, TwoCharacterKey)]
        [InlineData(true, ThreeCharacterKey)]
        [InlineData(true, FourCharacterKey)]
        [InlineData(true, FiveCharacterKey)]
        [InlineData(true, FakeZabbixHeader)]
        [InlineData(true, LongKey)]
        [InlineData(false, MediumSizedKey)]
        [InlineData(false, OneCharacterKey)]
        [InlineData(false, TwoCharacterKey)]
        [InlineData(false, ThreeCharacterKey)]
        [InlineData(false, FourCharacterKey)]
        [InlineData(false, FiveCharacterKey)]
        [InlineData(false, FakeZabbixHeader)]
        [InlineData(false, LongKey)]
        public void Integration_OneConnection_SendReceiveResults(bool useOldProtocol, string key)
        {
            var port = _agent.LocalEndpointPort;

            for (int i = 0; i < 3; i++)
            {
                var client = new TcpClient("127.0.0.1", port);
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] request = useOldProtocol ? CreateOldProtocolRequest(key) : CreateNewProtocolRequest(key);
                    stream.Write(request, 0, request.Length);
                    stream.Flush();

                    string results = ReadResponse(stream);
                    Assert.EndsWith("1234", results);
                }
            }
        }
    }
}
