using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Configuration;

namespace WatchRoomba2
{
    class Program
    {
        static TraceSource ts = new TraceSource("RoombaTrace");

        const int RECONNECT_TIMEOUT = 5 * 60 * 1000;    // drop connection and reconnect every 5 minutes
        static string host = ConfigurationManager.AppSettings["RoombaHost"].ToString();
        static int port = 8883;
        static string username = ConfigurationManager.AppSettings["RoombaUser"].ToString();
        static string password = ConfigurationManager.AppSettings["RoombaPassword"].ToString();

        static Roomba980mqqt roomba;
        static int batPct = 0;
        static int tick = 0;
        static int missionNumber = -1;
        static bool firstPose = true;
        static string phase = "";
        static Roomba980mqqt.CleanSchedule schedule;
        static int NextMission = 0;

        static void Main(string[] args)
        {
            ts.TraceInformation("WatchRoomba2 starting");
            roomba = new Roomba980mqqt(host, port, username, password);

            roomba.updatesReceived += roombaUpdatesReceived;
            roomba.Connect();

            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = RECONNECT_TIMEOUT;
            t.Elapsed += timerElapsed;
            t.Start();

            System.Console.ReadLine();
            roomba.Close();
            t.Stop();

            ts.TraceInformation("WatchRoomba2 exiting");
        }

        private static void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // If we are on a mission, reconnect so we can refresh some statistics, like bpct
            ts.TraceInformation("Reconnecting");
            roomba.Close();
            roomba.Connect();
            firstPose = true;
        }

        private static void roombaUpdatesReceived(Dictionary<string, object> changes)
        {
            Dictionary<string, string> updates = new Dictionary<string, string>(); ;

            foreach ( KeyValuePair<string,object> change in changes )
            {
                if ( change.Key == "pose")
                {
                    Roomba980mqqt.Pose p = (Roomba980mqqt.Pose)change.Value;
                    System.Console.WriteLine(String.Format("pose ({0},{1}) {2}", p.point.x, p.point.y, p.theta));

                    if (firstPose)
                    {
                        firstPose = false;
                        System.Console.WriteLine("First pose");
                    }
                    else if (phase=="run")
                    {
                        RoombaDB.NewPosition(missionNumber, tick, p.point.x, p.point.y, p.theta, batPct);
                        tick++;
                    }
                }
                else if (change.Key == "cleanSchedule")
                {
                    schedule = (Roomba980mqqt.CleanSchedule)change.Value;
                    UpdateNextMission();
                }
                else if ( change.Key == "cleanMissionStatus")
                {
                    CleanMissionStatusUpdate((Roomba980mqqt.CleanMissionStatus)change.Value);
                    UpdateStatus();
                }
                else if ( change.Value.GetType() == typeof(string))
                {
                    if (change.Key == "batPct")
                    {
                        batPct = Int32.Parse(change.Value.ToString());
                        UpdateStatus();
                    }

                    System.Console.WriteLine(String.Format("{0} => {1}", change.Key, change.Value.ToString()));
                    updates.Add(change.Key, change.Value.ToString());
                }
            }
            if ( updates.Count > 0 )
                RoombaDB.UpdateProperties(updates);
        }

        private static void UpdateNextMission()
        {
            NextMission = (int)GetNextMission().TotalSeconds;
            UpdateStatus();
        }

        private static TimeSpan GetNextMission()
        {
            // Just use Now, rather than the Roomba's time
            DateTime time = DateTime.Now;
            TimeSpan t = TimeSpan.Zero;

            int today = (int)time.DayOfWeek;
            int days = 0;
            for (int i = 0; i < 8; i++)
            {
                bool bMatch = false;
                if (schedule.cycle[today] != "none")   // Schedule for today?
                {
                    bMatch = true;
                    if (i == 0) // have we already finished today's run
                    {
                        if (schedule.h[today] < time.Hour)
                            bMatch = false;
                        else if (schedule.h[today] == time.Hour && schedule.m[today] < time.Minute)
                            bMatch = false;
                    }
                    if (bMatch)
                    {
                        int minutes = days * 24 * 60;
                        minutes += (schedule.h[today] - time.Hour) * 60;
                        minutes += schedule.m[today] - time.Minute;
                        t = TimeSpan.FromMinutes(minutes);
                        break;
                    }
                }
                days++;
                today = (today + 1) % 7;
            }
            return t;
        }

        private static void CleanMissionStatusUpdate(Roomba980mqqt.CleanMissionStatus v)
        {
            if (v.phase=="run")
            {
                System.Console.WriteLine(String.Format("{0} {1} {2} {3}", v.nMssn, v.cycle, v.phase, v.sqft));
                RoombaDB.UpdateMission(DateTime.Now, v.nMssn, v.cycle, v.phase, v.initiator, v.error, batPct);
            }

            phase = v.phase;

            if ( v.nMssn != missionNumber )
            {
                missionNumber = v.nMssn;
                tick = 1;
            }
        }

        private static void UpdateStatus()
        {
            RoombaDB.UpdateRoombaStatus(DateTime.Now, phase, NextMission, batPct);
        }
    }
}
