using System;
using System.Linq;
using System.Windows.Forms;

namespace Shelly_OTA_Win
{
    class StatusService
    {
        private readonly StatusStrip statusbar;

        public StatusService(StatusStrip target)
        {
            statusbar = target;
        }

        // Thread safe update of the status bar, checking if we're on the UI thread or Invoke is required
        public void Update(string text)
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
