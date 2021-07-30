//using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json.Linq;

// https://stackoverflow.com/questions/16339167/how-do-i-deserialize-a-complex-json-object-in-c-sharp-net


/*
 {
  "isok": true,
  "data": {
    "SHPLG-1": {
      "url": "http://shelly-api-eu.shelly.cloud/firmware/SHPLG-1.zip",
      "version": "20210115-103101/v1.9.4@e2732e05"
    },
    "SHPLG-S": {
      "url": "http://shelly-api-eu.shelly.cloud/firmware/SHPLG-S.zip",
      "version": "20210724-210557/v1.11.0-1PMfix-gdf51fe2"
    },
    ...
 */

namespace Shelly_OTA_Win
{
    class ShellyFirmwareAPI
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string Baseurl = "https://api.shelly.cloud/files/firmware";
        private static List<ShellyFirmwareVersion> fwdata = new();

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

            foreach (var (key, value) in data["data"].ToObject<Dictionary<string, ShellyFirmwareVersion>>())
            {
                value.deviceModel = key;
                fwdata.Add(value);
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
