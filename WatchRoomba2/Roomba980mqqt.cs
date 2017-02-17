using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace WatchRoomba2
{
    class Roomba980mqqt
    {
        public delegate void UpdatesEvent(Dictionary<string, object> changes);

        private static TraceSource ts = new TraceSource("RoombaTrace");

        private string sHost;
        private int iPort;
        private string sUser;
        private string sPwd;
        private MqttClient client;
        //public int lastError { get; }

        public event UpdatesEvent updatesReceived;

        public struct Pose
        {
            public int theta;
            public struct Point
            {
                public int x;
                public int y;
            };
            public Point point;
        }

        public struct Signal
        {
            public int snr;
            public int rssi;
        }

        public struct CleanMissionStatus
        {
            public string cycle;
            public string phase;
            public int expireM;
            public int rechrgM;
            public int error;
            public int notReady;
            public int mssnM;
            public int sqft;
            public string initiator;
            public int nMssn;
        };
        

        public struct CleanSchedule
        {
            public string[] cycle;
            public int[] h;
            public int[] m;
        };

        public Roomba980mqqt(string sHost, int iPort, string sUser, string sPwd)
        {
            this.sHost = sHost;
            this.iPort = iPort;
            this.sUser = sUser;
            this.sPwd = sPwd;
        }

        public bool Connect()
        {
            if (client != null)
                Close();
     
            client = new MqttClient(sHost, iPort, true, MqttSslProtocols.TLSv1_0, (a,b,c,d) => { return true; }, (a,b,c,d,e)=> { return null; });

            int lastError = client.Connect(sUser, sUser, sPwd);
            if (lastError == 0)
            {
                client.MqttMsgPublishReceived += MqttMsgPublishReceived;
                client.Subscribe(new string[] { "#" }, new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            }
            else
            {
                Close();
            }
            return lastError == 0;
        }

        private void MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            string sTopic = e.Topic;
            string sMsg = System.Text.Encoding.UTF8.GetString(e.Message);
            ts.TraceInformation(sTopic + " => " + sMsg);

            if (updatesReceived != null)
            { 
                JObject msg = JObject.Parse(sMsg);

                JEnumerable<JToken> updates = msg["state"]["reported"].Children();
                Dictionary<string, object> parsedUpdates = new Dictionary<string, object>();

                foreach (JProperty update in updates)
                {
                    string sName = update.Name;

                    switch (sName)
                    {
                        case "pose":
                            parsedUpdates[sName] = JsonConvert.DeserializeObject<Pose>(update.First.ToString());
                            break;
                        case "signal":
                            parsedUpdates[sName] = JsonConvert.DeserializeObject<Signal>(update.First.ToString());
                            break;
                        case "cleanMissionStatus":
                            parsedUpdates[sName] = JsonConvert.DeserializeObject<CleanMissionStatus>(update.First.ToString());
                            break;
                        case "cleanSchedule":
                            parsedUpdates[sName] = JsonConvert.DeserializeObject<CleanSchedule>(update.First.ToString());
                            break;
                        default:
                            parsedUpdates[sName] = update.First.ToString();
                            break;
                    }
                }
                if (parsedUpdates.Count > 0)
                    updatesReceived(parsedUpdates);
            }
        }

        public void Close()
        {
            client.Disconnect();
            client = null;
        }
    }
}
