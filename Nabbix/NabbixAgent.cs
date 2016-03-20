using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace Nabbix
{
    public interface INabbixAgent
    {
        void Start();
        void Stop();
        void RegisterInstance(object instance);
    }

    public class NabbixAgent : INabbixAgent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NabbixAgent));

        private readonly ItemRegistry _registry;
        private readonly IPAddress _address;
        private readonly int _port;

        private TcpListener _listener;
        private CancellationTokenSource _source;

        public NabbixAgent(int port, params object[] instances)
            : this(null, port, true, instances)
        {
        }

        public NabbixAgent(string address, int port, params object[] instances)
            : this(address, port, true, instances)
        {
        }

        public NabbixAgent(string address, int port, bool startImmediately = true, params object[] instances)
            : this(new ItemRegistry(), address, port, startImmediately)
        {
            if (instances == null)
            {
                Log.WarnFormat("instances is null");
                return;
            }

            foreach (object instance in instances)
            {
                RegisterInstance(instance);
            }
        }

        public NabbixAgent(ItemRegistry registry, string address, int port, bool startImmediately = true)
        {
            _registry = registry;
            _address = address == null ?
                IPAddress.Any :
                IPAddress.Parse(address); ;
            _port = port;

            if (startImmediately)
            {
                Start();
            }
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            _listener = new TcpListener(_address, _port);
            _listener.Start();
            var thread = new Thread(() =>
            {
                while (true)
                {
                    if (_source.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var client = _listener.AcceptTcpClient();
                        QueryHandler.Run(client, _registry);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Error running NabbixAgent.", e);
                    }
                }
            });

            thread.IsBackground = false;
            thread.Start();
        }
        
        public void Stop()
        {
            _source.Cancel();
            _listener.Stop();
        }

        public void RegisterInstance(object instance)
        {
            if (instance == null)
            {
                Log.WarnFormat("instance is null");
                return;
            }

            _registry.RegisterInstance(instance);
        }
    }
}
