using Plugins.ToolKits;
using Plugins.ToolKits.IOCKits;
using System;

namespace ConsoleTest
{
    internal partial class Program
    {


        public void IOC()
        {
            Injecter.Register<MyClass1>();

            Console.WriteLine(Injecter.Resolve<MyClass1>().Value);
        }



        public class MyClass1
        {
            public int Value { get; set; } = 100;
        }
    }
}
