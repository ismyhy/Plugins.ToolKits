using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Plugins.ToolKits.Transmission
{
    public static class TransmissionAssist
    {
        public static IReadOnlyList<int> GetAvailablePort(int getCount=1)
        {
            if (getCount < 1)
            {
                throw new ArgumentOutOfRangeException("getCount", "count less than 1 ");
            }

            var targets=new List<int>();
            IPGlobalProperties infos = IPGlobalProperties.GetIPGlobalProperties();
            List<int> existPorts = new List<int>();
            existPorts.AddRange(infos.GetActiveTcpListeners().Select(i => i.Port).ToList());
            existPorts.AddRange(infos.GetActiveUdpListeners().Select(i => i.Port).ToList());
            existPorts.AddRange(infos.GetActiveTcpConnections().Select(i => i.LocalEndPoint.Port).ToList());
            
            for (int port = 1000; port < 65535; port++)
            {
                if (!existPorts.Contains(port))
                {
                    targets.Add(port); 
                    if (targets.Count == getCount)
                    {
                        break;
                    } 
                }
            }

            return targets ;
        }



      
    }
}
