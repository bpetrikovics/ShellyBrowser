using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Makaretu.Dns;

// https://markheath.net/post/maintainable-winforms

namespace Shelly_OTA_Win
{
    public partial class OTABrowser : Form
    {
        private delegate void SafeRefreshListViewDelegate(List<ShellyDevice> devices);

        public static MulticastService mdns = new();
        public static ServiceDiscovery sd = new(mdns);

        // This needs to be a class variable otherwise if just defined as a local variable, the garbage collector
        // will collect it due to lack of reference
        public static System.Threading.Timer AgeCheckTimer;

        internal static List<ShellyDevice> Devices = new();

        // to be moved to app.config property later
        public static readonly int maxDeviceAge = 30;

        public OTABrowser()
        {
            FormClosing += new FormClosingEventHandler(OTABrowser_Closing);
            InitializeComponent();
        }

        private void OTABrowser_Load(object sender, EventArgs e)
        {
            SafeStatusUpdate("Checking latest firmware versions");
            ShellyFirmwareAPI.Init();
            DeviceInventory.Init();

            AgeCheckTimer = new System.Threading.Timer(new System.Threading.TimerCallback(DeviceAgeCheck), null, 1250, 500);

            SafeStatusUpdate("Please wait, devices will appear in the list once detected...");
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

        private void DeviceAgeCheck(object state)
        {
            var state_changed = false;

            foreach (var device in DeviceInventory.All())
            {
                if ((device.Age() >= maxDeviceAge) && (device.stale is false))
                {
                    // Transition available -> stale
                    device.stale = true;
                    state_changed = true;
                }
                else if ((device.Age() < maxDeviceAge) && (device.stale is true))
                {
                    SafeStatusUpdate($"Device {device.name} no longer stale");
                    device.stale = false;
                    state_changed = true;
                }
            }

            if(state_changed)
            {
                RefreshListView(DeviceInventory.All());
            }
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

                    DeviceListView.Items.Add(Item);
                }
                DeviceListView.EndUpdate();
            }
        }

        // The StatusLabel is not a Control so it does not have its InvokeRequired/Invoke methods; this needs to be done
        // through its parent.
        public void SafeStatusUpdate(string text)
        {
            if (StatusStrip.InvokeRequired)
            {
                StatusStrip.Invoke(new Action(() => StatusStrip.Items.Find("StatusLabel", false).First().Text = text + " (invoked)"));
            }
            else
            {
                StatusStrip.Items.Find("StatusLabel", false).First().Text = text;
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
                    ShellyDevice mydev = DeviceInventory.FindByName(address.Name.ToString());
                    if (mydev is not null)
                    {
                        SafeStatusUpdate($"Received update for device: {mydev.name}");
                        mydev.UpdateLastSeen();
                    }
                    else
                    {
                        SafeStatusUpdate($"Discovering device: {address.Name} at {address.Address}");
                        mydev = await ShellyDevice.Discover(address);

                        SafeStatusUpdate($"Discovered new {mydev.type} device at {mydev.address}");
                        DeviceInventory.AddDevice(mydev);
                        RefreshListView(DeviceInventory.All());

                        if (Devices.Count == 1)
                        {
                            UseWaitCursor = false;
                        }

                        // TODO: Make this thread-safe as well
                        DeviceCountLabel.Text = $"{DeviceInventory.Count()} device" + (DeviceInventory.Count() > 1 ? "s" : "");
                    }
                }
            }
        }

        private void DeviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DeviceListView.SelectedIndices.Count != 0)
            {
                var device = DeviceInventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text);
                SafeStatusUpdate($"Device {device.address} last seen {device.Age()} seconds ago, stale: {device.stale}");
                DetailPanel.Enabled = true;
            }
            else
            {
                SafeStatusUpdate("Selection cleared");
                DetailPanel.Enabled = false;
            }
        }

        // simply Process.Start() an URI does not work anymore due to breaking API changes
        private void VisitDeviceLink(ShellyDevice device, string path = "")
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

        private void WebUILinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitDeviceLink(DeviceInventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text));
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
                VisitDeviceLink(DeviceInventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/shelly");
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
                VisitDeviceLink(DeviceInventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/status");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }
    }
}
