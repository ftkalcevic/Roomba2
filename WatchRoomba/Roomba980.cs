using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

//var requestOptions = {
//    -'method': 'POST',
//    -'uri': 'https://' + host + ':443/umi',
//    'strictSSL': false,
//    'headers': {
//      -'Content-Type': 'application/json',
//      -'Connection': 'close',
//      -'User-Agent': 'aspen%20production/2618 CFNetwork/758.3.15 Darwin/15.4.0',
//      -'Content-Encoding': 'identity',
//      -'Authorization': 'Basic ' + new Buffer('user:' + password).toString('base64'),
//      -'Accept': '*/*',
//      -'Accept-Language': 'en-us',
//      -'Host': host
//    }
//  };
namespace WatchRoomba
{
    public class Roomba
    {
        class Request
        {
            public string @do;
            public string[] args;
            public int id;
        }

        public class Point
        {
            public int x;
            public int y;
        }

        public class Position
        {
            public int theta;
            public Point point;
        }

        public class MissionResponse
        {
            public int flags;
            public string cycle;
            public string phase;
            public Position pos;
            public int batPct;
            public int expireM;
            public int rechrgM;
            public int error;
            public int notReady;
            public int mssnM;
            public int sqft;
        }

        public class MissionResponseOK
        {
            public MissionResponse ok;
            public int id;
        }

        public class TimeResponseOK
        {
            public TimeResponse ok { get; set; }
            public int id { get; set; }
        }

        public class TimeResponse
        {
            public string d { get; set; }
            public int h { get; set; }
            public int m { get; set; }
        }


        public class WeekResponseOK
        {
            public WeekResponse ok { get; set; }
            public int id { get; set; }
        }

        public class WeekResponse
        {
            public string[] cycle { get; set; }
            public int[] h { get; set; }
            public int[] m { get; set; }
        }

        public class BbrunResponseOK
        {
            public BbrunResponse ok { get; set; }
            public int id { get; set; }
        }

        public class BbrunResponse
        {
            public int hr { get; set; }
            public int min { get; set; }
            public int sqft { get; set; }
            public int nStuck { get; set; }
            public int nScrubs { get; set; }
            public int nPicks { get; set; }
            public int nPanics { get; set; }
            public int nCliffsF { get; set; }
            public int nCliffsR { get; set; }
            public int nMBStll { get; set; }
            public int nWStll { get; set; }
            public int nCBump { get; set; }
        }

        static HttpClient client = new HttpClient();
        static string user = "";
        static string password = "";
        static string host = "";
        static int id;

        static Roomba()
        {
            id = 0;
            user = ConfigurationManager.AppSettings["RoombaUser"].ToString();
            password = ConfigurationManager.AppSettings["RoombaPassword"].ToString();
            host = ConfigurationManager.AppSettings["RoombaHost"].ToString();

            // accept self signed certs
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            client.BaseAddress = new Uri("https://" + host + ":443");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us"));

            string sCredentials = "user:" + password;
            byte[] oCredentials = Encoding.UTF8.GetBytes(sCredentials);
            string sCredentialsEncoded = Convert.ToBase64String(oCredentials);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", sCredentialsEncoded);
            //client.DefaultRequestHeaders.Add("User-Agent", "aspen%20production/2618 CFNetwork/758.3.15 Darwin/15.4.0");
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));
            client.DefaultRequestHeaders.Host = host;
        }

        private int NextId()
        {
            id++;
            if (id > 1000)
                id = 1;
            return id;
        }

        public class RequestOp
        {
            public string @do { get; set; }
            public object[] args { get; set; }
            public int id { get; set; }
        }

        public Roomba()
        {
        }

        public async Task<T> Get<T>(string requestType)
        {
            T ret = default(T);

            Request r = new Request();
            r.@do = "get";
            r.args = new string[] { requestType };
            r.id = NextId();

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("umi", r);

                response.EnsureSuccessStatusCode();

                //MissionResponseOK output = await response.Content.ReadAsAsync<MissionResponseOK>();
                var s = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.Print(s);
                ret = JsonConvert.DeserializeObject<T>(s);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
            }
            return ret;
        }

        public async Task<bool> OpTask(string cmd)
        {
            bool bRet = false;
            string sRequest = "{\"do\":\"set\",\"args\":[\"cmd\", {\"op\":\"" + cmd + "\"}],\"id\":" + NextId().ToString() + "}"; // c# can't make a class that does "cmd" by itself

            try
            {
                StringContent content = new StringContent(sRequest, Encoding.UTF8);
                HttpResponseMessage response = await client.PostAsync("umi", content);
                response.EnsureSuccessStatusCode();
                bRet = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
            }
            return bRet;
        }



        public async Task<bool> Start() { return await OpTask("start"); }
        public async Task<bool> Stop() { return await OpTask("stop"); }
        public async Task<bool> Pause() { return await OpTask("pause"); }
        public async Task<bool> Resume() { return await OpTask("resume"); }
        public async Task<bool> Home() { return await OpTask("dock"); }


        public Task<MissionResponseOK> GetMission() { return Get<MissionResponseOK>("mssn"); }
        public Task<TimeResponseOK> GetTime() { return Get<TimeResponseOK>("time"); }
        public Task<WeekResponseOK> GetWeek() { return Get<WeekResponseOK>("week"); }
        public Task<BbrunResponseOK> GetBbrun() { return Get<BbrunResponseOK>("bbrun"); }
    }
}
