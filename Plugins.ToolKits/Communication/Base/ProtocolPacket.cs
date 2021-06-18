using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Plugins.ToolKits.Communication.Base
{
    public enum PacketMode : byte
    {
        Request,
        Response,
        Arrived
    }


    /// <summary>
    ///     协议包
    /// </summary>
    public sealed class ProtocolPacket : IDisposable
    {
        private const int PacketLengthIndex = 0;
        private const int CounterIndex = PacketLengthIndex + sizeof(int);
        private const int PacketModeIndex = CounterIndex + sizeof(long);
        private const int CompressIndex = PacketModeIndex + 1;
        private const int HasResponseIndex = CompressIndex + 1;
        private const int ReportArrivedIndex = HasResponseIndex + 1;
        private const int IpIndex = ReportArrivedIndex + 1;
        private const int PortIndex = IpIndex + sizeof(int);
        private const int DataLengthIndex = PortIndex + sizeof(int);

        private const int DataMaxLength = 1024 * 64 - TotalHeaderLength;
        private const int TotalHeaderLength = 32;
        private static long _commandCounter = long.MinValue;
        private byte[] _data;
        internal bool UsingRemoteEndPoint = false;

        public ProtocolPacket()
        {
            Interlocked.Increment(ref _commandCounter);
            Counter = _commandCounter;

            if (Counter == long.MaxValue)
            {
                _commandCounter = long.MinValue;
            }
        }

        internal byte[] Ip { get; set; }
        internal int Port { get; set; }
        internal bool IsCompress { get; set; }
        internal long Counter { get; private set; }
        internal bool HasResponse { get; set; } = true;
        internal bool ReportArrived { get; set; } = true;
        public int PacketLength { get; private set; }
        public PacketMode PacketMode { get; set; } = PacketMode.Request;

        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;
                if (Data.Length >= DataMaxLength)
                {
                    throw new NotSupportedException($"The Data is Too Long,Max Length:{DataMaxLength - 1}");
                }

                PacketLength = TotalHeaderLength + _data?.Length ?? 0;
            }
        }


        public void Dispose()
        {
        }

        public override string ToString()
        {
            return $"PacketLength:{PacketLength} DataLength:{Data?.Length ?? 0}  ";
        }

        public ProtocolPacket CopyTo(ProtocolPacket packet)
        {
            packet.Counter = Counter;
            packet.IsCompress = IsCompress;
            packet.PacketLength = PacketLength;
            packet.PacketMode = PacketMode;
            packet.Ip = Ip;
            packet.Port = Port;
            packet.Data = Data;
            packet.HasResponse = HasResponse;
            packet.ReportArrived = ReportArrived;
            return packet;
        }


        private byte[] ToHeaderBuffer()
        {
            byte[] bytes = new byte[TotalHeaderLength];

            PacketLength = TotalHeaderLength + Data?.Length ?? 0;

            //包长度
            byte[] lengthBytes = BitConverter.GetBytes(PacketLength);
            Buffer.BlockCopy(lengthBytes, 0, bytes, PacketLengthIndex, lengthBytes.Length);

            //包命令计数
            byte[] counterBytes = BitConverter.GetBytes(Counter);
            Buffer.BlockCopy(counterBytes, 0, bytes, CounterIndex, counterBytes.Length);

            //包类型
            bytes[PacketModeIndex] = (byte)PacketMode;

            //是否压缩
            bytes[CompressIndex] = IsCompress ? (byte)1 : (byte)0;

            bytes[HasResponseIndex] = HasResponse ? (byte)1 : (byte)0;

            bytes[ReportArrivedIndex] = ReportArrived ? (byte)1 : (byte)0;
            //ip
            Buffer.BlockCopy(Ip ??= new byte[4], 0, bytes, IpIndex, Ip.Length);

            //端口
            byte[] portBytes = BitConverter.GetBytes(Port);
            Buffer.BlockCopy(portBytes, 0, bytes, PortIndex, portBytes.Length);

            //数据长度
            byte[] dataLengthBytes = BitConverter.GetBytes(Data?.Length ?? 0);
            Buffer.BlockCopy(dataLengthBytes, 0, bytes, DataLengthIndex, dataLengthBytes.Length);

            return bytes;
        }

        public byte[] ToBuffer()
        {
            bool hasData = Data != null && Data.Length > 0;
            if (hasData && IsCompress)
            {
                Data = Compress(Data, 0, Data.Length);
            }


            byte[] headerBuffer = ToHeaderBuffer();

            byte[] bytes = new byte[headerBuffer.Length + Data?.Length ?? 0];
            Buffer.BlockCopy(headerBuffer, 0, bytes, 0, headerBuffer.Length);

            if (hasData)
            {
                Buffer.BlockCopy(Data, 0, bytes, headerBuffer.Length, Data.Length);
            }

            return bytes;
        }

        private ProtocolPacket ParseBuffer(byte[] buffer, int offset = 0)
        {
            int bufferOffset = offset + PacketLengthIndex;
            PacketLength = BitConverter.ToInt32(buffer, bufferOffset);

            bufferOffset = offset + CounterIndex;
            Counter = BitConverter.ToInt64(buffer, bufferOffset);

            bufferOffset = offset + PacketModeIndex;
            PacketMode = (PacketMode)buffer[bufferOffset];

            bufferOffset = offset + CompressIndex;
            IsCompress = buffer[bufferOffset] == 1;

            HasResponse = buffer[HasResponseIndex] == 1;
            ReportArrived = buffer[ReportArrivedIndex] == 1;
            Ip = new byte[4];
            Buffer.BlockCopy(buffer, IpIndex, Ip, 0, Ip.Length);

            Port = BitConverter.ToInt32(buffer, offset + PortIndex);

            int dataLength = BitConverter.ToInt32(buffer, offset + DataLengthIndex);

            byte[] data = new byte[dataLength];

            Buffer.BlockCopy(buffer, TotalHeaderLength, data, 0, dataLength);

            if (IsCompress && dataLength > 0)
            {
                data = Decompress(data, 0, dataLength);
            }

            Data = data;

            return this;
        }

        public static ProtocolPacket FromBuffer(byte[] buffer, int offset)
        {
            ProtocolPacket header = new ProtocolPacket();

            header = header.ParseBuffer(buffer, offset);

            return header;
        }


        internal static ICollection<ProtocolPacket> ParseBuffers(byte[] buffer, int offset, int bufferLength)
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
                    ProtocolPacket pp = FromBuffer(buffer, offset);
                    list.Add(pp);

                    offset = currentLength;
                    continue;
                }

                break;
            } while (buffer.Length > offset + sizeof(int));

            return list;
        }


        #region   Compress  &  Decompress

        private byte[] Compress(byte[] bytes, int offset, int length)
        {
            using MemoryStream compressStream = new MemoryStream();

            using (GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, offset, length);
            }

            return compressStream.ToArray();
        }


        private byte[] Decompress(byte[] bytes, int offset, int length)
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