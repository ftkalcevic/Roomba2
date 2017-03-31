using System.Diagnostics;
using System.ServiceProcess;

namespace WatchRoomba2
{
    class Program
    {
        static TraceSource ts = new TraceSource("RoombaTrace");

        static void Main(string[] args)
        {
            {
                ts.TraceInformation("WatchRoomba2 program service starting");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new WatchRoomba2Service()
                };
                ServiceBase.Run(ServicesToRun);
                ts.TraceInformation("WatchRoomba2 program service exiting");
            }

            //ts.TraceInformation("WatchRoomba2 program starting");

            //WatchRoomba2 roomba = new WatchRoomba2();
            //roomba.oExit.WaitOne();

            //ts.TraceInformation("WatchRoomba2 program exiting");
        }

    }
}
