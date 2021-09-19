using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellyBrowserApp.Models
{
    public class ShellyFirmwareVersion
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
