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

        public int LocalEndpointPort
        {
            get
            {
                return ((IPEndPoint)_listener.LocalEndpoint).Port;
            }
        }

        public NabbixAgent(ItemRegistry registry, string address, int port, bool startImmediately = true)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));

            _registry = registry;
            _address = address == null
                ? IPAddress.Any
                : IPAddress.Parse(address);
            _port = port;

            if (startImmediately)
            {
                Start();
            }
        }

        public NabbixAgent(string address, int port, bool startImmediately = true, params object[] instances)
            : this(new ItemRegistry(), address, port, startImmediately)
        {
            if (instances == null)
            {
                return;
            }

            foreach (object instance in instances)
            {
                RegisterInstance(instance);
            }
        }

        public NabbixAgent(int port, params object[] instances)
            : this(null, port, true, instances)
        {
        }

        public NabbixAgent(string address, int port, params object[] instances)
            : this(address, port, true, instances)
        {
        }

        public void Start()
        {
            Log.Info("Starting NabbixAgent.");
            _source = new CancellationTokenSource();

            _listener = new TcpListener(_address, _port);
            _listener.Start();

            _listener.BeginAcceptTcpClient(ProcessRequest, _listener);
        }

        private void ProcessRequest(IAsyncResult ar)
        {
            if (_source.Token.IsCancellationRequested)
            {
                return;
            }

            if (!(ar.AsyncState is TcpListener listener))
            {
                Log.Debug("TcpLister is Null. This is impossible");
                return;
            }

            if (_source.Token.IsCancellationRequested)
            {
                Log.Debug("Token is already cancelled");
                return;
            }

            listener.BeginAcceptTcpClient(ProcessRequest, listener);

            try
            {
                using (TcpClient client = listener.EndAcceptTcpClient(ar))
                {
                    QueryHandler.Run(client, _registry);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error in QueryHandler Run", e);
            }
        }

        public void Stop()
        {
            if (_source.Token.IsCancellationRequested)
            {
                Log.Debug("Cancellation has already been requested.");
                return;
            }

            Log.Info("Stopping TCP connections.");
            _source.Cancel();

            Thread.Sleep(100);
            Log.Info("Stopping TCP Listener.");
            _listener.Stop();

            Log.Info("Stopped successfully.");
        }

        public void RegisterInstance(object instance)
        {
            if (instance == null)
            {
                Log.WarnFormat("instance is null");
                return;
            }

            Log.InfoFormat("Registering instance {0}.", instance.GetType());

            _registry.RegisterInstance(instance);
        }
    }
}
