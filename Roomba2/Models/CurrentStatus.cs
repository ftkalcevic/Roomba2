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
        public DateTime NextMission { get; set; }
        public DateTime RoombaTime { get; set; }
    }
}