using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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
        private Task _task;
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
            var cancellationToken = _source.Token;

            _listener = new TcpListener(_address, _port);
            _listener.Start();
            
            _task = Task.Factory.StartNew(() =>
            {
                Log.Debug("Starting task.");
                while (true)
                {
                    Log.Debug("Starting while loop.");
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Log.Debug("Token cancelled.");
                        break;
                    }

                    try
                    {
                        QueryHandler.Run(_listener, _registry);
                    }
                    catch (SocketException e)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Log.Debug(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Error running NabbixAgent.", e);
                    }
                }
            }, cancellationToken);
        }
        
        public void Stop()
        {
            Log.Info("Stopping TCP Listener.");
            _listener.Stop();

            Log.Info("Notifying handler task of cancellation.");
            _source.Cancel();

            Log.Info("Waiting for task to finish.");
            _task.Wait();

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
