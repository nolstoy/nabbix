using System;
using System.Net.Sockets;
using System.Text;
using Common.Logging;

namespace Nabbix
{
    internal class QueryHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(QueryHandler));

        private static byte[] GetResponseLength(string response)
        {
            var responseLength = new byte[8];
            long length = response.Length;
            for (int i = 0; i < 8; i++)
            {
                responseLength[i] = (byte)(int)(length & 0xFF);
                length >>= 8;
            }
            return responseLength;
        }

        private static string GetRequest(NetworkStream stream)
        {
            StringBuilder builder = new StringBuilder(32);
            int oneByte;
            do
            {
                oneByte = stream.ReadByte();

                if (oneByte != 10)
                {
                    builder.Append((char) oneByte);
                }

            } while (oneByte != -1 && oneByte != 10);

            return builder.ToString();
        }

        private static readonly byte[] Header = {(byte)'Z', (byte)'B', (byte)'X', (byte)'D', 1};
        private static void SendResponse(NetworkStream stream, string response)
        {
            byte[] responseLength = GetResponseLength(response);
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);

            byte[] buffer = new byte[Header.Length + responseLength.Length + responseBytes.Length];

            // https://www.zabbix.com/documentation/1.8/protocols
            Array.Copy(Header, 0, buffer, 0, Header.Length);
            Array.Copy(responseLength, 0, buffer, Header.Length, responseLength.Length);
            Array.Copy(responseBytes, 0, buffer, Header.Length + responseLength.Length, responseBytes.Length);

            Log.Debug("Response sending...");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            Log.Debug("Response sent.");
        }

        internal static void Run(TcpListener listener, ItemRegistry registry)
        {
            Log.Debug("Run... .");
            var client = listener.AcceptTcpClient();

            Log.Debug("TcpClient Accepted.");
            var stream = client.GetStream();
            do
            {
                Log.Debug("Request recieving...");
                string request = GetRequest(stream);
                Log.DebugFormat("Request received: {0}", request);

                string response = registry.GetItemValue(request);
                Log.DebugFormat("Response: {0}", response);
                SendResponse(stream, response);
            } while (stream.DataAvailable);

            Log.Debug("Run. Ended.");
        }
    }
}
