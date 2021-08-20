using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShellyBrowserApp
{
    public partial class ShellyBrowser : Form
    {
        private DeviceInventory inventory;
        private PresentationService presenter;

        internal static List<ShellyDevice> Devices = new();

        public ShellyBrowser()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(onMainFormClosing);

            presenter = new PresentationService(DeviceListView, DetailPanel, StatusStrip);
            inventory = new DeviceInventory(presenter);

            ShellyFirmwareAPI.Init();
        }

        private void onMainFormLoad(object sender, EventArgs e)
        {
            presenter.UpdateStatus("Please wait, devices will appear in the list once detected...");
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
            presenter.SelectedDeviceChanged(DeviceListView.SelectedItems, inventory);
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
            //
        }

        private void DeviceCountLabel_Click(object sender, EventArgs e)
        {
            //
        }
    }
}
