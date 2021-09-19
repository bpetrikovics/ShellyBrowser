using System;
using System.Windows.Forms;

namespace ShellyBrowserApp
{
    public partial class ShellyBrowser : Form
    {
        private InventoryService inventory;
        private PresentationService presenter;

        public ShellyBrowser()
        {
            InitializeComponent();

            FormClosing += new FormClosingEventHandler(onMainFormClosing);

            ShellyFirmwareService.Init(); // need to load firmware data before we'd start receiving device announcements

            presenter = new PresentationService(DeviceListView, DetailPanel, StatusStrip);
            inventory = new InventoryService(presenter);
        }

        private void onMainFormLoad(object sender, EventArgs e)
        {

            if (Utils.IsAdministrator())
            {
                Text += " (Administrator)";
            }
            else
            {
                presenter.NotifyMessage("Application is not running as admin. The OTA proxy feature either needs admin rights or will display an UAC popup every time it's activating or deactivating.");
            }

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

        private async void StartUpgradeButton_Click(object sender, EventArgs e)
        {
            ShellyDevice device = inventory.FindByMac(DeviceListView.SelectedItems[0].SubItems[1].Text);

            if (presenter.isOtaSelected)
            {
                var firmware = ShellyFirmwareService.getLatestFirmware(device);
                await OtaService.Instance.PreloadAsync(firmware);
                presenter.UpdateStatus($"Firmware preloaded for {device.type}");
                // OtaService.Instance.Start(presenter.otaBindAddress, presenter.otaBindPort);
                // await device.StartUpdate(OtaService.Instance.GetDownloadAddress(device.type));
                // OtaService.Instance.Stop(); // ??
                // presenter.UpdateStatus($"Firmware upgrade requested on device {device.name} via OTA proxy");
            }
            else
            {
                await device.StartUpdate("");
                presenter.UpdateStatus($"Firmware upgrade requested on device {device.name}");
                // This forces re-discovery of the device, which will re-read the firmware details,
                // so in case of a successful upgrade it will come up with the new version.
                // Later on we might rather just invalidate the device or schedule an update thread
                inventory.DeleteDevice(device);
                presenter.RefreshListView(inventory.All());
            }
        }

        private void UpdateProxyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (presenter.isOtaSelected)
            {
                // OtaService.Instance.Start(presenter.otaBindAddress, presenter.otaBindPort);
                presenter.UpdateStatus("Proxy enabled");
            }
            else
            {
                // OtaService.Instance.Stop();
                presenter.UpdateStatus("Proxy disabled");
            }
        }

        private void OtaBindIPSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // OTA service bind IP changed, if it is running, must be stopped/restarted
        }

        private void OtaPortTextBox_TextChanged(object sender, EventArgs e)
        {
            // OTA service bind port changed, if it is running, must be stopped/restarted
        }
    }
}
