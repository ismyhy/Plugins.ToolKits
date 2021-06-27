using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Input;

using Newtonsoft.Json;
using Plugins.ToolKits;
using Plugins.ToolKits.EasyHttp;
using Plugins.ToolKits.Extensions;
using Plugins.ToolKits.MVVM;

namespace ConsoleTest
{
    public class TestAttribute : Attribute
    {
    }


    internal partial class Program
    {
        public static ICommand TestCommand => RelayCommand.ExclusiveExecute(1,async context =>
        {
            //using ExclusiveContext c = context.BeginExclusive();
          
            await System.Threading.Tasks.Task.Delay(2000);

            Console.WriteLine(DateTime.Now.ToLongTimeString());
        });

        public static string IpAddress = "192.168.3.4";
        [Test]
        private static void RestConfigTest()
        {
            return;

            IRestClient rest = RestConfig.Default()
                .UseBaseUrl("http://localhost:65374")
                .UseEncoding(Encoding.UTF8)
                .UseSerializer(o => JsonConvert.SerializeObject(o))
                .UseDeserializer((s, t) => JsonConvert.DeserializeObject(s, t))
                .UseTimeout(1000000)
                .Build();

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            try
            {
               


                Invoker.For(0,1000, ()=>
                {
                    IRestRequest req = rest.Create()
                      .UseMethod(Method.POST)
                      .UseUrl("weatherforecast/index")
                      //.UseContinueTimeout(10000)
                      //.AddParameter(new int[] { 123, 123 })
                      //.AddFile(Path.Combine(desktop, "123.exe"))
                      //.UseFileResult(true)
                      //.UseFileWriter(fileWriter =>
                      //{
                      //    string targetFile = Path.Combine(desktop, $"{DateTime.Now.Ticks}.txt");
                      //    using FileStream fs = File.Create(targetFile);
                      //    fileWriter.WriteTo(fs);
                      //})
                      .AddParameter("loginName", "123")
                      .AddParameter("password", "123");
                    IRestResponse resp =  req.Execute ();
                    resp.GetResult<int[]>(); 
                    resp.Dispose();
                    Console.WriteLine(resp);
                });


                Console.WriteLine("Complete");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }
}