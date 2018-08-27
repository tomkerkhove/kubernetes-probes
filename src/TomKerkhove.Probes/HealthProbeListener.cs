using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TomKerkhove.Probes
{
    public class HealthProbeListener
    {
        public const string ProbeAddress = "0.0.0.0";

        private const string DefaultResponseMessage = "Pong";
        public readonly int ProbePort;

        public HealthProbeListener(int probePort)
        {
            ProbePort = probePort;
        }

        public async Task StartListeningAsync()
        {
            try
            {
                var address = IPAddress.Parse(ProbeAddress);
                var endpoint = new IPEndPoint(address, ProbePort);
                var tcpListener = new TcpListener(endpoint);
                tcpListener.Start();

                LogMessage($"Start listening for health probes on {endpoint}");

                await Task.Factory.StartNew(async () => await HandleConnectionsAsync(tcpListener));
            }
            catch (Exception ex)
            {
                LogMessage($"Health probe listener failed due to an exception and is closing. Details: {ex.Message}");
            }
        }

        private async Task HandleConnectionsAsync(TcpListener tcpListener)
        {
            while (true)
            {
                await HandleTcpRequestAsync(tcpListener);
            }
        }

        private Task HandleNewClientConnectionAsync(TcpClient tcpClient)
        {
            WriteResponse(tcpClient, DefaultResponseMessage);
            LogMessage($"Wrote '{DefaultResponseMessage}' back to client");

            LogMessage(message: "Closing connection.");
            tcpClient.GetStream().Dispose();

            return Task.CompletedTask;
        }

        private static void WriteResponse(TcpClient tcpClient, string responseMessage)
        {
            var clientStream = tcpClient.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(responseMessage);
            clientStream.Write(data, offset: 0, size: data.Length);
        }

        private async Task<string> ReadAndLogRequest(string message, NetworkStream clientStream, byte[] data)
        {
            int numberOfBytesRead;
            while ((numberOfBytesRead = clientStream.Read(data, offset: 0, size: data.Length)) > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await memoryStream.WriteAsync(data, offset: 0, count: numberOfBytesRead);

                    message = Encoding.ASCII.GetString(memoryStream.ToArray(), index: 0, count: (int)memoryStream.Length);
                }

                LogMessage(message);
            }

            return message;
        }

        private async Task HandleTcpRequestAsync(TcpListener tcpListener)
        {
            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            LogMessage($"New client connected - {tcpClient.Client.LocalEndPoint}");
            await HandleNewClientConnectionAsync(tcpClient);
        }

        private void LogMessage(string message)
        {
            Console.WriteLine($"Health Probe Listener ({DateTimeOffset.Now:G}) > {message}");
        }
    }
}