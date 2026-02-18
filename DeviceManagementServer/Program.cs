using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementServer
{
    class Program
    {
        static async Task Main()
        {
            int port = 5000;

            var deviceManager = new DeviceManager();
            var processor = new CommandProcessor(deviceManager);
            var monitor = new HeartbeatMonitor(deviceManager);

            var webSocketManager = new WebSocketManager(deviceManager);
            deviceManager.SetWebSocketManager(webSocketManager);

            monitor.Start();
            _ = webSocketManager.StartAsync();

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"TCP Server started on port {port}");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client, processor);
            }
        }


        static async Task HandleClientAsync(TcpClient client, CommandProcessor processor)
        {
            using(client)
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead;
                    try
                    {
                        bytesRead = await stream.ReadAsync(buffer);
                    }
                    catch
                    {
                        break;
                    }

                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    string[] commands = message.Split('\n');

                    foreach (var cmd in commands)
                    {
                        await processor.ProcessCommandAsync(cmd.Trim());  
                    }
                }
            }
        }
    }
}