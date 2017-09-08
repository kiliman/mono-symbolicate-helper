using System;
using System.Configuration;
using Nancy;
using Nancy.Hosting.Self;

namespace MonoSymbolicateHelper.Service
{
    public class NancySelfHost
    {
        private NancyHost _nancyHost;

        public void Start()
        {
            // configure service
            var port = ConfigurationManager.AppSettings["port"] ?? "5000";
            var url = $"http://localhost:{port}";
            var config = new HostConfiguration {UrlReservations = {CreateAutomatically = true}};
            _nancyHost = new NancyHost(new Uri(url), new DefaultNancyBootstrapper(), config);
            _nancyHost.Start();
            Console.WriteLine($"Listening on {url}");
        }

        public void Stop()
        {
            _nancyHost.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
