using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// https://docs.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-5.0

namespace ShellyBrowserApp
{
    // Singleton class; background service that provides the OTA proxy webserver
    public sealed class OtaService
    {
        private static OtaService _instance = null;
        private static readonly object _lock = new();

        private HttpListener listener = null;
        private bool started = false;

        OtaService()
        {
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

        public void Init()
        {
            listener = new();
        }

        public void Start(string bindAddress, string bindPort)
        {
            if (started)
            {
                Stop();
            }

            try
            {
                listener.Prefixes.Clear();
                listener.Prefixes.Add($"http://{bindAddress}:{bindPort}/ota/");
                listener.Start();
                started = true;
            }
            catch (Exception exc) {
                MessageBox.Show($"Exception: {exc}");
            }
        }

        public void Stop()
        {
            if (started)
            {
                listener.Stop();
                started = false;
            }
        }
    }
}
