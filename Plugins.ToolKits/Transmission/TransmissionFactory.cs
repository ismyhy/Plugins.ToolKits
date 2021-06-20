using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission
{
   public sealed class TransmissionFactory
    {

        public static IUDPConfig UDPCreate()
        {
           return new  Plugins.ToolKits.Transmission.UDP.UDPConfig();
        }
    }
}
