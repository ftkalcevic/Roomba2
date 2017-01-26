using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Roomba2.Models
{
    public class CurrentStatus
    {
        public DateTime LastUpdate { get; set; }
        public string Status { get; set; }
        public int NextMission { get; set; }
        public DateTime RoombaTime { get; set; }
        public int Flags { get; set; }
        public int BatteryPercentage { get; set; }
        public int Error { get; set; }
        public int NotReady { get; set; }
    }
}