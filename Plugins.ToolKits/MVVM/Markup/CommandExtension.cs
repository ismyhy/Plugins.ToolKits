namespace Plugins.ToolKits.MVVM.Markup
{
    // [MarkupExtensionReturnType(typeof(ICommand))]
    public sealed class CommandExtension //: MarkupExtension, ICommand
    {
        //private const string DataContext = "DataContext";
        //private const string BindingContext = "BindingContext";

        //private Tuple<MethodInfo, bool, bool> bindingMethodInfo;
        //private Tuple<MethodInfo, bool, bool> bindingCanExecuteMethodInfo;
        //private object contextObject;
        //private PropertyInfo contextPropertyInfo;
        //private object provideObject;

        //public CommandExtension(string methodName, string canExecuteMethodName)
        //{
        //    MethodName = methodName;
        //    CanExecuteMethodName = canExecuteMethodName;
        //}

        //public CommandExtension(string methodName)
        //{
        //    MethodName = methodName;
        //}

        //public CommandExtension()
        //{
        //}

        //public string CanExecuteMethodName { get; set; }

        //public string MethodName { get; set; }

        //public event EventHandler CanExecuteChanged;

        //public bool CanExecute(object parameter)
        //{
        //    if (string.IsNullOrWhiteSpace(CanExecuteMethodName))
        //    {
        //        return true;
        //    }

        //    if (bindingCanExecuteMethodInfo is null)
        //    {
        //        if (provideObject is null)
        //        {
        //            return true;
        //        }

        //        contextObject = contextPropertyInfo.GetValue(provideObject);
        //        if (contextObject is null)
        //        {
        //            return true;
        //        }

        //        var f = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        //        var method = contextObject.GetType().GetMethod(CanExecuteMethodName, f);


        //        if (method == null)
        //        {
        //            var contextType = contextObject.GetType();
        //            throw new NullReferenceException(
        //                $"Not found the method:{MethodName} in {contextType} type.");
        //        }

        //        var parameters = method.GetParameters();
        //        if (parameters.Length > 0)
        //        {
        //            throw new InvalidOperationException(
        //                $"The method:{MethodName} must only have 0  parameters.");
        //        }

        //        var returnType = method.ReturnType;
        //        if (returnType != typeof(bool))
        //        {
        //            throw new InvalidOperationException(
        //                $"The method:{MethodName} return Type must only  Boolean");
        //        }

        //        bindingCanExecuteMethodInfo = Tuple.Create(method, false, false);
        //    }

        //    var result = bindingCanExecuteMethodInfo.Item1.Invoke(contextObject, null);

        //    return (bool)result;
        //}

        //public async void Execute(object parameter)
        //{
        //    if (bindingMethodInfo is null)
        //    {
        //        if (provideObject is null)
        //        {
        //            return;
        //        }

        //        contextObject = contextPropertyInfo.GetValue(provideObject);
        //        if (contextObject is null)
        //        {
        //            return;
        //        }

        //        var f = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        //        var method = contextObject.GetType().GetMethod(MethodName, f);


        //        if (method == null)
        //        {
        //            var contextType = contextObject.GetType();
        //            throw new NullReferenceException(
        //                $"Not found the method:{MethodName} in {contextType} type.");
        //        }

        //        var parameters = method.GetParameters();
        //        if (parameters.Length > 1)
        //        {
        //            throw new InvalidOperationException(
        //                $"The method:{MethodName} must only have 0 or 1 parameters.");
        //        }

        //        if (method.IsGenericMethod)
        //        {
        //            method = method.MakeGenericMethod(parameter.GetType());
        //        }

        //        var hasParameter = parameters.Length > 0;
        //        var isTask = typeof(Task).IsAssignableFrom(method.ReturnType);

        //        bindingMethodInfo = Tuple.Create(method, hasParameter, isTask);
        //    }

        //    var result =
        //        bindingMethodInfo.Item1.Invoke(contextObject, bindingMethodInfo.Item2 ? new[] { parameter } : null);
        //    if (bindingMethodInfo.Item3)
        //    {
        //        await (Task)result;
        //    }
        //}

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    var provide = serviceProvider.GetService(typeof(IProvideValueTarget));
        //    if (!(provide is IProvideValueTarget provideValueTarget))
        //    {
        //        throw new ArgumentException(
        //            $"The {nameof(serviceProvider)} must implement {nameof(IProvideValueTarget)} interface.");
        //    }

        //    provideObject = provideValueTarget.TargetObject;

        //    contextPropertyInfo = provideObject.GetType().GetProperties()
        //        .FirstOrDefault(i => i.Name == DataContext || i.Name == BindingContext);

        //    return this;
        //}
    }
}