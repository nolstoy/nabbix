using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabbix
{
    public interface INabbixAgent
    {
        void Stop();
    }

    public class NabbixAgent : INabbixAgent
    {
        public static INabbixAgent Start(string address, int port)
        {
            NabbixAgent agent = new NabbixAgent();

            IPAddress ipAddress = IPAddress.Parse(address);

            agent.StartListener(ipAddress, port);

            return agent;
        }

        TcpListener listener;
        CancellationTokenSource source;

        public void Stop()
        {
            source.Cancel();
            listener.Stop();
            // TODO: Wait until all tasks have stopped.
        }

        // Cancelation token.

        // Create TCP Listener
        // Create new threads
        private void StartListener(IPAddress address, int port)
        {
            source = new CancellationTokenSource();
            listener = new TcpListener(port);
            listener.Start();
            var thread = new Thread(() =>
            {

                while (true)
                {
                    if (source.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    var client = listener.AcceptTcpClient();
                    QueryHandler.Run(client);

                }
            });

            thread.IsBackground = false;
            thread.Start();
        }
    }
}
