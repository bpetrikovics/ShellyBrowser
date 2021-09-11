using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellyBrowserApp
{
    public class Netsh
    {
        private static bool Command(string cmd)
        {
            var netsh = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "netsh.exe");
            var startInfo = new ProcessStartInfo(netsh);

            startInfo.Arguments = cmd;
            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";

            try
            {
                var process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Win32Exception)
            {
                return false;
            }

            return true;
        }

        public static bool AddUrlAcl(string url)
        {
            return Command($"http add urlacl url={url} user=Users listen=yes");
        }

        public static bool DeleteUrlAcl(string url)
        {
            return Command($"http delete urlacl url={url}");
        }
    }
}
