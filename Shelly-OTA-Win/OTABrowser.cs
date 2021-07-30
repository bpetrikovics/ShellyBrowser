using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Makaretu.Dns;

/*
 * TODO:
 *   - Remove stale devices (regular ping or just simply based on timer, if timestamp is too old?)
 * 
 * */

namespace Shelly_OTA_Win
{
    public partial class OTABrowser : Form
    {
        private delegate void SafeRefreshListViewDelegate(List<ShellyDevice> devices);

        public static MulticastService mdns = new MulticastService();
        public static ServiceDiscovery sd = new(mdns);

        private List<ShellyDevice> Devices = new();

        public OTABrowser()
        {
            FormClosing += new FormClosingEventHandler(OTABrowser_Closing);
            InitializeComponent();
        }

        private void RefreshListView(List<ShellyDevice> devices)
        {
            if (DeviceListView.InvokeRequired)
            {
                var d = new SafeRefreshListViewDelegate(RefreshListView);
                DeviceListView.Invoke(d, new object[] { devices });

            }
            else
            {
                DeviceListView.BeginUpdate();
                DeviceListView.Items.Clear();
                foreach (var device in devices)
                {
                    var Item = new ListViewItem(device.name);
                    Item.SubItems.Add(device.mac);
                    Item.SubItems.Add(device.address);
                    Item.SubItems.Add(device.type);
                    Item.SubItems.Add(device.fw);
                    if (device.fw != ShellyFirmwareAPI.getLatestVersionForModel(device.type))
                    {
                        Item.UseItemStyleForSubItems = false;
                        Item.SubItems[4].ForeColor = Color.Red;
                        Item.ToolTipText = $"New version available: {ShellyFirmwareAPI.getLatestVersionForModel(device.type)}";
                    }
                    DeviceListView.Items.Add(Item);
                }
                DeviceListView.EndUpdate();
            }
        }
        // As this is being called from the mdns service, it may be run in a thread which is separate
        // from the thread where the Form exists. Therefore the form controls need to be accessed in a
        // thread-safe way via delegate function
        private async void MdnsAnswerReceived(object sender, MessageEventArgs e)
        {
            var addresses = e.Message.Answers.OfType<AddressRecord>();

            foreach (var address in addresses)
            {
                if (address.Name.ToString().StartsWith("shelly"))
                {
                    ShellyDevice mydev = Devices.Find(x => x.name == address.Name.ToString());
                    if (mydev is not null)
                    {
                        StatusLabel.Text = $"Device already known: {mydev.name}";
                        mydev.UpdateLastSeen();
                    }
                    else
                    {
                        StatusLabel.Text = $"Discovering device: {address.Name.ToString()} at {address.Address.ToString()}";
                        mydev = await ShellyDevice.Discover(address);
                        StatusLabel.Text = $"Discovered new {mydev.type} device at {mydev.address}";
                        Devices.Add(mydev);
                        RefreshListView(Devices);

                        if (Devices.Count == 1)
                        {
                            UseWaitCursor = false;
                        }

                        DeviceCountLabel.Text = $"{Devices.Count} device" + (Devices.Count > 1 ? "s" : "");
                    }
                }
            }
        }

        private void OTABrowser_Load(object sender, EventArgs e)
        {
            StatusLabel.Text = "Checking latest firmware versions";
            ShellyFirmwareAPI.Init();

            StatusLabel.Text = "Waiting for mDNS announcements...";
            mdns.AnswerReceived += MdnsAnswerReceived;
            mdns.Start();
            mdns.SendQuery("_http._tcp.local");
        }

        // This doesn't seem to properly work, likely because of threads e.g.
        // Exception thrown: 'System.ObjectDisposedException' in System.Net.Sockets.dll
        // Exception thrown: 'System.ObjectDisposedException' in System.Private.CoreLib.dll
         private void OTABrowser_Closing(object sender, EventArgs e)
         {
            sd.Dispose();
            mdns.Stop();
         }

        private void DeviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DeviceListView.SelectedIndices.Count != 0)
            {
                var device = Devices.Find(x => x.mac == DeviceListView.SelectedItems[0].SubItems[1].Text);
                StatusLabel.Text = $"Device {device.address} last seen {device.Age()} seconds ago";
                DetailPanel.Enabled = true;
            }
            else
            {
                StatusLabel.Text = "Selection cleared";
                DetailPanel.Enabled = false;
            }
        }

        private void VisitDeviceLink(ShellyDevice device, string path = "")
        {
            ProcessStartInfo psi = new()
            {
                FileName = $"http://{device.address}{path}",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void WebUILinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitDeviceLink(Devices.Find(x => x.mac == DeviceListView.SelectedItems[0].SubItems[1].Text));
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }

        private void DeviceInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitDeviceLink(Devices.Find(x => x.mac == DeviceListView.SelectedItems[0].SubItems[1].Text), "/shelly");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }

        private void StatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitDeviceLink(Devices.Find(x => x.mac == DeviceListView.SelectedItems[0].SubItems[1].Text), "/status");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }
    }
}
