using Plugins.ToolKits;
using Plugins.ToolKits.Transmission;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ConsoleTest
{
    internal partial class Program
    { 

        [Test]
        public void UDPChannelTest()
        {


            int serverIndex = 0;
            int clientIndex = 0;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            var serverPoart = TransmissionAssist.GetAvailablePort();

            var setting = new PacketSetting()
            {
                ReportArrived = false,
            };

            IUDPChannel channel = TransmissionFactory.UDPCreate()
                .UseAsynchronousExecutionCallback(true)
              .UseReceiveCallback((session, buffer) =>
              {
                  string message = Encoding.UTF8.GetString(buffer);
                  if (serverIndex % 100 == 0)
                      Console.WriteLine(message + "   " + session.GetHashCode());
                  byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Client {serverIndex++}");
                  session.SendAsync(buffer2, 0, buffer2.Length, setting);

              })
              .UseLocalIPEndPoint(ip, serverPoart)
              .Build()
              .RunAsync();

            IUDPChannel channel2 = TransmissionFactory.UDPCreate()
                .UseAsynchronousExecutionCallback(true)
              .UseReceiveCallback((session, buffer) =>
              {
                  string message = Encoding.UTF8.GetString(buffer);

                  if (clientIndex % 100 == 0)
                      Console.WriteLine(message + "   " + session.GetHashCode());
 
                  byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Server {clientIndex++}");
                  session.SendAsync(buffer2, 0, buffer2.Length, setting);
              })
              .UseRemoteIPEndPoint(ip, serverPoart)
              .Build()
              .RunAsync();

            Invoker.For(0, 1, () =>
            {
                byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Server {clientIndex++}");

                channel2.Send(buffer2, 0, buffer2.Length, setting);
            });

            Console.ReadKey();


            channel2.Dispose();
            channel.Dispose();
        }
    }
}
