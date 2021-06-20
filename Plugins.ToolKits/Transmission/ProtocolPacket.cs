
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Plugins.ToolKits.Transmission.Protocol
{
    [DebuggerDisplay("PacketLength:{PacketLength} DataLength:{Data?.Length ?? 0}")]
    internal sealed class ProtocolPacket : IDisposable
    {

        private const int TotalHeaderLength = 10;

        private static int CommandCounter = 0;

        internal bool UsingRemoteEndPoint = false;

        public ProtocolPacket()
        {

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
        }

        public byte[] ToBuffer()
        {

            bool hasData = Data != null && DataLength > 0;
            

            PacketLength = TotalHeaderLength + DataLength;

            using MemoryStream stream = new MemoryStream();
            using BinaryWriter write = new BinaryWriter(stream);

            write.Write(PacketLength);
            write.Write(Counter);
            write.Write(IsCompress ? (byte)1 : (byte)0);
            write.Write(ReportArrived ? (byte)1 : (byte)0);

            write.Write(Data, Offset, DataLength);
            return stream.ToArray();
        }

        public void RefreshCounter()
        {
            Counter = Interlocked.Increment(ref CommandCounter);
            if (Counter == int.MaxValue)
            {
                CommandCounter = 0;
            }
        }




        public static ProtocolPacket FromBuffer(byte[] buffer, int offset, int bufferLength)
        {
            ProtocolPacket protocol = new ProtocolPacket();

            using MemoryStream stream = new MemoryStream(buffer, offset, bufferLength);
            using BinaryReader reader = new BinaryReader(stream);

            protocol.PacketLength = reader.ReadInt32();
            protocol.Counter = reader.ReadInt32();
            protocol.IsCompress = reader.ReadByte() == 1;
            protocol.ReportArrived = reader.ReadByte() == 1;

            int effectiveDataLength = protocol.PacketLength - TotalHeaderLength;
            if (effectiveDataLength > 0)
            {
                protocol.Data = reader.ReadBytes(effectiveDataLength);
                if (protocol.IsCompress && protocol.Data.Length > 0)
                {
                    protocol.Data = Decompress(protocol.Data, 0, protocol.Data.Length);
                }
                protocol.Offset = 0;
                protocol.DataLength = protocol.Data.Length;
            }
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
    }
}
