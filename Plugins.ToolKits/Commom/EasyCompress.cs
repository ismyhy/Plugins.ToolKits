using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{
    public static class EasyCompress
    {
        // 读入大小
        private const int SIZE = 1024;


        /// <summary>
        ///     解压缩
        /// </summary>
        /// <param name="compressBuffer"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] compressBuffer)
        {
            using MemoryStream msDecompress = new MemoryStream();

            using GZipStream gzip = new GZipStream(new MemoryStream(compressBuffer), CompressionMode.Decompress);
            int len = 0;
            byte[] buffer = new byte[SIZE];

            while ((len = gzip.Read(buffer, 0, buffer.Length)) != 0)
            {
                msDecompress.Write(buffer, 0, len);
            }

            return msDecompress.ToArray();
        }


        /// <summary>
        ///     解压缩
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream Decompress(Stream stream)
        {
            MemoryStream msDecompress = new MemoryStream();

            using GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
            int len = 0;
            byte[] buffer = new byte[SIZE];

            while ((len = gzip.Read(buffer, 0, buffer.Length)) != 0)
            {
                msDecompress.Write(buffer, 0, len);
            }

            return msDecompress;
        }


        /// <summary>
        ///     压缩
        /// </summary>
        /// <param name="buffer">要压缩的字节</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] buffer)
        {
            using MemoryStream ms = new MemoryStream();
            using GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(buffer, 0, buffer.Length);

            return ms.ToArray();
        }



        /// <summary>
        ///     压缩
        /// </summary>
        /// <param name="stream">要压缩的字节</param>
        /// <returns></returns>
        public static Stream Compress(Stream stream)
        {
            using MemoryStream ms = new MemoryStream();
            using GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);

            int len = 0;
            byte[] buffer = new byte[SIZE];

            while ((len = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                compressedzipStream.Write(buffer, 0, len);
            }

            return ms;
        }
    }
}