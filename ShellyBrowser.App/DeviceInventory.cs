using System.Collections.Generic;
using System.Linq;

using Makaretu.Dns;

namespace ShellyBrowserApp
{
    class DeviceInventory
    {
        private static readonly MulticastService mdns = new();
        private static readonly ServiceDiscovery sd = new(mdns);

        private PresentationService presenter;

        private List<ShellyDevice> devices;

        private System.Threading.Timer AgeCheckTimer;

        // to be moved to app.config property later
        public static readonly int maxDeviceAge = 30;

        public DeviceInventory(PresentationService pservice)
        {
            presenter = pservice;
            devices = new();

            AgeCheckTimer = new System.Threading.Timer(new System.Threading.TimerCallback(DeviceAgeCheck), null, 1250, 500);

            mdns.AnswerReceived += onMdnsAnswerReceived;
            mdns.Start();
            mdns.SendQuery("_http._tcp.local");
        }

        private void DeviceAgeCheck(object state)
        {
            var state_changed = false;

            foreach (var device in All())
            {
                if ((device.Age() >= maxDeviceAge) && (device.stale is false))
                {
                    // Transition available -> stale
                    device.stale = true;
                    state_changed = true;
                }
                else if ((device.Age() < maxDeviceAge) && (device.stale is true))
                {
                    presenter.UpdateStatus($"Device {device.name} no longer stale");
                    device.stale = false;
                    state_changed = true;
                }
            }

            if (state_changed)
            {
                presenter.RefreshListView(All());
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
                    ShellyDevice mydev = FindByName(address.Name.ToString());
                    if (mydev is not null)
                    {
                        presenter.UpdateStatus($"Received update for device: {mydev.name}");
                        mydev.UpdateLastSeen();
                        mydev.UpdateOtaStatus();
                    }
                    else
                    {
                        presenter.UpdateStatus($"Discovering device: {address.Name} at {address.Address}");
                        mydev = await ShellyDevice.Discover(address);

                        presenter.UpdateStatus($"Discovered new {mydev.type} device at {mydev.address}");
                        AddDevice(mydev);
                        presenter.RefreshListView(All());

                        presenter.UpdateDeviceCount(Count);
                    }
                }
            }
        }

        public void AddDevice(ShellyDevice device)
        {
            devices.Add(device);
        }

        public void DeleteDevice(ShellyDevice device)
        {
            devices.Remove(device);
        }

        public List<ShellyDevice> All()
        {
            return devices;
        }

        public ShellyDevice FindByName(string name)
        {
            return devices.Find(x => x.name == name);
        }

        public ShellyDevice FindByMac(string mac)
        {
            return devices.Find(x => x.mac == mac);
        }

        public int Count
        {
            get
            {
                return devices.Count;
            }
        }

        public void Dispose()
        {
            sd.Dispose();
            mdns.Stop();
        }

    }
}
