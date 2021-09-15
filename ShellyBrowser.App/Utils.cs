using System.Security.Principal;

namespace ShellyBrowserApp
{
    public static class Utils
    {
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
             .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
