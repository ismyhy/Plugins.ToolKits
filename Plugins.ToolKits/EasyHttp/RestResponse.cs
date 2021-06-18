
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EasyHttp
{
    internal sealed class RestResponse : IRestResponse
    {
        internal RestResponse()
        {
        }

        public void Dispose()
        {
            if (Context is null)
            {
                return;
            }
            List<IDisposable> disosableObjects = Context.ToObjectCollection().OfType<IDisposable>().ToList();
            disosableObjects.ForEach(i => Invoker.RunIgnore<Exception>(() => i.Dispose()));
            disosableObjects.Clear();
            Context.Clear();
        }


        public ResponseStatus ResponseStatus { get; private set; }
        public IReadOnlyDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public IReadOnlyList<Cookie> Cookies { get; private set; } = new List<Cookie>();
        public string ProtocolVersion { get; private set; }
        public string ContentEncoding { get; private set; }
        public string ContentType { get; private set; }
        public long ContentLength { get; private set; }
        public string ResponseUrl { get; private set; }
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }

        ~RestResponse()
        {
            Dispose();
        }



        #region IResultResponse

        TType IRestResponse.GetResult<TType>()
        {
            if (ResponseStatus != ResponseStatus.Success)
            {
                Exception ex = Exception.GetBaseException();
                throw new Exception($"ResponseStatus :{ResponseStatus} {Environment.NewLine}{ex.Message}",
                    ex.GetBaseException());
            }

            if (Context.Get<Exception>(EasyHttpKeys.FileWriterException) is Exception e)
            {
                throw e;
            }

            if (!Context.Get<bool>(EasyHttpKeys.CanGetResult))
            {
                throw new InvalidOperationException("fileWriter callback has been registered");
            }

            Stream stream = Context.Get<Stream>(EasyHttpKeys.ResponseStream);

            if (stream is null || stream.Length == 0 || stream.CanRead == false)
            {
                return default;
            }


            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            buffer = Context.Get<Func<byte[], byte[]>>(EasyHttpKeys.Decoder)?.Invoke(buffer) ?? buffer;

            string stringBuffer = Context.Get<Encoding>(EasyHttpKeys.Encoding).GetString(buffer);

            Func<string, Type, object> Deserializer = Context.Get<Func<string, Type, object>>(EasyHttpKeys.Deserializer);

            try
            {
                object o = Deserializer.Invoke(stringBuffer, typeof(TType));

                if (o is TType t)
                {
                    return t;
                }

                return default;
            }
            catch (Exception excep)
            {
                throw new Exception("An exception occurred during deserialization", excep);
            }

        }

        Task<TType> IRestResponse.GetResultAsync<TType>()
        {
            return Task.Factory.StartNew(((IRestResponse)this).GetResult<TType>, CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        #endregion


        #region CreateResult 
        internal readonly ContextContainer Context = new ContextContainer();

        internal void SuccessResponse(HttpWebResponse response)
        {
            Context.Set(EasyHttpKeys.CanGetResult, true);
            Context.Set(EasyHttpKeys.HttpWebResponse, response);

            Stream ResponseStream = response.GetResponseStream();

            Context.Set(EasyHttpKeys.WebResponseStream, ResponseStream);

            ManualResetEvent ManualReset = null;
            if (HttpStatusCode.OK == response.StatusCode && ResponseStream != null)
            {
                Stream stream = new MemoryStream();
                ResponseStream.CopyTo(stream);

                Context.Set(EasyHttpKeys.ResponseStream, stream);

                //BinaryWriter Writer = new BinaryWriter(stream); 
                //Context.Set(EasyHttpKeys.BinaryWriter, Writer);
                //byte[] buffer = new byte[1024];
                //while (true)
                //{
                //    int readLength = ResponseStream.Read(buffer, 0, buffer.Length);
                //    if (readLength > 0)
                //    {
                //        Writer.Write(buffer, 0, readLength);
                //        continue;
                //    } 
                //    break;
                //}
                //buffer = null;

                stream.Seek(0, SeekOrigin.Begin);

                bool fresut = Context.TryGet(EasyHttpKeys.FileWriter, out Action<IFileWriter> fileWriter);


                if (fresut && fileWriter != null)
                {
                    ManualReset = new ManualResetEvent(false);
                    Context.Set(EasyHttpKeys.ManualReset, ManualReset);
                    Context.Set(EasyHttpKeys.CanGetResult, false);
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            fileWriter.Invoke(new FileWriter(stream));
                        }
                        catch (Exception e)
                        {
                            Context.Set(EasyHttpKeys.FileWriterException, new Exception("fileWriter Invoke Error", e));
                        }
                        finally
                        {
                            ManualReset.Set();
                            stream.Seek(0, SeekOrigin.Begin);
                        }
                    }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                }
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            Headers = dic;
            foreach (string itemKey in response.Headers.AllKeys)
            {
                dic[itemKey] = response.Headers[itemKey] ?? "";
            }

            List<Cookie> list = new List<Cookie>();
            Cookies = list;
            foreach (System.Net.Cookie cookie in response.Cookies.OfType<System.Net.Cookie>().ToArray())
            {
                list.Add(new Cookie
                {
                    Value = cookie.Value,
                    Name = cookie.Name,
                    Domain = cookie.Domain,
                    Comment = cookie.Comment,
                    CommentUri = cookie.CommentUri,
                    Discard = cookie.Discard,
                    Expired = cookie.Expired,
                    Expires = cookie.Expires,
                    HttpOnly = cookie.HttpOnly,
                    Path = cookie.Path,
                    Port = cookie.Port,
                    Secure = cookie.Secure,
                    TimeStamp = cookie.TimeStamp,
                    Version = cookie.Version
                });
            }

            ProtocolVersion = "HTTP/" + response.ProtocolVersion;
            ContentEncoding = response.ContentEncoding ?? response.CharacterSet;
            ContentType = response.ContentType;
            ContentLength = response.ContentLength;
            ResponseUrl = response.ResponseUri.ToString();
            StatusCode = (int)response.StatusCode;
            StatusDescription = response.StatusDescription;
            ResponseStatus = ResponseStatus.Success;

            ManualReset?.WaitOne();
        }

        internal void ErrorResponse(Exception exception)
        {
            RestResponse response = new RestResponse();
            Exception ex = exception.GetBaseException();
            ErrorMessage = ex.Message;
            Exception = ex;
            response.ResponseStatus = ResponseStatus.Error;
            if (exception is WebException webException && webException.Status == WebExceptionStatus.Timeout)
            {
                response.ResponseStatus = ResponseStatus.Timeout;
            }

        }

        #endregion



        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("REST Response:");
            if (ResponseStatus != ResponseStatus.Success)
            {
                s.Append("  Error").AppendLine().AppendLine();
                s.AppendLine($"Status Code        : { StatusCode}");
                s.AppendLine($"Status Description : { StatusDescription}");
                s.AppendLine($"Error Message      : { ErrorMessage}");
                return s.ToString();
            }


            s.Append("  Success").AppendLine().AppendLine();
            s.AppendLine($"Status Code        : { StatusCode}");
            s.AppendLine($"Status Description : { StatusDescription}");
            s.AppendLine();
            int maxLength = Headers.Count > 0 ? Headers.Max(i => i.Key.Length) : 0;
            int maxLength2 = Cookies.Count > 0 ? Cookies.Max(i => i.Name.Length) : 0;
            maxLength = Math.Max(maxLength, maxLength2);
            if (Headers.Count > 0)
            {
                s.AppendLine("Headers:");
                foreach (KeyValuePair<string, string> header in Headers)
                {
                    s.AppendLine($"    {header.Key.PadRight(maxLength)} : {header.Value}");
                }

                s.AppendLine();
            }

            if (Cookies.Count > 0)
            {
                s.AppendLine("Cookies:");
                foreach (Cookie cookies in Cookies)
                {
                    s.AppendLine($"    {cookies.Name.PadRight(maxLength)} : {cookies.Value}");
                }

                s.AppendLine();
            }

            s.AppendLine($"Content Encoding   : { ContentEncoding}");
            s.AppendLine($"ContentType        : { ContentType}");
            s.AppendLine($"ResponseUrl        : { ResponseUrl}");
            s.AppendLine($"Content Length     : { ContentLength}");
            if (Context.Get<Stream>(EasyHttpKeys.ResponseStream) is Stream stream)
            {
                s.AppendLine($"Stream Length      : { stream.Length}");
            }

            return s.ToString();
        }


        private class FileWriter : IFileWriter
        {
            private readonly Stream stream;

            public FileWriter(Stream stream)
            {
                Length = stream.Length;
                this.stream = stream;
            }

            public long Length { get; }

            public long WriteTo(Stream targetStream)
            {
                if (targetStream is null)
                {
                    throw new ArgumentNullException(nameof(targetStream));
                }

                if (!targetStream.CanWrite)
                {
                    throw new ArgumentException("Stream can not write");
                }

                stream.CopyTo(targetStream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.Length;
            }

            public override string ToString()
            {
                return $"Buffer Size:{Length}";
            }

            public byte[] ToArray()
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return buffer;
            }

            public Stream ToStream()
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                MemoryStream st = new MemoryStream(buffer);
                return st;

            }
        }

    }
}