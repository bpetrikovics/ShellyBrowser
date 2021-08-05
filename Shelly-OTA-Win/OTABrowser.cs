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
        private static readonly MulticastService mdns = new();
        private static readonly ServiceDiscovery sd = new(mdns);

        private DeviceInventory inventory;
        private PresentationService presenter;
        private StatusService status;

        // This needs to be a class variable otherwise if just defined as a local variable, the garbage collector
        // will collect it due to lack of reference
        private static System.Threading.Timer AgeCheckTimer;

        internal static List<ShellyDevice> Devices = new();

        // to be moved to app.config property later
        public static readonly int maxDeviceAge = 30;

        public OTABrowser()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(onMainFormClosing);
            status = new StatusService(StatusStrip);
            presenter = new PresentationService(DeviceListView);
            inventory = new DeviceInventory(presenter, status);
        }

        private void onMainFormLoad(object sender, EventArgs e)
        {
            status.Update("Checking latest firmware versions");
            ShellyFirmwareAPI.Init();

            AgeCheckTimer = new System.Threading.Timer(new System.Threading.TimerCallback(DeviceAgeCheck), null, 1250, 500);

            status.Update("Please wait, devices will appear in the list once detected...");
            mdns.AnswerReceived += onMdnsAnswerReceived;
            mdns.Start();
            mdns.SendQuery("_http._tcp.local");
        }

        // This doesn't seem to properly work, likely because of threads e.g.
        // Exception thrown: 'System.ObjectDisposedException' in System.Net.Sockets.dll
        // Exception thrown: 'System.ObjectDisposedException' in System.Private.CoreLib.dll
        private void onMainFormClosing(object sender, EventArgs e)
        {
            sd.Dispose();
            mdns.Stop();
        }

        private void DeviceAgeCheck(object state)
        {
            var state_changed = false;

            foreach (var device in inventory.All())
            {
                if ((device.Age() >= maxDeviceAge) && (device.stale is false))
                {
                    // Transition available -> stale
                    device.stale = true;
                    state_changed = true;
                }
                else if ((device.Age() < maxDeviceAge) && (device.stale is true))
                {
                    status.Update($"Device {device.name} no longer stale");
                    device.stale = false;
                    state_changed = true;
                }
            }

            if(state_changed)
            {
                presenter.RefreshListView(inventory.All());
            }
        }

        // As this is being called from the mdns service, it may be run in a thread which is separate
        // from the thread where the Form exists. Therefore the form controls need to be accessed in a
        // thread-safe way via delegate function
        private async void onMdnsAnswerReceived(object sender, MessageEventArgs e)
        {
            var addresses = e.Message.Answers.OfType<AddressRecord>();

            foreach (var address in addresses)
            {
                if (address.Name.ToString().StartsWith("shelly"))
                {
                    ShellyDevice mydev = inventory.FindByName(address.Name.ToString());
                    if (mydev is not null)
                    {
                        status.Update($"Received update for device: {mydev.name}");
                        mydev.UpdateLastSeen();
                    }
                    else
                    {
                        status.Update($"Discovering device: {address.Name} at {address.Address}");
                        mydev = await ShellyDevice.Discover(address);

                        status.Update($"Discovered new {mydev.type} device at {mydev.address}");
                        inventory.AddDevice(mydev);
                        presenter.RefreshListView(inventory.All());

                        status.UpdateDeviceCount(inventory.Count);
                    }
                }
            }
        }

        private void DeviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DeviceListView.SelectedIndices.Count != 0)
            {
                var device = inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text);
                status.Update($"Device {device.address} last seen {device.Age()} seconds ago");
                DetailPanel.Enabled = true;
            }
            else
            {
                status.Update("Selection cleared");
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
                VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text));
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
                VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/shelly");
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
                VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/status");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }
    }
}
