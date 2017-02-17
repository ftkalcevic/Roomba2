using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Configuration;

namespace WatchRoomba2
{
    class RoombaDB
    {
        static string ConnectionString = ConfigurationManager.ConnectionStrings["RoombaDB"].ConnectionString;
        static TraceSource ts = new TraceSource("RoombaTrace");
        
        public static void EndMission(int MissionId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.EndMission", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = MissionId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }

        public static void NewPosition(int MissionNumber, int tick, int x, int y, int theta, int battery)
        {
            //System.Console.WriteLine(string.Format("{0} {1} {2} {3}", MissionNumber, tick, x, y));
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.NewPosition", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = MissionNumber;
                        cmd.Parameters.Add("@Tick", SqlDbType.Int).Value = tick;
                        cmd.Parameters.Add("@x", SqlDbType.Int).Value = x;
                        cmd.Parameters.Add("@y", SqlDbType.Int).Value = y;
                        cmd.Parameters.Add("@theta", SqlDbType.Int).Value = theta;
                        cmd.Parameters.Add("@battery", SqlDbType.Int).Value = battery;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }

        public static void UpdateProperties(Dictionary<string,string> updates)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.UpdateProperty", con))
                    {
                        DataTable table = new DataTable();
                        table.Columns.Add("Name", typeof(String));
                        table.Columns.Add("Value", typeof(String));

                        foreach (KeyValuePair<string,string> i in updates)
                            table.Rows.Add(i.Key, i.Value.Substring(0,Math.Min(i.Value.Length, 500)));

                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter p = cmd.Parameters.Add("@Properties", SqlDbType.Structured);
                        p.TypeName = "dbo.PropertiesTableType";
                        p.Value = table;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }

        internal static void UpdateMission(DateTime timestamp, int nMssn, string cycle, string phase, string initiator, int error, int batPct)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.UpdateMissionStatus", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@LastUpdate", SqlDbType.DateTime).Value = timestamp;
                        cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = nMssn;
                        cmd.Parameters.Add("@Cycle", SqlDbType.VarChar, 100).Value = cycle;
                        cmd.Parameters.Add("@Phase", SqlDbType.VarChar, 100).Value = phase;
                        cmd.Parameters.Add("@Initiator", SqlDbType.VarChar, 100).Value = initiator;
                        cmd.Parameters.Add("@Error", SqlDbType.Int).Value = error;
                        cmd.Parameters.Add("@BatteryPercent", SqlDbType.Int).Value = nMssn;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }

        internal static void UpdateRoombaStatus(DateTime now, string phase, int nextMission, int batPct)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.UpdateCurrentStatus", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@LastUpdate", SqlDbType.DateTime).Value = now;
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 30).Value = phase;
                        cmd.Parameters.Add("@NextMission", SqlDbType.Int).Value = nextMission;
                        cmd.Parameters.Add("@BatteryPercent", SqlDbType.Int).Value = batPct;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }
    }
}
