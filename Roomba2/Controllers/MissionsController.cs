using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Roomba2.Models;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;

namespace Roomba2.Controllers
{
    public class MissionsController : ApiController
    {
        private const string RoombaDBName = "DB";

        public IEnumerable<Mission> GetAllMissions()
        {
            List<Mission> missions = new List<Mission>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[RoombaDBName].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetAllMissions", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while ( rdr.Read())
                    {
                        Mission m = new Mission();
                        m.MissionNumber = rdr.GetInt32(rdr.GetOrdinal("MissionNumber"));
                        m.StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime"));
                        m.EndTime = rdr.GetDateTime(rdr.GetOrdinal("LastUpdate"));
                        m.Cycle = rdr.GetString(rdr.GetOrdinal("Cycle"));
                        m.Phase = rdr.GetString(rdr.GetOrdinal("Phase"));
                        m.Initiator = rdr.GetString(rdr.GetOrdinal("Initiator"));
                        m.BatteryPercent = rdr.GetInt32(rdr.GetOrdinal("BatteryPercent"));
                        m.Error = rdr.GetInt32(rdr.GetOrdinal("Error"));
                        missions.Add(m);
                    }
                }
            }
            return missions;
        }
        public IHttpActionResult GetMission(int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[RoombaDBName].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetMission", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        Mission m = new Mission();
                        m.MissionNumber = rdr.GetInt32(rdr.GetOrdinal("MissionNumber"));
                        m.StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime"));
                        m.EndTime = rdr.GetDateTime(rdr.GetOrdinal("LastUpdate"));
                        m.Cycle = rdr.GetString(rdr.GetOrdinal("Cycle"));
                        m.Phase = rdr.GetString(rdr.GetOrdinal("Phase"));
                        m.Initiator = rdr.GetString(rdr.GetOrdinal("Initiator"));
                        m.Error = rdr.GetInt32(rdr.GetOrdinal("Error"));
                        m.BatteryPercent = rdr.GetInt32(rdr.GetOrdinal("BatteryPercent"));
                        return Ok(m);
                    }
                }
            }
            return NotFound();
        }
    }
}
