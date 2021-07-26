//using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json;

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

        // TODO: properly parse and interpret shelly API response
        public static async void Init()
        {
            var result = await client.GetStringAsync(Baseurl);
            dynamic jsondata = JsonConvert.DeserializeObject(result);
            var fwlist = jsondata.data;
        }
    }

 
}
