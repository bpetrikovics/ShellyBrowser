using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace ShellyBrowserApp
{
    class PresentationService
    {
        private readonly ListView listview;
        private readonly Panel panel;
        private readonly GroupBox detailbox;
        private readonly GroupBox upgradebox;
        private readonly StatusStrip statusbar;

        private readonly ComboBox ipselector;
        private readonly TextBox bindport;

        private readonly SingleDeviceView singledeviceview;

        private delegate void SafeRefreshListViewDelegate(List<ShellyDevice> devices);

        public PresentationService(ListView listview, Panel panel, StatusStrip statusbar)
        {
            this.listview = listview;
            this.panel = panel;
            this.detailbox = (GroupBox)this.panel.Controls.Find("DetailBox", false).FirstOrDefault();
            this.upgradebox = (GroupBox)this.panel.Controls.Find("UpgradeBox", false).FirstOrDefault();
            this.statusbar = statusbar;

            this.ipselector = (ComboBox)upgradebox.Controls.Find("OtaBindIPSelector", false).First();
            this.bindport = (TextBox)upgradebox.Controls.Find("OtaPortTextBox", false).First();

            singledeviceview = new SingleDeviceView();

            // feed this into listbox
            string hostName = Dns.GetHostName();
            IPAddress[] localIPs = Dns.GetHostAddresses(hostName);

            foreach (var address in localIPs)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipselector.Items.Add(address);
                }
            }

            ipselector.SelectedIndex = ipselector.Items.Count - 1;
        }

        internal void NotifyMessage(string message)
        {
            MessageBox.Show(message, "Shelly Browser");
        }

        // FIXME: this loses selection since we clear the whole listview...
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
                        Item.ToolTipText = $"New firmware available: {ShellyFirmwareAPI.getLatestVersionForModel(device.type)}";
                    }

                    listview.Items.Add(Item);
                }

                listview.EndUpdate();
                if (listview.SelectedItems.Count == 0)
                {
                    panel.Enabled = false;
                }
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

        public void SelectedDeviceChanged(ListView.SelectedListViewItemCollection selected, DeviceInventory inventory)
        {
            if (selected.Count != 0)
            {
                var device = inventory.FindByMac(selected[0].SubItems[1].Text);
                UpdateStatus($"Device {device.address} last seen {device.Age()} seconds ago");
                panel.Enabled = true;

                // Enable and populate the detail area with device information
                detailbox.Text = device.name;
                detailbox.Enabled = true;
                singledeviceview.UpdateView(device);
                detailbox.Controls.Add(singledeviceview);

                isOtaSelected = device.update_mismatch;
            }
            else
            {
                panel.Enabled = false;
                detailbox.Controls.Remove(singledeviceview);
                detailbox.Text = "Shelly Device Details";
                detailbox.Enabled = false;
            }
        }

        public bool isOtaSelected
        {
            get
            {
                var box = (CheckBox)upgradebox.Controls.Find("UpdateProxyCheckbox", false).First();
                return box.Checked;
            }
            set
            {
                var box = (CheckBox)upgradebox.Controls.Find("UpdateProxyCheckbox", false).First();
                box.Checked = value;
            }
        }

        public string otaBindAddress
        {
            get
            {
                return this.ipselector.Text;
            }
        }

        public string otaBindPort
        {
            get
            {
                return this.bindport.Text;
            }
        }

        public void UpdateStatus(string text)
        {
            if (statusbar.InvokeRequired)
            {
                statusbar.Invoke(new Action(() => statusbar.Items.Find("StatusLabel", false).First().Text = text));
            }
            else
            {
                statusbar.Items.Find("StatusLabel", false).First().Text = text;
            }
        }

        // Device count will be always updated from background thread, guaranteed not to be on the UI thread,
        // so we can skip the check we do above
        public void UpdateDeviceCount(int num)
        {
            statusbar.Invoke(new Action(() => statusbar.Items.Find("DeviceCountLabel", false).First().Text = $"{num} device" + (num > 1 ? "s" : "")));
        }

    }
}
