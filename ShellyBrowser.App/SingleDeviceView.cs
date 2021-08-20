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
            InternetAccessLabel.Text = device.update_mismatch ? "This device might not have direct internet access, use OTA proxy to update" : "Device has direct internet access, OTA proxy probably not needed";
        }
    }
}
