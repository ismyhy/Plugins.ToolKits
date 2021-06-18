using Plugins.ToolKits;
using Plugins.ToolKits.Extensions;
using Plugins.ToolKits.MVVM;
using Plugins.ToolKits.Transmission;
using Plugins.ToolKits.Validatement;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConsoleTest
{
    internal partial class Program
    {
        public class Test111 : IResettable
        {
            public void Reset()
            {
                
            }
        }


        private static void Main(string[] args)
        {

          

            int serverIndex = 0;
            int clientIndex = 0;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            var serverPoart = TransmissionAssist.GetAvailablePort();

            var setting= new PacketSetting()
            {
                ReportArrived = true,
            };

            IUDPChannel channel = UDPChannel.Create()
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

            IUDPChannel channel2 = UDPChannel.Create().UseAsynchronousExecutionCallback(true)
              .UseReceiveCallback((session, buffer) =>
              {
                  string message = Encoding.UTF8.GetString(buffer);

                  if (clientIndex % 100 == 0)
                      Console.WriteLine(message + "   "+ session.GetHashCode());
                  //Thread.Sleep(100);
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


            List<KeyValuePair<MethodInfo, TestAttribute>> actions = typeof(Program).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(i => i, i => i.GetAttribute<TestAttribute>())
                .Where(i => i.Value != null)
                .Where(i => i.Key.GetParameters().Length == 0)
                .ToList();

            Program p = new Program();

            foreach (KeyValuePair<MethodInfo, TestAttribute> action in actions)
            {
                action.Key.Invoke(p, null);
            }




        }



        public class LoginViewModel : ValidatableViewModelBase
        {

            public string LoginName
            {
                get => GetValue<string>();
                set => SetValue(value);
            }

            public string Password
            {
                get => GetValue<string>();
                set => SetValue(value);
            }


            public ICommand TestCommand => RelayCommand.Execute(async () =>
           {
               await Task.Delay(01000);
           });

            public ICommand TestCommand2 => RelayCommand.Execute(async () =>
            {
                await Task.Delay(01000);
            }, () => true);

            protected override void ValidateItems(IValidate validator)
            {
            }

            public void Run()
            {
                Invoker.For(0, 100, i =>
                {
                    Console.WriteLine(TestCommand.GetHashCode());
                });



            }
        }


    }
}
