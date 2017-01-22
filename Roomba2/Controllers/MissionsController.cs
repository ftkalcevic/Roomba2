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
                        missions.Add(new Mission()
                        {
                            MissionId = rdr.GetInt32(rdr.GetOrdinal("MissionId")),
                            StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime")),
                            EndTime = rdr.GetDateTime(rdr.GetOrdinal("EndTime"))
                        });
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
                    cmd.Parameters.Add("@MissionId", SqlDbType.Int).Value = id;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return Ok(new Mission()
                        {
                            MissionId = rdr.GetInt32(rdr.GetOrdinal("MissionId")),
                            StartTime = rdr.GetDateTime(rdr.GetOrdinal("StartTime")),
                            EndTime = rdr.GetDateTime(rdr.GetOrdinal("EndTime"))
                        });
                    }
                }
            }
            return NotFound();
        }
    }
}
