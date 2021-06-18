
using System;
using System.Text;

namespace Plugins.ToolKits.EasyHttp
{
    public enum Method
    {
        GET, POST, PUT, DELETE, HEAD, OPTIONS,
        PATCH, MERGE, COPY
    }

    public interface IRestClient : IDisposable
    {
        IRestRequest Create();
    }

    internal class EasyHttpKeys
    {
        public const string MillisecondsTimeout = "MillisecondsTimeout";
        public const string BaseUrl = "BaseUrl";
        public const string Encoding = "Encoding";
        public const string Serializer = "Serializer";
        public const string Deserializer = "Deserializer";
        public const string Decoder = "Decoder";
        public const string FileWriter = "FileWriter";
        public const string RequestUrl = "RequestUrl";
        public const string AlwaysMultipartFormData = "AlwaysMultipartFormData";
        public const string Parameters = "Parameters";
        public const string Files = "Files";
        public const string RequestBody = "RequestBody";
        public const string CanGetResult = "CanGetResult";
        public const string HttpWebResponse = "HttpWebResponse";
        public const string ResponseStream = "ResponseStream";
        public const string WebResponseStream = "WebResponseStream";
        public const string BinaryWriter = "BinaryWriter";
        public const string FileWriterException = "FileWriterException";
        public const string ManualReset = "ManualReset";
    }



    public sealed class RestConfig : IRestConfig, IRestClient, IDisposable
    {

        private readonly ContextContainer Context = new ContextContainer();


        private RestConfig()
        {
            Context.Set(EasyHttpKeys.Encoding, Encoding.UTF8);
            Context.Set(EasyHttpKeys.MillisecondsTimeout, 100000);
        }

        public void Dispose()
        {
        }

        IRestConfig IRestConfig.UseTimeout(int useMillisecondsTimeout)
        {
            Context.Set(EasyHttpKeys.MillisecondsTimeout, useMillisecondsTimeout);
            return this;
        }

        IRestConfig IRestConfig.UseBaseUrl(string useBaseUrl)
        {
            Context.Set(EasyHttpKeys.BaseUrl, useBaseUrl);
            return this;
        }

        IRestConfig IRestConfig.UseEncoding(Encoding encoding)
        {
            Context.Set(EasyHttpKeys.Encoding, encoding);
            return this;
        }

        IRestConfig IRestConfig.UseSerializer(Func<object, string> serializer)
        {
            Context.Set(EasyHttpKeys.Serializer, serializer);
            return this;
        }

        IRestConfig IRestConfig.UseDeserializer(Func<string, Type, object> deserializer)
        {
            Context.Set(EasyHttpKeys.Deserializer, deserializer);
            return this;
        }

        public IRestConfig UseDecoder(Func<byte[], byte[]> decoder)
        {
            Context.Set(EasyHttpKeys.Decoder, decoder);
            return this;
        }

        IRestClient IRestConfig.Build()
        {
            if (!Context.TryGet(EasyHttpKeys.BaseUrl, out object _))
            {
                throw new ArgumentNullException(nameof(EasyHttpKeys.BaseUrl),
                    $"Function:{nameof(IRestConfig.UseBaseUrl)} must be registered");
            }

            if (!Context.TryGet(EasyHttpKeys.Serializer, out object _))
            {
                throw new ArgumentNullException(nameof(EasyHttpKeys.Serializer),
                    $"Function:{nameof(IRestConfig.UseSerializer)} must be registered");
            }

            if (!Context.TryGet(EasyHttpKeys.Deserializer, out object _))
            {
                throw new ArgumentNullException(nameof(EasyHttpKeys.Deserializer),
                    $"Function:{nameof(IRestConfig.UseDeserializer)} must be registered");
            }



            return this;
        }

        IRestRequest IRestClient.Create()
        {
            RestRequest req = new RestRequest
            {
                HostUri = Context.Get<string>(EasyHttpKeys.BaseUrl),
                Encoding = Context.Get<Encoding>(EasyHttpKeys.Encoding)
            };
            Context.CopyTo(req.Context);

            return req;
        }

        /// <summary>
        /// Create Default Configuration Object
        /// </summary>
        /// <returns></returns>
        public static IRestConfig Default()
        {
            return new RestConfig();
        }

        private class HostCache : IDisposable
        {
            public Encoding Encoding { get; internal set; } = System.Text.Encoding.UTF8;
            public Func<object, string> SerializerFunc { get; internal set; }
            public string BaseUrl { get; internal set; }
            public Func<string, Type, object> DeserializerFunc { get; internal set; }
            public int Timeout { get; internal set; } = 10000;
            public Func<byte[], byte[]> DecoderFunc { get; set; }

            public void Dispose()
            {
                Encoding = null;
                SerializerFunc = null;
                BaseUrl = null;
                DeserializerFunc = null;
                Timeout = 0;
            }
        }
    }
}