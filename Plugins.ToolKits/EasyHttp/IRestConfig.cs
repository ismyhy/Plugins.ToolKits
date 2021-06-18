using System;
using System.Text;

namespace Plugins.ToolKits.EasyHttp
{
    public interface IRestConfig : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="useMillisecondsTimeoutFunc">milliseconds Timeout</param>
        /// <returns></returns>
        IRestConfig UseTimeout(int useMillisecondsTimeoutFunc);

        IRestConfig UseBaseUrl(string useUrlFunc);

        IRestConfig UseEncoding(Encoding encodingFunc);

        IRestConfig UseSerializer(Func<object, string> serializer);

        IRestConfig UseDeserializer(Func<string, Type, object> deserializer);

        IRestConfig UseDecoder(Func<byte[], byte[]> decoder);

        IRestClient Build();
    }
}