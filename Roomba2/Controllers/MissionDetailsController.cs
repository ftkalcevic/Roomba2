using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using Roomba2.Models;


namespace Roomba2.Controllers
{
    public class MissionDetailsController : ApiController
    {
        private const string RoombaDBName = "DB";
        // GET: api/MissionDetails/5
        public IHttpActionResult Get(int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[RoombaDBName].ConnectionString))
            {
                Mission mission = null;
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetMission", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        mission = new Mission();
                        mission.MissionNumber = rdr.GetInt32(rdr.GetOrdinal("MissionNumber"));
                        mission.StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime"));
                        mission.EndTime = rdr.GetDateTime(rdr.GetOrdinal("LastUpdate"));
                        mission.Cycle = rdr.GetString(rdr.GetOrdinal("Cycle"));
                        mission.Phase = rdr.GetString(rdr.GetOrdinal("Phase"));
                        mission.Initiator = rdr.GetString(rdr.GetOrdinal("Initiator"));
                        mission.Error = rdr.GetInt32(rdr.GetOrdinal("Error"));
                        mission.BatteryPercent = rdr.GetInt32(rdr.GetOrdinal("BatteryPercent"));
                    }
                    rdr.Close();
                }
                if ( mission != null )
                {
                    using (SqlCommand cmd = new SqlCommand("GetMissionDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MissionNumber", SqlDbType.Int).Value = id;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        List<int> x = new List<int>();
                        List<int> y = new List<int>();
                        List<int> theta = new List<int>();
                        List<int> battery = new List<int>();
                        while (rdr.Read())
                        {
                            x.Add(rdr.GetInt32(rdr.GetOrdinal("x")));
                            y.Add(rdr.GetInt32(rdr.GetOrdinal("y")));
                            theta.Add(rdr.GetInt32(rdr.GetOrdinal("theta")));
                            battery.Add(rdr.GetInt32(rdr.GetOrdinal("battery")));
                        }

                        MissionDetails md = new MissionDetails();
                        md.StartTime = mission.StartTime;
                        md.EndTime = mission.EndTime;
                        md.MissionNumber = mission.MissionNumber;
                        md.x = x.ToArray();
                        md.y = y.ToArray();
                        md.theta = theta.ToArray();
                        md.battery = battery.ToArray();

                        return Ok(md);
                    }
                }
            }
            return NotFound();
        }
    }
}
