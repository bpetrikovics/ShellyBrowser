using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShellyBrowserApp
{
    public partial class SingleDeviceView : UserControl
    {
        public SingleDeviceView()
        {
            InitializeComponent();
        }

        public void UpdateView(ShellyDevice device)
        {
            UpdateStatusLabel.Text = device.fw != ShellyFirmwareAPI.getLatestVersionForModel(device.type) ? "An updated firmware is available for this device" : "No firmware update for this device";
            InternetAccessLabel.Text = device.update_mismatch ? "The device is not aware of the new firmware, OTA proxy might be needed" : "Device firmware state consistent with Shelly firmware API response.";
        }
    }
}
