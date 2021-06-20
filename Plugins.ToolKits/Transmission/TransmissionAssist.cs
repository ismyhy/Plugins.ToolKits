using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Plugins.ToolKits.Transmission
{
    public static class TransmissionAssist
    {
        public static int GetAvailablePort()
        {
            int availablePort = 0;

            IPGlobalProperties infos = IPGlobalProperties.GetIPGlobalProperties();
            List<int> existPorts = new List<int>();
            existPorts.AddRange(infos.GetActiveTcpListeners().Select(i => i.Port).ToList());
            existPorts.AddRange(infos.GetActiveUdpListeners().Select(i => i.Port).ToList());
            existPorts.AddRange(infos.GetActiveTcpConnections().Select(i => i.LocalEndPoint.Port).ToList());
            for (int i = 1000; i < 65535; i++)
            {
                if (!existPorts.Contains(i))
                {
                    availablePort = i;
                    break;
                }
            }

            return availablePort;
        }



      
    }
}
