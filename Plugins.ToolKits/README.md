# Plugins.ToolKits

- EasyEvent
```csharp
public async void Test()
{
     var token= Events.Register((stringValue, intValue) =>
     {
          Console.WriteLine($"{stringValue} {intValue}");
     });
     Events.Invoke("123",123);

     await Events.InvokeAsync("123", 123);

     token.Dispose();
}
public EasyEvent<string,int> Events { get; }=new EasyEvent<string, int>();
```
- ViewModelBase
```csharp
public class TestViewModel : ViewModelBase
{ 
    public string TestProperty
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    private string _testProperty2;
    public string TestProperty2
    {
        get => _testProperty2;
        set => SetValue(ref _testProperty2,value);
    }
}
```
- RelayCommand
```csharp

public ICommand TestCommand => RelayCommand.Execute(() =>
{
     Console.WriteLine("test Command");
});

public ICommand TestCommand2 => RelayCommand.TryExecute(() =>
{
    Console.WriteLine("test Command2");

},e=>MessageBox.Show(e.Message));

public ICommand TestCommand3 => RelayCommand.ExecuteAsync(() =>
{
    Console.WriteLine("test Command");
});

```

- IOC Container
```csharp

public void ContainerTest()
{
    Injecter.Register<TestViewModel>().As<TestViewModel>();

    var model=Injecter.Resolve<TestViewModel>();

    model.TestProperty="new string value";
}

```

- Restful Test
```csharp

 private static void RestConfigTest()
 { 
     IRestClient rest = RestConfig.Default()
         .UseBaseUrl("http://localhost:65374")
         .UseEncoding(Encoding.UTF8)
         .UseSerializer(o => JsonConvert.SerializeObject(o))
         .UseDeserializer((s, t) => JsonConvert.DeserializeObject(s, t))
         .Build();

     string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

     try
     {
         IRestRequest req = rest.Create()
             .UseMethod(Method.POST)
             .UseUrl("weatherforecast/index")
             .UseContinueTimeout(10000)
             .AddParameter(new int[] { 123, 123 })
             .AddFile(Path.Combine(desktop, "123.exe"))
             //.UseFileResult(true)
             //.UseFileWriter(fileWriter =>
             //{
             //    var targetFile = Path.Combine(desktop, $"{DateTime.Now.Ticks}.txt");
             //    using var fs = File.Create(targetFile);
             //    fileWriter.WriteTo(fs);
             //})
             //.AddParameter("loginName", "123")
             //.AddParameter("password", "123");
         IRestResponse resp = req.Execute();

         resp.GetResult<int[]>();

         Console.WriteLine(resp);
     }
     catch (Exception e)
     {
         Console.WriteLine(e);
     }
     Console.ReadKey();
 }

```
