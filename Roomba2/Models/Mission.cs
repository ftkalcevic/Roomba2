using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Roomba2.Models
{
    public class Mission
    {
        public int MissionNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String Cycle { get; set; }
        public String Phase { get; set; }
        public String Initiator { get; set; }
        public int BatteryPercent { get; set; }
        public int Error { get; set; }
    }
}