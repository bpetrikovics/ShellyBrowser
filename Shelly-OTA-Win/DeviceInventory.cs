using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shelly_OTA_Win
{
    class DeviceInventory
    {
        private static List<ShellyDevice> devices;

        public static void Init()
        {
            devices = new();
        }

        public static void AddDevice(ShellyDevice device)
        {
            devices.Add(device);
        }

        public static List<ShellyDevice> All()
        {
            return devices;
        }

        public static ShellyDevice? FindByName(string name)
        {
            return devices.Find(x => x.name == name);
        }

        public static ShellyDevice? FindByMac(string mac)
        {
            return devices.Find(x => x.mac == mac);
        }

        public static int Count()
        {
            return devices.Count();
        }
    }
}
