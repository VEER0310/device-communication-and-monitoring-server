using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
namespace DeviceManagementServer
{
    internal class DeviceManager
    {
        private readonly ConcurrentDictionary<string, Device> _devices = new ConcurrentDictionary<string, Device>();

        private WebSocketManager? _webSocketManager;

        public void SetWebSocketManager(WebSocketManager manager)
        {
            _webSocketManager = manager;
        }

        public void Register(string deviceId)
        {
            var device = new Device
            {
                DeviceId = deviceId,
                Status = "UKNOWNN",
                IsOnline = true,
                LastHeartBeat = DateTime.UtcNow
            };
            _devices.AddOrUpdate(deviceId, device, (k, v) => device);
            Console.WriteLine($"[REGISTERED] {deviceId}");

            _webSocketManager?.BroadcastDevicesAsync();
        }

        public void UpdateStatus(string deviceId, string status)
        {
            if (_devices.TryGetValue(deviceId, out var device))
            {
                device.Status = status;
                device.IsOnline = true;
                Console.WriteLine($"[STATUS] {deviceId} -> {status}");

                _webSocketManager?.BroadcastDevicesAsync();
            }
        }
        public void Heartbeat(string deviceId)
        {
            if (_devices.TryGetValue(deviceId, out var device))
            {
                device.LastHeartBeat = DateTime.UtcNow;
                device.IsOnline = true;
                Console.WriteLine($"[HEARTBEAT] {deviceId}");

                _webSocketManager?.BroadcastDevicesAsync();
            }
        }
        public void Disconnect(string deviceId)
        {
            if (_devices.TryGetValue(deviceId, out var device))
            {
                device.IsOnline = false;
                Console.WriteLine($"[DISCONNECTED] {deviceId}");

                _webSocketManager?.BroadcastDevicesAsync();
            }
        }

        public async Task BroadcastAsync()
        {
            if (_webSocketManager != null)
            {
                await _webSocketManager.BroadcastDevicesAsync();
            }
        }


        public ConcurrentDictionary<string, Device>
        GetAllDevices()
            {
                return _devices;
            }
    }
}
