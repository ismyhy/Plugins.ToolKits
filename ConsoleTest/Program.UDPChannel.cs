using Plugins.ToolKits;
using Plugins.ToolKits.Transmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace ConsoleTest
{

    public class Program1 : UDPChannel
    {
        private readonly PacketSetting setting = new PacketSetting()
        {
            ReportArrived = false,
        };
        public static int serverIndex = 0;
        public Program1(IPEndPoint localPoint) : base(localPoint)
        {

        }
        protected override void Recived(ISession session, byte[] dataBuffer)
        {

            Interlocked.Increment(ref serverIndex);
            //string message = Encoding.UTF8.GetString(dataBuffer);
            //if (serverIndex % 100 == 0)
            //    Console.WriteLine(message + "   " + session.GetHashCode());
            //byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Client {serverIndex++}");
            //session.SendAsync(buffer2, 0, buffer2.Length, null);
        }
    }

    public class Program2 : UDPChannel
    {
        private readonly PacketSetting setting = new PacketSetting()
        {
            ReportArrived = false,
        };
        public static int clientIndex = 0;
        public Program2(IPEndPoint localPoint, IPEndPoint remotePoint) : base(localPoint, remotePoint)
        {

        }
        protected override void Recived(ISession session, byte[] dataBuffer)
        {
            var a = dataBuffer;

            //string message = Encoding.UTF8.GetString(dataBuffer);

            //if (clientIndex % 100 == 0)
            //    Console.WriteLine(message + "   " + session.GetHashCode());

            //byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Server {clientIndex++}");
            //session.SendAsync(buffer2, 0, buffer2.Length, null);
        }
    }
    internal partial class Program
    {

        [Test]
        public void UDPChannelTest()
        {
            int clientIndex = 0;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IReadOnlyList<int> ports = TransmissionAssist.GetAvailablePort(2);

            PacketSetting setting = new PacketSetting()
            {
                ReportArrived = false,
            };
            IPEndPoint serverEndPoint = new IPEndPoint(ip, ports.First());
            Program1 channel = new Program1(serverEndPoint);
            channel.RunAsync();

            Program2 channel2 = new Program2(new IPEndPoint(ip, ports.Last()), serverEndPoint);

            channel2.RunAsync();

            //IUDPChannel channel = TransmissionFactory.UDPCreate()
            //    .UseAsynchronousExecutionCallback(true)
            //  .UseReceiveCallback((session, buffer) =>
            //  {
            //      string message = Encoding.UTF8.GetString(buffer);
            //      if (serverIndex % 100 == 0)
            //          Console.WriteLine(message + "   " + session.GetHashCode());
            //      byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Client {serverIndex++}");
            //      session.SendAsync(buffer2, 0, buffer2.Length, setting);

            //  })
            //  .UseLocalIPEndPoint(ip, serverPoart)
            //  .Build()
            //  .RunAsync();

            //IUDPChannel channel2 = TransmissionFactory.UDPCreate()
            //    .UseAsynchronousExecutionCallback(true)
            //  .UseReceiveCallback((session, buffer) =>
            //  {
            //      string message = Encoding.UTF8.GetString(buffer);

            //      if (clientIndex % 100 == 0)
            //          Console.WriteLine(message + "   " + session.GetHashCode());

            //      byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Server {clientIndex++}");
            //      session.SendAsync(buffer2, 0, buffer2.Length, setting);
            //  })
            //  .UseRemoteIPEndPoint(ip, serverPoart)
            //  .Build()
            //  .RunAsync();
            EasyTimer.Share
                    .UseAutoReset(true)
                    .UseInterval(1000)
                    .UesCallback(() =>
                    {
                        var a = Program1.serverIndex;
                        Console.WriteLine($"clientIndex:{ clientIndex}  serverIndex:{a}   {clientIndex - a}");
                    })
                    .RunAsync();

            var sb = new StringBuilder();
            Invoker.For(0, 200, () =>
            {
                sb.Append("X");
            });

            Invoker.For(0, 1 * int.MaxValue, () =>
         {
             Interlocked.Increment(ref clientIndex);

             byte[] buffer2 = /*BitConverter.GetBytes(clientIndex++); //*/Encoding.UTF8.GetBytes(sb.ToString());

             channel2.Send(buffer2, 0, buffer2.Length, setting);
         });


       

            channel2.Dispose();
            channel.Dispose();    
            Console.ReadKey();

        }
    }
}
