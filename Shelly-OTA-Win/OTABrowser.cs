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
        private DeviceInventory inventory;
        private PresentationService presenter;
        private StatusService status;

        internal static List<ShellyDevice> Devices = new();

        public OTABrowser()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(onMainFormClosing);
            status = new StatusService(StatusStrip);
            presenter = new PresentationService(DeviceListView);
            inventory = new DeviceInventory(presenter, status);
            ShellyFirmwareAPI.Init();
        }

        private void onMainFormLoad(object sender, EventArgs e)
        {
            status.Update("Please wait, devices will appear in the list once detected...");
        }

        // This doesn't seem to properly work, likely because of threads e.g.
        // Exception thrown: 'System.ObjectDisposedException' in System.Net.Sockets.dll
        // Exception thrown: 'System.ObjectDisposedException' in System.Private.CoreLib.dll
        private void onMainFormClosing(object sender, EventArgs e)
        {
            inventory.Dispose();
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
