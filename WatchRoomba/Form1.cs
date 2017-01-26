using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WatchRoomba
{
    public partial class Form1 : Form
    {
        static TraceSource ts = new TraceSource("RoombaTrace");
        private const int PollInterval = 1000;
        private Timer t;
        private Roomba r;
        Task<Roomba.MissionResponseOK> missionResponse;
        DynamicMap map;
        int MissionId;
        int MissionTick;
        string last_phase;

        public Form1()
        {
            InitializeComponent();

            ts.TraceInformation("WatchRoomba starting");

            if (r == null)
                r = new Roomba();
        }

        private async void btnWatch_Click(object sender, EventArgs e)
        {
            if (missionResponse != null)
            {
                await missionResponse;
            }

            //Roomba.TimeResponseOK time = await r.GetTime();
            //Roomba.WeekResponseOK week = await r.GetWeek();
            //Roomba.BbrunResponseOK bbrun = await r.GetBbrun();

            if (t != null)
                t.Stop();

            t = new Timer();
            t.Interval = PollInterval;
            t.Tick += T_Tick;
            t.Start();
        }

        private async void T_Tick(object sender, EventArgs e)
        {
            t.Stop();

            if (r == null)
                r = new Roomba();

            MissionTick++;

            missionResponse = r.GetMission();

            Roomba.MissionResponseOK respOK = await missionResponse;
            missionResponse = null;
            if (respOK != null)
            {
                Roomba.MissionResponse resp = respOK.ok;

                ts.TraceInformation("{0} ({1},{2})@{3} {4}", resp.phase, resp.pos.point.x, resp.pos.point.y, resp.pos.theta, resp.batPct);
                resp.pos.point.x = -resp.pos.point.x;

                if (last_phase != resp.phase)
                {
                    if (last_phase == "run")
                        EndRun();

                    last_phase = resp.phase;
                    if (resp.phase == "run")
                        StartNewRun();
                }
                if (resp.phase == "run")
                {
                    RoombaDB.NewPosition(MissionId, MissionTick, resp.pos.point.x, resp.pos.point.y, resp.pos.theta, resp.batPct);
                    map.Update(0, resp.pos.point.x, resp.pos.point.y, resp.pos.theta);
                    map.Render(panelMap);
                    lblNextMission.Text = "";
                }

                if ( MissionTick % (10*60) == 10) // Every 10 minutes
                {
                    Roomba.TimeResponseOK time = await r.GetTime();
                    Roomba.WeekResponseOK week = await r.GetWeek();

                    DateTime dtRoombaTime = new DateTime(2000, 1, 1, time.ok.h, time.ok.m, 0);
                    TimeSpan tsNextMission = GetNextMission(time,week);
                    if (tsNextMission.TotalSeconds != 0)
                    {
                        if (tsNextMission.Days == 0)
                            if (tsNextMission.Hours == 0)
                                lblNextMission.Text = tsNextMission.ToString(@"m\ \m\i\n");
                            else
                                lblNextMission.Text = tsNextMission.ToString(@"h\ \h\o\u\r\s\ m\ \m\i\n");
                        else
                            lblNextMission.Text = tsNextMission.ToString(@"d\ \d\a\y\s\ h\ \h\o\u\r\s\ m\ \m\i\n");
                    }
                    lblRoombaTime.Text = time.ok.d + " " + time.ok.h.ToString() + ":" + time.ok.m.ToString("00");
                    RoombaDB.UpdateCurrentStats(DateTime.Now, resp.phase, (int)tsNextMission.TotalSeconds, dtRoombaTime, resp.flags, resp.batPct, resp.error, resp.notReady);
                }
                else
                    RoombaDB.UpdateCurrentStats(DateTime.Now, resp.phase, null, null, resp.flags, resp.batPct, resp.error, resp.notReady);

                UpdateStats(resp);
            }

            t.Start();
        }

        private void EndRun()
        {
            RoombaDB.EndMission(MissionId);
        }

        private void StartNewRun()
        {
            map = new DynamicMap();
            MissionId = RoombaDB.NewMission();
            MissionTick = 0;
        }

        private void UpdateStats(Roomba.MissionResponse resp)
        {
            lblCycle.Text = resp.cycle;
            lblPhase.Text = resp.phase;
            lblPosition.Text = string.Format("{0},{1}/{2}", resp.pos.point.x, resp.pos.point.y, resp.pos.theta);
            lblBattery.Text = resp.batPct.ToString();
            lblExpireM.Text = resp.expireM.ToString();
            lblRechrgM.Text = resp.rechrgM.ToString();
            lblError.Text = resp.error.ToString();
            lblNotReady.Text = resp.notReady.ToString();
            lblMssnM.Text = resp.mssnM.ToString();
            lblSqft.Text = resp.sqft.ToString();
        }

        private TimeSpan GetNextMission(Roomba.TimeResponseOK time, Roomba.WeekResponseOK week )
        {
            TimeSpan t = TimeSpan.Zero;

            if (time != null && week != null)
            {
                int today = -1;
                switch (time.ok.d)
                {
                    case "sun": today = 0; break;
                    case "mon": today = 1; break;
                    case "tue": today = 2; break;
                    case "wed": today = 3; break;
                    case "thu": today = 4; break;
                    case "fri": today = 5; break;
                    case "sat": today = 6; break;
                }
                int days = 0;
                for (int i = 0; i < 8; i++)
                {
                    bool bMatch = false;
                    if (week.ok.cycle[today] != "none")   // Schedule for today?
                    {
                        bMatch = true;
                        if (i == 0) // have we already finished today's run
                        {
                            if (week.ok.h[today] < time.ok.h)
                                bMatch = false;
                            else if (week.ok.h[today] == time.ok.h && week.ok.m[today] < time.ok.m)
                                bMatch = false;
                        }
                        if (bMatch)
                        {
                            int minutes = days * 24 * 60;
                            minutes += (week.ok.h[today] - time.ok.h) * 60;
                            minutes += week.ok.m[today] - time.ok.m;
                            t = TimeSpan.FromMinutes(minutes);
                            break;
                        }
                    }
                    days++;
                    today = (today + 1) % 7;
                }
            }
            return t;
        }

        private async void btnHome_Click(object sender, EventArgs e)
        {
            if (r == null)
                r = new Roomba();
            await r.Home();
        }

        private async void btnResume_Click(object sender, EventArgs e)
        {
            if (r == null)
                r = new Roomba();
            await r.Resume();
        }

        private async void btnPause_Click(object sender, EventArgs e)
        {
            if (r == null)
                r = new Roomba();
            await r.Pause();
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            if (r == null)
                r = new Roomba();
            await r.Stop();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (r == null)
                r = new Roomba();
            await r.Start();
        }

        private void panelMap_Resize(object sender, EventArgs e)
        {
            if ( map != null )
                map.Render(panelMap);
        }
    }
}
