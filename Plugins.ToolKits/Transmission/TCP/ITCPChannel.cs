using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission.TCP
{


    public interface ITCPChannel
    {
        ITCPChannel RunAsync();

        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);

        void Close();
    }

    //internal class TCPChannel : ITCPChannel
    //{
    //    public readonly ContextContainer Context = new ContextContainer();

    //    public TCPChannel()
    //    {

    //    }

    //    public void Close()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITCPChannel RunAsync()
    //    {
    //        if (!Context.TryGet<IPEndPoint>(TCPConfigKeys.RemoteIPEndPoint, out var remoteIpPoint))
    //        {
    //            throw new Exception("The correct remote port is not configured");
    //        }

    //        if (!Context.TryGet<TcpClient>(TCPConfigKeys.TcpClient, out var tcpClient))
    //        {
    //            throw new Exception("The correct remote port is not configured");
    //        }
    //        tcpClient.Connect(remoteIpPoint);

    //        var buffer = new byte[1024 * 1024 * 32];

    //        NetworkStream stream = tcpClient.GetStream();
    //        stream.BeginRead(buffer, 0, buffer.Length, HandleDataReceived, stream);

    //        return this;

    //        void HandleDataReceived(IAsyncResult ar)
    //        {
    //            NetworkStream stream1 = (NetworkStream)ar.AsyncState;
    //            int receivedLength;
    //            try
    //            {
    //                receivedLength = stream1.EndRead(ar);
    //                _easyBase.ReceivedHandler(_bufferPool, 0, receivedLength);
    //                stream1.BeginRead(buffer, 0, buffer.Length, HandleDataReceived, stream1);
    //            }
    //            catch
    //            {
    //                receivedLength = 0;
    //            }

    //            if (receivedLength == 0)
    //            {
    //                _removeAction?.Invoke(this);
    //            }
    //        }

    //    }

    //    public int Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}



}
