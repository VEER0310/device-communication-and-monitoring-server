using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceManagementServer
{
    public class WebSocketManager
    {
        private readonly HttpListener _listener;
        private readonly ConcurrentBag<WebSocket> _clients = new();
        private readonly DeviceManager _deviceManager;

        internal WebSocketManager(DeviceManager manager)
        {
            _deviceManager = manager;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8080/ws/");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("WebSocket Server started at ws://localhost:8080/ws");

            while (true)
            {
                var context = await _listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var socket = wsContext.WebSocket;
                    _clients.Add(socket);
                    Console.WriteLine("Web client connected.");

                    await SendDeviceList(socket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        public async Task BroadcastDevicesAsync()
        {
            var devices = _deviceManager.GetAllDevices().Values;

            var json = JsonSerializer.Serialize(devices);
            var bytes = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<byte>(bytes);

            foreach (var socket in _clients)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }

        private async Task SendDeviceList(WebSocket socket)
        {
            var devices = _deviceManager.GetAllDevices().Values;
            var json = JsonSerializer.Serialize(devices);
            var bytes = Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
