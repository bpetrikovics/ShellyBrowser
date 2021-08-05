using System.Collections.Generic;
using System.Linq;

namespace Shelly_OTA_Win
{
    class DeviceInventory
    {
        private PresentationService presenter;
        private StatusService status_service;

        private List<ShellyDevice> devices;

        public DeviceInventory(PresentationService pservice, StatusService sservice)
        {
            presenter = pservice;
            status_service = sservice;
            devices = new();
        }

        public void AddDevice(ShellyDevice device)
        {
            devices.Add(device);
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
    }
}
