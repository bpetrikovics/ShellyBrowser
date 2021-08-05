using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shelly_OTA_Win
{
    class PresentationService
    {
        private ListView listview;
        private delegate void SafeRefreshListViewDelegate(List<ShellyDevice> devices);

        public PresentationService(ListView listview)
        {
            this.listview = listview;
        }

        public void RefreshListView(List<ShellyDevice> devices)
        {
            if (listview.InvokeRequired)
            {
                var d = new SafeRefreshListViewDelegate(RefreshListView);
                listview.Invoke(d, new object[] { devices });
            }
            else
            {
                listview.BeginUpdate();
                listview.Items.Clear();
                foreach (var device in devices)
                {
                    var Item = new ListViewItem(device.name);
                    Item.ImageIndex = device.auth is true ? 1 : 0;

                    Item.SubItems.Add(device.mac);
                    Item.SubItems.Add(device.address);
                    Item.SubItems.Add(device.type);
                    Item.SubItems.Add(device.fw);

                    if (device.stale)
                    {
                        Item.UseItemStyleForSubItems = true;
                        Item.ForeColor = Color.DarkGray;
                        Item.ToolTipText = $"This device has not sent an update recently, it might have gone offline";
                    }
                    else if (device.fw != ShellyFirmwareAPI.getLatestVersionForModel(device.type))
                    {
                        Item.UseItemStyleForSubItems = false;
                        Item.SubItems[4].ForeColor = Color.Red;
                        Item.ToolTipText = $"New firmware available: {ShellyFirmwareAPI.getLatestVersionForModel(device.type)}";
                    }

                    listview.Items.Add(Item);
                }
                listview.EndUpdate();
            }
        }

        public void VisitDeviceLink(ShellyDevice device, string path = "")
        {
            if (device is null)
            {
                // Should not happen, but handle error here anyway
                MessageBox.Show("VisitDeviceLink() called with null device. This should not happen.");
                return;
            }

            ProcessStartInfo psi = new()
            {
                FileName = $"http://{device.address}{path}",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

    }
}
