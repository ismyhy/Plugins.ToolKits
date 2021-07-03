
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace Plugins.ToolKits.Transmission.Protocol
{ 
    [DebuggerDisplay("PacketLength:{PacketLength} DataLength:{Data?.Length ?? 0}")]
    internal sealed class ProtocolPacket : IDisposable
    {
        private const int IntSize=sizeof(int);
        private const int PacketLengthIndex = 0;
        private const int PacketCounterIndex = IntSize;
        private const int PacketIsCompressIndex = IntSize * 2;
        private const int PacketReportArrivedIndex = IntSize * 2 + 1;
        private const int PacketDataIndex = IntSize * 2 + 2;
        private const int TotalHeaderLength = PacketDataIndex;

        private static int CommandCounter = 0; 
        internal bool UsingRemoteEndPoint = false;

        public ProtocolPacket()
        {
            Counter = Interlocked.Increment(ref CommandCounter);
        }

        internal bool IsCompress;
        internal int Counter;
        internal bool ReportArrived = true;
        internal int PacketLength;
        internal byte[] Data;
        internal int Offset;
        internal int DataLength;
        public void Dispose()
        {
            IsCompress = false;
            Data = null;
            Offset = 0;
            PacketLength = 0;
            ReportArrived = false;
            Counter = 0;
            DataLength = 0;

            RemoteEndPoint = null;
            SendBuffer = null;
            Semaphore.Dispose();
        }

        public byte[] ToBuffer()
        { 
            PacketLength = TotalHeaderLength + DataLength;

            var buffer = new byte[TotalHeaderLength + DataLength]; 
            Buffer.BlockCopy(BitConverter.GetBytes(PacketLength), 0, buffer, PacketLengthIndex, IntSize); 
            Buffer.BlockCopy(BitConverter.GetBytes(Counter), 0, buffer, PacketCounterIndex, IntSize); 
            buffer[PacketIsCompressIndex] = IsCompress ? (byte)1 : (byte)0; 
            buffer[PacketReportArrivedIndex] = ReportArrived ? (byte)1 : (byte)0; 
            Buffer.BlockCopy(Data, Offset, buffer, PacketDataIndex, DataLength);


            return buffer; 
        }
         

        public static ProtocolPacket FromBuffer(byte[] buffer, int offset, int bufferLength)
        {
            ProtocolPacket protocol = new ProtocolPacket();

            protocol.PacketLength = BitConverter.ToInt32(buffer,PacketLengthIndex + offset);
            protocol.Counter = BitConverter.ToInt32(buffer, PacketCounterIndex + offset);
            protocol.IsCompress = buffer[PacketIsCompressIndex+offset] == 1;
            protocol.ReportArrived = buffer[PacketReportArrivedIndex + offset] == 1;
            if (bufferLength == TotalHeaderLength)
            { 
                return protocol;
            } 

            if (protocol.IsCompress)
            {
                protocol.Data = Decompress(buffer, PacketDataIndex + offset, protocol.PacketLength - TotalHeaderLength);
                protocol.Offset = 0;
                protocol.DataLength = protocol.Data.Length;

                return protocol;
            }
            protocol.DataLength = protocol.PacketLength - TotalHeaderLength;
            var buffer2=new byte[protocol.DataLength];
            Buffer.BlockCopy(buffer, PacketDataIndex + offset, buffer2, 0, buffer2.Length);
            protocol.Data = buffer2;
            protocol.Offset = 0;
            
            return protocol;
        }

        internal static ProtocolPacket BuildPacket(byte[] buffer, int offset, int length, PacketSetting setting)
        { 
            return new ProtocolPacket
            {
                Data = buffer,
                Offset = offset,
                DataLength = length,
                IsCompress = setting?.IsCompressBuffer ?? false,
                ReportArrived = setting?.ReportArrived ?? false,
            };
        }

        internal static ICollection<ProtocolPacket> FromBuffers(byte[] buffer, int offset, int bufferLength)
        {
            List<ProtocolPacket> list = new List<ProtocolPacket>();
            do
            {
                int packetLength = BitConverter.ToInt32(buffer, offset);

                if (packetLength == 0 || packetLength > buffer.Length - offset)
                {
                    break;
                }

                int currentLength = offset + packetLength;

                if (currentLength <= bufferLength)
                {
                    ProtocolPacket pp = FromBuffer(buffer, offset, buffer.Length);
                    list.Add(pp);

                    offset = currentLength;
                    continue;
                }

                break;
            } while (buffer.Length > offset + sizeof(int));

            return list;
        }


        #region   Compress  &  Decompress

        internal static byte[] Compress(byte[] bytes, int offset, int length)
        {
            using MemoryStream compressStream = new MemoryStream();

            using (GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, offset, length);
            }

            return compressStream.ToArray();
        }


        internal static byte[] Decompress(byte[] bytes, int offset, int length)
        {
            using MemoryStream compressStream = new MemoryStream(bytes, offset, length);

            using GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Decompress);

            using MemoryStream resultStream = new MemoryStream();

            zipStream.CopyTo(resultStream);

            return resultStream.ToArray();
        }

        #endregion



        #region Sender

        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(0, 1);

        public IPEndPoint RemoteEndPoint { get; set; }

        public byte[] SendBuffer { get; set; }

        public int Wait(int millisecondsTimeout = -1)
        {
            Semaphore.Wait(millisecondsTimeout);
            return SendCount;
        }

        public void Set()
        {
            Semaphore.Release(1);
        }
         
        public int SendCount { get; set; }

        #endregion
    }


    //internal  sealed class SyncProtocol:IDisposable
    //{
       
    //    public SyncProtocol(ProtocolPacket protocol, IPEndPoint remoteEndPoint)
    //    {
    //        Buffer = protocol.ToBuffer();
    //        ReportArrived = protocol.ReportArrived;
    //        RemoteEndPoint = remoteEndPoint;
    //        Counter = protocol.Counter;
    //    }


    //    private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(0, 1);

    //    public IPEndPoint RemoteEndPoint { get; }
    //    public byte[] Buffer { get; set; }

    //    public int Wait(int millisecondsTimeout=-1)
    //    {
    //        Semaphore.Wait(millisecondsTimeout);
    //        return SendCount;
    //    }

    //    public void Set()
    //    {
    //        Semaphore.Release(1);
    //    }

    //    public void Dispose()
    //    {
    //        Buffer = null;
    //        Semaphore.Dispose(); 
    //    }
    //    public int SendCount { get; set; }




    //    public int Counter { get; set; }
 
    //    public bool ReportArrived { get; }
    //}

}
