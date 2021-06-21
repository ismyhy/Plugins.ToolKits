using Plugins.ToolKits;
using Plugins.ToolKits.Transmission;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ConsoleTest
{

    public class Program1 : UDPChannel
    {
        PacketSetting setting = new PacketSetting()
        {
            ReportArrived = false,
        };
        int serverIndex = 0;
        public Program1(IPEndPoint localPoint) : base(localPoint)
        {

        } 
        protected override void Recived(ISession session, byte[] dataBuffer)
        {
            string message = Encoding.UTF8.GetString(dataBuffer);
            if (serverIndex % 100 == 0)
                Console.WriteLine(message + "   " + session.GetHashCode());
            byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Client {serverIndex++}");
            session.SendAsync(buffer2, 0, buffer2.Length, setting);
        } 
    }

    public class Program2 : UDPChannel
    {
        PacketSetting setting = new PacketSetting()
        {
            ReportArrived = false,
        };
        int clientIndex = 0;
        public Program2(IPEndPoint localPoint, IPEndPoint remotePoint) : base(localPoint, remotePoint)
        {

        }
        protected override void Recived(ISession session, byte[] dataBuffer)
        {
            string message = Encoding.UTF8.GetString(dataBuffer);

            if (clientIndex % 100 == 0)
                Console.WriteLine(message + "   " + session.GetHashCode());

            byte[] buffer2 = Encoding.UTF8.GetBytes($"Hello Server {clientIndex++}");
            session.SendAsync(buffer2, 0, buffer2.Length, setting);
        }
    }
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
            var serverEndPoint=new IPEndPoint(ip, serverPoart);
            var channel = new Program1(new IPEndPoint(ip, TransmissionAssist.GetAvailablePort()) );
            channel.RunAsync();

            var channel2 = new Program2(new IPEndPoint(ip, TransmissionAssist.GetAvailablePort()), serverEndPoint);

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
