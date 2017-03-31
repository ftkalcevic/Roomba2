using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Threading;

namespace WatchRoomba2
{
    class WatchRoomba2
    {
        private TraceSource ts = new TraceSource("RoombaTrace");

        private const int RECONNECT_TIMEOUT = 60 * 1000;    // 1 minute inactivity time out
        private string host = ConfigurationManager.AppSettings["RoombaHost"].ToString();
        private int port = 8883;
        private string username = ConfigurationManager.AppSettings["RoombaUser"].ToString();
        private string password = ConfigurationManager.AppSettings["RoombaPassword"].ToString();
        
        private Roomba980mqqt roomba;
        private int batPct = 0;
        private int tick = 0;
        private int missionNumber = -1;
        private bool firstPose = true;
        private string phase = "";
        private Roomba980mqqt.CleanSchedule schedule;
        private int NextMission = 0;
        public ManualResetEvent oExit;
        private System.Timers.Timer activityTimer;


        public WatchRoomba2()
        {
            ts.TraceInformation("WatchRoomba2 starting");

            oExit = new ManualResetEvent(false);

            roomba = new Roomba980mqqt(host, port, username, password);

            roomba.updatesReceived += roombaUpdatesReceived;
            roomba.Connect();

            activityTimer = new System.Timers.Timer();
            activityTimer.Interval = RECONNECT_TIMEOUT;
            activityTimer.Elapsed += timerElapsed;
            activityTimer.Start();
        }

        public void Close()
        {
            roomba.Close();
            roomba = null;
        }

        private void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // If we are on a mission, reconnect so we can refresh some statistics, like bpct
            ts.TraceInformation("Inactivity timeout");
            roomba.Close();

            roomba = null;
            //roomba = new Roomba980mqqt(host, port, username, password);
            //roomba.updatesReceived += roombaUpdatesReceived;
            //roomba.Connect();
            // firstPose = true;
            oExit.Set();
        }

        private void roombaUpdatesReceived(Dictionary<string, object> changes)
        {
            activityTimer.Stop();
            activityTimer.Start();

            Dictionary<string, string> updates = new Dictionary<string, string>(); ;

            foreach (KeyValuePair<string, object> change in changes)
            {
                if (change.Key == "pose")
                {
                    Roomba980mqqt.Pose p = (Roomba980mqqt.Pose)change.Value;
                    System.Console.WriteLine(String.Format("pose ({0},{1}) {2}", p.point.x, p.point.y, p.theta));

                    if (firstPose)
                    {
                        firstPose = false;
                        System.Console.WriteLine("First pose");
                    }
                    else if (phase == "run")
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
                else if (change.Key == "cleanMissionStatus")
                {
                    CleanMissionStatusUpdate((Roomba980mqqt.CleanMissionStatus)change.Value);
                    UpdateStatus();
                }
                else if (change.Value.GetType() == typeof(string))
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
            if (updates.Count > 0)
                RoombaDB.UpdateProperties(updates);
        }

        private void UpdateNextMission()
        {
            NextMission = (int)GetNextMission().TotalSeconds;
            UpdateStatus();
        }

        private TimeSpan GetNextMission()
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

        private void CleanMissionStatusUpdate(Roomba980mqqt.CleanMissionStatus v)
        {
            if (v.phase == "run")
            {
                System.Console.WriteLine(String.Format("{0} {1} {2} {3}", v.nMssn, v.cycle, v.phase, v.sqft));
                RoombaDB.UpdateMission(DateTime.Now, v.nMssn, v.cycle, v.phase, v.initiator, v.error, batPct);
            }

            phase = v.phase;

            if (v.nMssn != missionNumber)
            {
                missionNumber = v.nMssn;
                tick = 1;
            }
        }

        private void UpdateStatus()
        {
            RoombaDB.UpdateRoombaStatus(DateTime.Now, phase, NextMission, batPct);
        }
    }
}
