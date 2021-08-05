using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            presenter = new PresentationService(DeviceListView, DetailPanel);
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

        private void WebUILinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DeviceListView.SelectedItems.Count == 0)
            {
                return;
            }

            try
            {
                presenter.VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text));
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }

        private void DeviceInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DeviceListView.SelectedItems.Count == 0)
            {
                return;
            }

            try
            {
                presenter.VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/shelly");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }

        private void StatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DeviceListView.SelectedItems.Count == 0)
            {
                return;
            }

            try
            {
                presenter.VisitDeviceLink(inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text), "/status");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Unable to open link that was clicked: {exc}");
            }
        }

        private void StartUpgradeButton_Click(object sender, EventArgs e)
        {

        }

        private void DeviceCountLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
