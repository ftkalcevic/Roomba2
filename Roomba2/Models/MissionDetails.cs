using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Roomba2.Models
{
   
    public class MissionDetails
    {
        public int MissionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int[] x { get; set; }
        public int[] y { get; set; }
        public int[] theta { get; set; }
        public int[] battery { get; set; }
    }
}