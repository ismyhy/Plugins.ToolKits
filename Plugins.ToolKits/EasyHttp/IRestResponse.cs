using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EasyHttp
{
    public interface IRestResponse : IDisposable
    {

        #region Property

        IReadOnlyDictionary<string, string> Headers { get; }
        IReadOnlyList<Cookie> Cookies { get; }
        string ProtocolVersion { get; }

        string ContentEncoding { get; }

        string ContentType { get; }

        long ContentLength { get; }

        string ResponseUrl { get; }

        int StatusCode { get; }

        string StatusDescription { get; }

        string ErrorMessage { get; }

        Exception Exception { get; }

        #endregion



        ResponseStatus ResponseStatus { get; }

        TType GetResult<TType>();

        Task<TType> GetResultAsync<TType>();
    }


    public interface IFileWriter
    {
        long Length { get; }

        long WriteTo(Stream stream);

        byte[] ToArray();

        Stream ToStream();
    }
}