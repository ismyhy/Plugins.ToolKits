
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Plugins.ToolKits.EasyHttp
{
    internal partial class RestRequest : IRestRequest
    {
        internal readonly ContextContainer Context = new ContextContainer();
        private const string LineBreak = "\r\n";

        //internal readonly RequestContext requestCache = new RequestContext();

        public Method Method { get; private set; } = Method.GET;
        public Encoding Encoding { get; internal set; } = Encoding.UTF8;
        public string RequestUrl { get; internal set; }
        public string HostUri { get; internal set; }

        public IRestRequest AddParameter<TParam>(string key, TParam parameter)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            Parameter p = new Parameter(key, parameter, ContentType.FromDataFormat[DataFormat.Json])
            {
                ParameterType = ParameterType.GetOrPost
            };

            Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>()).Add(p);
            return this;
        }

        public IRestRequest AddParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            List<Parameter> parameterList = Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>());

            foreach (KeyValuePair<string, object> item in parameters)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    throw new ArgumentNullException(nameof(item.Key));
                }

                Parameter p = new Parameter(item.Key, item.Value, ContentType.FromDataFormat[DataFormat.Json])
                {
                    ParameterType = ParameterType.GetOrPost
                };
                parameterList.Add(p);
            }


            return this;
        }

        public IRestRequest AddParameter<TParam>(TParam parameter, DataFormat dataFormat = DataFormat.Json)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            Parameter p = new Parameter("", parameter, ContentType.FromDataFormat[dataFormat])
            {
                ParameterType = ParameterType.RequestBody
            };

            Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>()).Add(p);
            return this;
        }


        public IRestRequest AddFile([NotNull] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("file not exist", nameof(fileName));
            }

            return AddFile(fileName, File.OpenRead(fileName));
        }

        public IRestRequest AddFiles(IEnumerable<string> filePaths)
        {
            if (filePaths is null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }

            foreach (string filePath in filePaths)
            {
                AddFile(filePath);
            }

            return this;
        }

        public IRestRequest AddFile([NotNull] string fileName, [NotNull] Stream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("file does not exist", nameof(fileName));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (stream.CanRead == false)
            {
                throw new ArgumentException("Stream can not read", nameof(stream));
            }

            if (stream.CanSeek == false)
            {
                throw new ArgumentException("Stream can not seek", nameof(stream));
            }

            stream.Seek(0, SeekOrigin.Begin);


            Context.TryGet(EasyHttpKeys.Files, () => new List<RequestFile>()).Add(new RequestFile
            {
                FileName = fileName,
                FileStream = stream
            });

            return this;
        }

        public IRestRequest AddHeader([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Parameter p = new Parameter(name, value)
            {
                ParameterType = ParameterType.HttpHeader
            };
            Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>()).Add(p);
            return this;

        }

        public IRestRequest AddCookie([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }


            Parameter p = new Parameter(name, value)
            {
                ParameterType = ParameterType.Cookie
            };
            Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>()).Add(p);
            return this;
        }


        public void Dispose()
        {
            if (Context is null)
            {
                return;
            }
            Context.ToObjectCollection().OfType<IDisposable>().ForEach(i =>
            {
                Invoker.RunIgnore<Exception>(i.Dispose);
            });
            Context.Clear();
        }

        ~RestRequest()
        {
            Dispose();
        }
    }
}