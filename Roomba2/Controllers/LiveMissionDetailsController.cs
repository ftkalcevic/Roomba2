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
    public class LiveMissionDetailsController : ApiController
    {
        private const string RoombaDBName = "DB";

        // GET: api/MissionDetails/5 
        public IHttpActionResult Get(int id)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[RoombaDBName].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetLiveMissionDetails_Test", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@LastTick", SqlDbType.Int).Value = id;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (!rdr.Read() || rdr.IsDBNull(0) )
                        return NotFound();

                    int LastTick = rdr.GetInt32(rdr.GetOrdinal("LastTick"));
                    int MissionId = rdr.GetInt32(rdr.GetOrdinal("MissionId"));
                    DateTime StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime"));

                    if (!rdr.NextResult())
                        return NotFound();

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

                    LiveMissionDetails md = new LiveMissionDetails();
                    md.StartTime = StartTime;
                    md.MissionId = MissionId;
                    md.LastTick = LastTick;
                    md.x = x.ToArray();
                    md.y = y.ToArray();
                    md.theta = theta.ToArray();
                    md.battery = battery.ToArray();

                    return Ok(md);
                }
            }
        }
    }
}
