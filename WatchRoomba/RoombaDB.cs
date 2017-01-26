using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Configuration;

namespace WatchRoomba
{
    class RoombaDB
    {
        static string ConnectionString = ConfigurationManager.ConnectionStrings["RoombaDB"].ConnectionString;
        static TraceSource ts = new TraceSource("RoombaTrace");

        public static int NewMission()
        {
            int id = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.NewMission", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            id = rdr.GetInt32(rdr.GetOrdinal("NewMissionId"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
            return id;
        }

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
                        cmd.Parameters.Add("@MissionId", SqlDbType.Int).Value = MissionId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ts.TraceEvent(TraceEventType.Error, 0, e.ToString());
            }
        }

        public static void NewPosition(int MissionId, int tick, int x, int y, int theta, int battery)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.NewPosition", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MissionId", SqlDbType.Int).Value = MissionId;
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

        internal static void UpdateCurrentStats(DateTime timestamp, string status, int? nextMission, DateTime? roombaTime, int flags, int batteryPercentage, int error, int notReady)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.UpdateCurrentStatus", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@LastUpdate", SqlDbType.DateTime).Value = timestamp;
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar,100).Value = status;
                        cmd.Parameters.Add("@Flags", SqlDbType.Int).Value = flags;
                        cmd.Parameters.Add("@BatteryPercentage", SqlDbType.Int).Value = batteryPercentage;
                        cmd.Parameters.Add("@Error", SqlDbType.Int).Value = error;
                        cmd.Parameters.Add("@NotReady", SqlDbType.Int).Value = notReady;
                        if ( nextMission != null )
                            cmd.Parameters.Add("@NextMission", SqlDbType.Int).Value = nextMission;
                        if (roombaTime != null)
                            cmd.Parameters.Add("@RoombaTime", SqlDbType.DateTime).Value = roombaTime;

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
