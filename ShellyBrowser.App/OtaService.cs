using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-5.0

namespace ShellyBrowserApp
{
    // Singleton class; background service that provides the OTA proxy webserver
    public sealed class OtaService
    {
        private static readonly string UrlPrefix = "ota";
        
        private static OtaService _instance = null;
        private static readonly object _lock = new();

        private static Dictionary<string, byte[]> Firmwares = new();

        private HttpListener listener = null;
        private bool started = false;
        private string addr = null;
        private string port = null;

        OtaService()
        {
            // Intentionally left blank :)
        }

        public static OtaService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new OtaService();
                    }

                    return _instance;
                }
            }
        }

        public void Start(string bindAddress, string bindPort)
        {
            if (started)
            {
                Stop();
            }

            addr = bindAddress;
            port = bindPort;

            try
            {
                listener = new();
                listener.Prefixes.Clear();
                Netsh.AddUrlAcl($"http://{addr}:{port}/{UrlPrefix}/");
                listener.Prefixes.Add($"http://{addr}:{port}/{UrlPrefix}/");
                listener.Start();
                started = true;

                // do something to handle requests here,
                // start handler thread etc
            }
            catch (HttpListenerException exc) {
                // In case of an exception, the listener seems to be already disposed, so subsequent start() calls will fail!
                MessageBox.Show($"Exception while trying to bind to {addr}:{port}:\n{exc}");
                Stop();
            }
        }

        public void Stop()
        {
            // In case the UrlAcl was added but then te listener threw an exception, we need to
            // be able to remove the previously added Acl; since at stop time we explicitly set the
            // address/port to null, we can can use that to check if we have something to remove
            if (addr is not null)
            {
                Netsh.DeleteUrlAcl($"http://{addr}:{port}/{UrlPrefix}/");
            }

            if (started)
            {
                listener.Stop();
                listener.Close();
                started = false;
                addr = null;
                port = null;
            }
        }

        public async Task PreloadAsync(ShellyFirmwareVersion firmware)
        {
            if (!Firmwares.ContainsKey(firmware.deviceModel))
            {
                // Download the firmware file specified in the firmware object and prepare it in the
                // OTA server's file cache
                HttpClient client = new();
                var response = await client.GetByteArrayAsync(firmware.downloadUrl);

                // Since the http service runs in a separate thread, protect the firmware dictionary with locking
                lock (_lock)
                {
                    Firmwares.Add(firmware.deviceModel, response);
                }
            }
        }

        public static string GetDownloadAddress(string bindAddress, string bindPort, string deviceModel)
        {
            return $"http://{bindAddress}:{bindPort}/{UrlPrefix}/{deviceModel}";
        }
    }
}
