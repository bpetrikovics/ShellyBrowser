//using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace Shelly_OTA_Win
{
    class ShellyFirmwareAPI
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string Baseurl = "https://api.shelly.cloud/files/firmware";
        private static readonly List<ShellyFirmwareVersion> fwdata = new();

        // TODO: properly parse and interpret shelly API response
        public static async void Init()
        {
            var json = await client.GetStringAsync(Baseurl);
            JObject data = JObject.Parse(json);

            // Check if API reports the data is healthy
            if ((bool)(data["isok"]) is false)
            {
                throw new InvalidOperationException();
            }

            foreach (var (model, device) in data["data"].ToObject<Dictionary<string, ShellyFirmwareVersion>>())
            {
                device.deviceModel = model;
                fwdata.Add(device);
            }
        }

        public static string getLatestVersionForModel(string model)
        {
            return fwdata.Find(x => x.deviceModel == model).availableVersion;
        }

        public static ShellyFirmwareVersion getLatestFirmware(ShellyDevice device)
        {
            return fwdata.Find(x => x.deviceModel == device.type);
        }
    }

    class ShellyFirmwareVersion
    {
        public string deviceModel { get; set; }
        public string availableVersion { get; set; }
        public string downloadUrl { get; set; }

        public ShellyFirmwareVersion(string model, string version, string url)
        {
            this.deviceModel = model;
            this.availableVersion = version;
            this.downloadUrl = url;
        }

        public override string ToString()
        {
            return $"<ShellyFirmware model={this.deviceModel}, version={this.availableVersion}, url={this.downloadUrl}>";
        }
    }
}
