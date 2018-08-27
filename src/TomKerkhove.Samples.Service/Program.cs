using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using TomKerkhove.Probes;

namespace TomKerkhove.Samples.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var rawProbePort = Environment.GetEnvironmentVariable("Probe_Tcp_Port");
            var probePort = int.Parse(rawProbePort);
            var healthProbeListener = new HealthProbeListener(probePort);
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConsoleLifetime()
                .Build();

            await host.StartAsync();
            await healthProbeListener.StartListeningAsync();
            await ProcessMessages();
        }

        private static async Task ProcessMessages()
        {
            var counter = 1;
            while (true)
            {
                await Task.Delay(1000 * 5);
                Console.WriteLine($"{DateTimeOffset.Now:G} > Processing task #{counter}");
                counter++;
            }
        }
    }
}
