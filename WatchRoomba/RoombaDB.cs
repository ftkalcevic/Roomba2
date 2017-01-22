using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WatchRoomba
{
    class RoombaDB
    {
        static string ConnectionString = @"Data Source = Server\SQLEXPRESS;Initial Catalog = Roomba2; Integrated Security = True;";

        public static int NewMission()
        {
            int id = 0;
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
            return id;
        }

        public static void EndMission(int MissionId)
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

        public static void NewPosition(int MissionId, int tick, int x, int y, int theta, int battery)
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

        internal static void UpdateCurrentStats(DateTime timestamp, string status, DateTime? nextMission)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.UpdateCurrentStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@LastUpdate", SqlDbType.DateTime).Value = timestamp;
                    cmd.Parameters.Add("@Status", SqlDbType.VarChar,100).Value = status;
                    if ( nextMission != null )
                        cmd.Parameters.Add("@NextMission", SqlDbType.DateTime).Value = nextMission;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
