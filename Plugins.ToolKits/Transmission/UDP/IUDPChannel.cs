﻿using System;
using System.Net;
using System.Threading.Tasks;
namespace Plugins.ToolKits.Transmission
{ 
    public interface IUDPChannel : IDisposable
    {
        IUDPChannel RunAsync();

        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);

        void Close();
    }
}
