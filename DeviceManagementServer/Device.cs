using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceManagementServer
{
    internal class Device
    {
        public string? DeviceId { get; set; }
        public string? Status { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastHeartBeat { get; set; }
    }
}
