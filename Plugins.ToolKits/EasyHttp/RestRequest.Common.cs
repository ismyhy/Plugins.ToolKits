
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Plugins.ToolKits.EasyHttp
{
    internal partial class RestRequest
    {
        public IRestRequest UseFileWriter([NotNull] Action<IFileWriter> fileWriterFunc)
        {


            if (fileWriterFunc is null)
            {
                throw new ArgumentNullException(nameof(fileWriterFunc));
            }

            Context.Set(EasyHttpKeys.FileWriter, fileWriterFunc);

            return this;
        }


        public IRestRequest UseUrl([NotNull] string url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            Context.Set(EasyHttpKeys.RequestUrl, url);

            RequestUrl = url;
            return this;
        }


        public IRestRequest UseAlwaysMultipartFormData(bool alwaysMultipartFormData)
        {
            Context.Set(EasyHttpKeys.AlwaysMultipartFormData, alwaysMultipartFormData);

            return this;
        }


        private HttpWebRequest BuildWebClient()
        {
            string baseUri = BuildURI();

            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(baseUri);

            client.Method = Method.GET.ToString().ToUpper();
            client.AllowAutoRedirect = true;
            client.AllowWriteStreamBuffering = true;
            client.Timeout = Context.Get<int>(EasyHttpKeys.MillisecondsTimeout);

            Checker.Ignore<Exception>(() =>
            {
                client.ServicePoint.Expect100Continue = false;
                client.ServicePoint.UseNagleAlgorithm = false;
            });

            AssemblyName named = Assembly.GetExecutingAssembly().GetName();
            client.UserAgent = $"{named.Name} v{named.Version}";
            AddConfig(client);

            List<Parameter> parameterList = Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>());

            Checker.Ignore<Exception>(() =>
            {
                List<Parameter> parameters = parameterList.Where(i => i.ParameterType == ParameterType.HttpHeader)
                    .ToList();
                foreach (Parameter header in parameters)
                {
                    client.Headers[header.Name] = header.Value?.ToString() ?? "";
                }

                if (client.Headers.AllKeys.FirstOrDefault(i => i.ToLower() == "accept") is null)
                {
                    client.Headers["Accept"] =
                        "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml";
                }
            });

            client.CookieContainer ??= new CookieContainer();
            string uri = HostUri;
            List<Parameter> cookies = parameterList.Where(i => i.ParameterType == ParameterType.Cookie).ToList();
            foreach (Parameter cookie in cookies)
            {
                client.CookieContainer.Add(new System.Net.Cookie
                {
                    Name = cookie.Name,
                    Value = cookie.Value?.ToString() ?? "",
                    Domain = uri
                });
            }

            Checker.Ignore<Exception>(() =>
            {
                IWebProxy proxy = WebRequest.DefaultWebProxy;
                proxy ??= WebRequest.GetSystemWebProxy();
                client.Proxy ??= proxy;
            });

            return client;
        }

        private string BuildURI()
        {
            string targetUrl = RequestUrl;


            if (string.IsNullOrWhiteSpace(targetUrl))
            {
                throw new ArgumentNullException(nameof(targetUrl));
            }

            string baseUri = HostUri;

            if (baseUri.EndsWith("/") && targetUrl.StartsWith("/"))
            {
                baseUri = $"{baseUri}{targetUrl.Substring(1)}";
            }
            else if (baseUri.EndsWith("/") || targetUrl.StartsWith("/"))
            {
                baseUri = $"{baseUri}{targetUrl}";
            }
            else
            {
                baseUri = $"{ HostUri}/{targetUrl}";
            }

            ICollection<Parameter> parameters = GetQueryStringParameters();

            if (parameters.Count == 0)
            {
                return baseUri;
            }


            string @params = string.Join("&", parameters.Select(i => $"{i.Name}={i.Value}").ToArray());

            baseUri = $"{baseUri}?{@params}";

            return baseUri;

            ICollection<Parameter> GetQueryStringParameters()
            {
                Method method = Method;
                List<Parameter> parameterList = Context.TryGet(EasyHttpKeys.Parameters, () => new List<Parameter>());
                bool flag = method != Method.POST && method != Method.PUT && method != Method.PATCH;

                if (flag)
                {
                    return parameterList.Where(
                        p => p.ParameterType == ParameterType.GetOrPost ||
                             p.ParameterType == ParameterType.QueryString ||
                             p.ParameterType == ParameterType.QueryStringWithoutEncode
                    ).ToArray();
                }

                return parameterList
                    .Where(
                        p => p.ParameterType == ParameterType.QueryString ||
                             p.ParameterType == ParameterType.QueryStringWithoutEncode
                    ).ToArray();
            }
        }


        private RestResponse GetResponse(HttpWebRequest webRequest)
        {
            System.Text.Encoding encoding = Encoding;
            RestResponse resp = new RestResponse();
            Context.CopyTo(resp.Context);
            try
            {
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                resp.SuccessResponse(response);
                return resp;
            }
            catch (WebException webEx)
            {
                if (webEx.Response is HttpWebResponse webResp)
                {
                    resp.SuccessResponse(webResp);
                    return resp;
                }

                resp.ErrorResponse(webEx);

            }
            catch (Exception ex)
            {
                resp.ErrorResponse(ex);
            }

            return resp;
        }
    }
}