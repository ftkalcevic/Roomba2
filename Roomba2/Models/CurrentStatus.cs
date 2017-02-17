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
        public int BatteryPercent { get; set; }
    }
}