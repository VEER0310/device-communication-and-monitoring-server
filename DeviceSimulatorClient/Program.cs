using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulatorClient
{
    class Program
    {
        static bool heartbeatRunning = false;

        static async Task Main()
        {
            Console.Write("Enter DeviceId: ");
            string deviceId = Console.ReadLine();

            using TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            using NetworkStream stream = client.GetStream();

            Console.WriteLine("Connected to server.");

            Task heartbeatTask = null;

            while (true)
            {
                Console.WriteLine("\nChoose option:");
                Console.WriteLine("1. REGISTER");
                Console.WriteLine("2. STATUS RUNNING");
                Console.WriteLine("3. STATUS STOPPED");
                Console.WriteLine("4. START HEARTBEAT");
                Console.WriteLine("5. STOP HEARTBEAT (simulate offline)");
                Console.WriteLine("6. FLOOD COMMANDS");
                Console.WriteLine("7. DISCONNECT");
                Console.Write("Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Send(stream, $"REGISTER|{deviceId}");
                        break;

                    case "2":
                        Send(stream, $"STATUS|{deviceId}|RUNNING");
                        break;

                    case "3":
                        Send(stream, $"STATUS|{deviceId}|STOPPED");
                        break;

                    case "4":
                        if (!heartbeatRunning)
                        {
                            heartbeatRunning = true;
                            heartbeatTask = Task.Run(() => HeartbeatLoop(stream, deviceId));
                            Console.WriteLine("Heartbeat started.");
                        }
                        break;

                    case "5":
                        heartbeatRunning = false;
                        Console.WriteLine("Heartbeat stopped (wait 15s for OFFLINE).");
                        break;

                    case "6":
                        await Flood(stream, deviceId);
                        break;

                    case "7":
                        heartbeatRunning = false;
                        Send(stream, $"DISCONNECT|{deviceId}");
                        return;
                }
            }
        }

        static void Send(NetworkStream stream, string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Sent: {msg}");
        }

        static async Task Flood(NetworkStream stream, string deviceId)
        {
            Console.WriteLine("Sending 20 commands quickly...");
            for (int i = 1; i <= 20; i++)
            {
                string cmd = $"STATUS|{deviceId}|RUNNING-{i}";
                Send(stream, cmd);
            }
            await Task.CompletedTask;
        }

        static void HeartbeatLoop(NetworkStream stream, string deviceId)
        {
            while (heartbeatRunning)
            {
                Send(stream, $"HEARTBEAT|{deviceId}");
                Thread.Sleep(5000);
            }
        }
    }
}
