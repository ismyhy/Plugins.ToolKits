# Plugins.ToolKits

- EasyEvent
```csharp
public async void EasyEventTest()
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

- IOC Injecter
```csharp

public void ContainerTest()
{
    Injecter.Register<TestViewModel>().As<TestViewModel>();

    var model=Injecter.Resolve<TestViewModel>();

    model.TestProperty="new string value";
}

```

- Restful
```csharp

public async Task RestConfigTest()
{
     var host = RestConfig.Default()
          .UseBaseUrl(() => "http://localhost:15594/")
          .UseDeserialize((stringBuffer, targetType) => JsonConvert.DeserializeObject(stringBuffer, targetType))
          .UseSerializer(targetObject => JsonConvert.SerializeObject(targetObject))
          .UseTimeout(() => 5000)
          .UseEncoding(() => Encoding.UTF8)
          .Build();

     var deskTop=Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
     var request = host.CreateRequest()
          .UseMethod(() => Method.POST)
          .UseUserAgent(() => "Plugins.ToolKits.Restfull")
          .UseUrl(() => "WeatherForecast/index")
          .UseTimeout(() => 1000)
          .AddRequestBody("value", 123)
          .AddFile(Path.Combine(deskTop, "123.txt"))
          .UseFileWriter(stream =>
          {
               var targetFile = Path.Combine(deskTop,$"{DateTime.Now.Ticks}.txt");
               using var fs = File.Create(targetFile);
               stream.CopyTo(fs);
          });

     var resp = await request.ExecuteAsync();
     Console.WriteLine(resp);
     var result = await resp.GetResultAsync<int[]>();
     Console.WriteLine(result.Length); 

     Console.ReadKey();
}



```

