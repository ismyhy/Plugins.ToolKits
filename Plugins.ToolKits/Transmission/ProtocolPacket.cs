
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Plugins.ToolKits.Transmission
{
    [DebuggerDisplay("PacketLength:{PacketLength} DataLength:{Data?.Length ?? 0}")]
    internal sealed class ProtocolPacket : IDisposable
    {

        private const int TotalHeaderLength = 11;

        private static int CommandCounter = 0;

        internal bool UsingRemoteEndPoint = false;


        public ProtocolPacket()
        {

        }

        //internal byte[] Ip;
        //internal int Port;
        internal bool IsCompress;
        internal int Counter;
        internal bool JoinMulticastGroup = false;
        internal bool ReportArrived = true;
        internal int PacketLength;
        internal byte[] Data;
        internal int Offset;
        internal int DataLength;
        public void Dispose()
        {
        }

        public ProtocolPacket CopyTo(ProtocolPacket packet)
        {
            packet.Counter = Counter;
            packet.JoinMulticastGroup = JoinMulticastGroup;
            packet.IsCompress = IsCompress;
            packet.PacketLength = PacketLength;
            //packet.Ip = Ip;
            //packet.Port = Port;
            packet.Data = Data;
            packet.Offset = Offset;
            packet.DataLength = DataLength;
            packet.ReportArrived = ReportArrived;
            return packet;
        }


        public byte[] ToBuffer()
        {

            bool hasData = Data != null && DataLength > 0;
            int offset = Offset;
            int dataLength = DataLength;
            byte[] dataTemp = Data;

            if (hasData && IsCompress)
            {
                dataTemp = Compress(dataTemp, offset, dataLength);
                offset = 0;
                dataLength = dataTemp.Length;
            }

            PacketLength = TotalHeaderLength + dataLength;

            using MemoryStream stream = new MemoryStream();
            using BinaryWriter write = new BinaryWriter(stream);

            write.Write(PacketLength);
            write.Write(Counter);
            write.Write(JoinMulticastGroup ? 1 : 0);
            write.Write(IsCompress ? (byte)1 : (byte)0);
            write.Write(ReportArrived ? (byte)1 : (byte)0);
            //write.Write(Ip);
            //write.Write((short)Port);
            write.Write(dataTemp, offset, dataLength);
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
            protocol.JoinMulticastGroup = reader.ReadByte() == 1;
            protocol.IsCompress = reader.ReadByte() == 1;
            protocol.ReportArrived = reader.ReadByte() == 1;
            //protocol.Ip = reader.ReadBytes(4);
            //protocol.Port = reader.ReadInt16();
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


        #region   Compress  &  Decompress

        private static byte[] Compress(byte[] bytes, int offset, int length)
        {
            using MemoryStream compressStream = new MemoryStream();

            using (GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, offset, length);
            }

            return compressStream.ToArray();
        }


        private static byte[] Decompress(byte[] bytes, int offset, int length)
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
