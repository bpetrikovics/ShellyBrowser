using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ShellyBrowserApp
{
    public class ShellyUpdateStatus
    {
        public string status;
        public bool has_update;
        public string new_version;
        public string old_version;
        public string beta_version;

        [JsonConstructor]
        public ShellyUpdateStatus(string status, bool has_update, string new_version, string old_version, string beta_version)
        {
            this.status = status;
            this.has_update = has_update;
            this.new_version = new_version;
            this.old_version = old_version;
            this.beta_version = beta_version;
        }
    }
}
