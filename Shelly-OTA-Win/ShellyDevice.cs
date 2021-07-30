using System;
using System.Threading.Tasks;
using System.Net.Http;

using Makaretu.Dns;
using Newtonsoft.Json;

namespace Shelly_OTA_Win
{
    // IEquatable makes List.Contains() work with ShellyDevice objects
    // It requires to implement Equals and GetHashCode
    class ShellyDevice : System.IEquatable<ShellyDevice>
    {
        // fields that are filled via JSON data from Shelly's status REST api
        public string type { get; set; }
        public string mac { get; set; }
        public string auth { get; set; }
        public string fw { get; set; }

        // fields used and controlled internally
        public string address { get; set; }
        public string name { get; set; }
        public DateTimeOffset lastseen { get; set; }

        private static readonly HttpClient client = new HttpClient();

        [JsonConstructor]
        public ShellyDevice(string type, string mac, string auth, string fw)
        {
            this.type = type;
            this.mac = mac;
            this.auth = auth;
            this.fw = fw;
        }

        public static async Task<ShellyDevice> Discover(AddressRecord address)
        {
            // TODO: error handling
            var result = await client.GetStringAsync($"http://{address.Address.ToString()}/shelly");
            ShellyDevice dev = JsonConvert.DeserializeObject<ShellyDevice>(result);

            dev.address = address.Address.ToString();
            dev.name = address.Name.ToString();
            dev.UpdateLastSeen();

            return dev;
        }

        public void UpdateLastSeen()
        {
            this.lastseen = (DateTimeOffset)DateTime.Now;
        }

        public int Age()
        {
            return (int)(DateTime.Now - this.lastseen).TotalSeconds;
        }

        public bool Equals(ShellyDevice other)
        {
            if (other == null)
            {
                return false;
            }
            
            return this.mac == other.mac;
        }

        public override bool Equals(System.Object other)
        {
            if (other == null)
            {
                return false;
            }

            ShellyDevice otherdev = other as ShellyDevice;
            if (otherdev == null)
            {
                return false;
            }
            return this.Equals(otherdev);
        }

        public override int GetHashCode()
        {
            return this.mac.GetHashCode();
        }

    }
}
