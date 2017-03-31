using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WatchRoomba2
{
    partial class WatchRoomba2Service : ServiceBase
    {
        private TraceSource ts = new TraceSource("RoombaTrace");
        WatchRoomba2 roomba;

        public WatchRoomba2Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ts.TraceInformation("WatchRoomba2 service starting");
            roomba = new WatchRoomba2();
        }

        protected override void OnStop()
        {
            roomba.Close();
            roomba = null;
            ts.TraceInformation("WatchRoomba2 service stopping");
        }
    }
}
