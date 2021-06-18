using Plugins.ToolKits.Communication.Host;

using System;
using System.Net;

namespace Plugins.ToolKits.Communication
{
    public interface IConnectConfig
    {
        IConnectConfig UseLocalEndPoint(Func<IPEndPoint> localEndPointFunc);

        IConnectConfig UseRemoteEndPoint(Func<IPEndPoint> remoteEndPointFunc);

        IEasyHost RunAsHostAsync();

        IEasyClient RunAsClientAsync();

        IConnectConfig UseReceiveFunc(Action<IEasySession, byte[]> receivedFunc);

        IConnectConfig UseExceptionFunc(Action<Exception> exceptionFunc);
    }
}
