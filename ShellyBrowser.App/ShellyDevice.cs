using System;
using System.Net.Http;
using System.Threading.Tasks;

using Makaretu.Dns;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShellyBrowserApp
{
    // IEquatable makes List.Contains() work with ShellyDevice objects
    // It requires to implement Equals and GetHashCode
    public class ShellyDevice : System.IEquatable<ShellyDevice>
    {
        // fields that are filled via JSON data from Shelly's status REST api
        public string type { get; set; }
        public string mac { get; set; }
        public bool auth { get; set; }
        public string fw { get; set; }
        public bool sleep_mode { get; set; }

        // fields used and controlled internally
        public string address { get; set; }
        public string name { get; set; }
        public DateTimeOffset lastseen { get; set; }
        public bool stale { get; set; }
        public bool has_update { get; set; }
        public bool update_mismatch { get; set; }

        private JObject status_data;
        private static readonly HttpClient client = new HttpClient();

        [JsonConstructor]
        public ShellyDevice(string type, string mac, bool auth, string fw, bool sleep_mode = false)
        {
            this.type = type;
            this.mac = mac;
            this.auth = auth;
            this.fw = fw;
            this.sleep_mode = sleep_mode;
        }

        public static async Task<ShellyDevice> Discover(AddressRecord address)
        {
            // TODO: error handling
            var result = await client.GetStringAsync($"http://{address.Address.ToString()}/shelly");
            ShellyDevice dev = JsonConvert.DeserializeObject<ShellyDevice>(result);

            dev.address = address.Address.ToString();
            dev.name = address.Name.ToString();
            dev.stale = false;
            dev.UpdateLastSeen();

            result = await client.GetStringAsync($"http://{address.Address.ToString()}/status");
            dev.status_data = JObject.Parse(result);
            dev.has_update = (bool)dev.status_data[nameof(has_update)];

            // If the device is not aware of the newer firmware, it might not have direct access to the internet
            if ((dev.fw != ShellyFirmwareAPI.getLatestVersionForModel(dev.type)) && (dev.has_update is false))
            {
                dev.update_mismatch = true;
            }
            else
            {
                dev.update_mismatch = false;
            }

            return dev;
        }

        public async Task<string> StartUpdate(string ota_url = "")
        {
            string uri = $"http://{this.address}/ota";
            if (ota_url != "")
            {
                uri += "?url={ota_url}";
            }
            else
            {
                uri += "?update=true";
            }
            return await client.GetStringAsync(uri);
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
