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
            List<KeyValuePair<MethodInfo, TestAttribute>> actions = typeof(Program).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(i => i, i => i.GetAttribute<TestAttribute>())
                .Where(i => i.Value != null)
                .Where(i => i.Key.GetParameters().Length == 0)
                .ToList();

            Program p = new Program();

            foreach (KeyValuePair<MethodInfo, TestAttribute> action in actions)
            {
                Task.Run(() => action.Key.Invoke(p, null));
            } 


           Console.ReadKey();
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
