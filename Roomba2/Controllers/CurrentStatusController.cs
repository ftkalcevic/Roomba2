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
    public class CurrentStatusController : ApiController
    {
        private const string RoombaDBName = "DB";

        public IHttpActionResult GetCurrentStatus()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[RoombaDBName].ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetCurrentStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return Ok(new CurrentStatus()
                        {
                            LastUpdate = rdr.GetDateTime(rdr.GetOrdinal("LastUpdate")),
                            Status = rdr.GetString(rdr.GetOrdinal("Status")),
                            NextMission = rdr.GetDateTime(rdr.GetOrdinal("NextMission")),
                            RoombaTime = rdr.GetDateTime(rdr.GetOrdinal("RoombaTime"))
                        });
                    }
                }
            }
            return NotFound();
        }

    }
}
