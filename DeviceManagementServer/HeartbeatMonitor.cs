

using System;
using System.Threading.Tasks;

namespace DeviceManagementServer
{
    internal class HeartbeatMonitor
    {
        private readonly DeviceManager _manager;

        public HeartbeatMonitor(DeviceManager manager)
        {
            _manager = manager;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    bool changed = false;

                    foreach (var device in _manager.GetAllDevices().Values)
                    {
                        if (device.IsOnline &&
                            DateTime.UtcNow - device.LastHeartBeat > TimeSpan.FromSeconds(15))
                        {
                            device.IsOnline = false;
                            device.Status = "OFFLINE";
                            Console.WriteLine($"[AUTO OFFLINE] {device.DeviceId}");
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        await _manager.BroadcastAsync();
                    }

                    await Task.Delay(5000);
                }
            });
        }
    }
}
