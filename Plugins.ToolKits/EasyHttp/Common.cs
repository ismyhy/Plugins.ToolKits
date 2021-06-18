using System;
using System.Collections.Generic;
using System.IO;

namespace Plugins.ToolKits.EasyHttp
{
    internal enum ParameterType
    {
        /// <summary>
        /// Cookie parameter
        /// </summary>
        Cookie,
        GetOrPost, HttpHeader, RequestBody, QueryString,
        QueryStringWithoutEncode
    }

    /// <summary>
    ///     Data formats
    /// </summary>
    public enum DataFormat
    {

        /// <summary>
        /// JSON 
        /// </summary>
        Json,
        /// <summary>
        /// XML
        /// </summary>
        Xml,
    }

    /// <summary>
    /// ResponseStatus
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Success
        /// </summary>
        Success,
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Timeout
        /// </summary>
        Timeout
    }

    internal class Parameter
    {
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
            Ticks = DateTime.Now.Ticks;
        }

        public Parameter(string name, object value, string contentType) : this(name, value)
        {
            ContentType = contentType;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public DataFormat DataFormat { get; set; } = DataFormat.Json;

        public ParameterType ParameterType { get; set; }

        public string ContentType { get; set; }

        public long Ticks { get; set; }

        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }

    internal class RequestFile : IDisposable
    {
        public RequestFile()
        {
            Ticks = DateTime.Now.Ticks;
            ContentType = "application/octet-stream";
        }

        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public long Ticks { get; }
        public string ContentType { get; }

        public void Dispose()
        {
            FileStream?.Dispose();
            FileStream = null;
            HeaderBuffer = null;
            EndBuffer = null;
        }

        public byte[] HeaderBuffer { get; set; }

        public byte[] EndBuffer { get; set; }


        public long BufferLength
        {
            get
            {
                int length1 = HeaderBuffer?.Length ?? 0;
                int length2 = EndBuffer?.Length ?? 0;
                long length3 = FileStream?.Length ?? 0;
                return length2 + length1 + length3;
            }
        }
    }


    internal static class ContentType
    {
        public const string Json = "application/json";

        public const string Xml = "application/xml";

        public static readonly Dictionary<DataFormat, string> FromDataFormat =
            new Dictionary<DataFormat, string>
            {
                {DataFormat.Xml, Xml},
                {DataFormat.Json, Json}
            };

        public static readonly string[] JsonAccept =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public static readonly string[] XmlAccept =
        {
            "application/xml", "text/xml", "*+xml", "*"
        };
    }


    internal class RequestBody
    {
        public string ContentType { get; }
        public string Name { get; }
        public object Value { get; }

        public RequestBody(string contentType, string name, object value)
        {
            ContentType = contentType;
            Name = name;
            Value = value;
        }
    }
}