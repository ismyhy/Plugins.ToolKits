using Plugins.ToolKits.Attributes;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Plugins.ToolKits.EasyHttp
{
    internal partial class RestRequest
    {
        private readonly IDictionary<string, object> RequestConfigs = new Dictionary<string, object>();

        public IRestRequest UseContentType([NotNull] string contentType)
        {
            RequestConfigs[nameof(HttpWebRequest.ContentType)] = contentType ?? throw new ArgumentNullException(nameof(contentType));

            return this;
        }

        public IRestRequest UseTimeout(int millisecondsTimeout)
        {
            RequestConfigs[nameof(HttpWebRequest.Timeout)] = millisecondsTimeout;

            return this;
        }

        public IRestRequest UseUserAgent([NotNull] string userAgent)
        {
            RequestConfigs[nameof(HttpWebRequest.UserAgent)] = userAgent ?? throw new ArgumentNullException(nameof(userAgent));

            return this;
        }

        public IRestRequest UseMethod(Method method)
        {
            RequestConfigs[nameof(HttpWebRequest.Method)] = method.ToString().ToUpper();
            Method = method;
            return this;
        }



        public IRestRequest UseReadWriteTimeout(int millisecondsReadWriteTimeout)
        {
            RequestConfigs[nameof(HttpWebRequest.ReadWriteTimeout)] = millisecondsReadWriteTimeout;
            return this;
        }

        public IRestRequest UseContinueTimeout(int millisecondsContinueTimeout)
        {
            RequestConfigs[nameof(HttpWebRequest.ContinueTimeout)] = millisecondsContinueTimeout;
            return this;
        }

        public IRestRequest UseConnectionGroupName(string connectionGroupName)
        {
            RequestConfigs[nameof(HttpWebRequest.ConnectionGroupName)] = connectionGroupName;
            return this;
        }

        public IRestRequest UseDefaultCredentials(bool useDefaultCredentials)
        {
            RequestConfigs[nameof(HttpWebRequest.UseDefaultCredentials)] = useDefaultCredentials;
            return this;
        }

        public IRestRequest UseExpect(bool expect)
        {
            RequestConfigs[nameof(HttpWebRequest.Expect)] = expect;
            return this;
        }

        public IRestRequest UseClientCertificates(X509CertificateCollection clientCertificates)
        {
            RequestConfigs[nameof(HttpWebRequest.ClientCertificates)] = clientCertificates;
            return this;
        }

        public IRestRequest UseRemoteCertificateValidationCallback(
            RemoteCertificateValidationCallback remoteCertificateValidationCallback)
        {
            RequestConfigs[nameof(HttpWebRequest.ServerCertificateValidationCallback)] = remoteCertificateValidationCallback;
            return this;
        }

        public IRestRequest UseIfModifiedSince(DateTime ifModifiedSince)
        {
            RequestConfigs[nameof(HttpWebRequest.IfModifiedSince)] = ifModifiedSince;
            return this;
        }

        public IRestRequest UseDate(DateTime date)
        {
            RequestConfigs[nameof(HttpWebRequest.Date)] = date;
            return this;
        }

        public IRestRequest UseSendChunked(string sendChunked)
        {
            RequestConfigs[nameof(HttpWebRequest.SendChunked)] = sendChunked;
            return this;
        }

        public IRestRequest UseConnection(string connection)
        {
            RequestConfigs[nameof(HttpWebRequest.Connection)] = connection;
            return this;
        }

        public IRestRequest UseTransferEncoding(string transferEncoding)
        {
            RequestConfigs[nameof(HttpWebRequest.TransferEncoding)] = transferEncoding;
            return this;
        }

        public IRestRequest UseKeepAlive(bool keepAlive)
        {
            RequestConfigs[nameof(HttpWebRequest.KeepAlive)] = keepAlive;
            return this;
        }

        public IRestRequest UsePipelined(bool pipelined)
        {
            RequestConfigs[nameof(HttpWebRequest.Pipelined)] = pipelined;
            return this;
        }

        public IRestRequest UseAllowReadStreamBuffering(bool allowReadStreamBuffering)
        {
            RequestConfigs[nameof(HttpWebRequest.AllowReadStreamBuffering)] = allowReadStreamBuffering;
            return this;
        }

        public IRestRequest UseAllowWriteStreamBuffering(bool allowWriteStreamBuffering)
        {
            RequestConfigs[nameof(HttpWebRequest.AllowWriteStreamBuffering)] = allowWriteStreamBuffering;
            return this;
        }

        public IRestRequest UseAllowAutoRedirect(bool allowAutoRedirect)
        {
            RequestConfigs[nameof(HttpWebRequest.AllowAutoRedirect)] = allowAutoRedirect;
            return this;
        }

        public IRestRequest UseMaximumResponseHeadersLength(int maximumResponseHeadersLength)
        {
            RequestConfigs[nameof(HttpWebRequest.MaximumResponseHeadersLength)] = maximumResponseHeadersLength;
            return this;
        }

        public IRestRequest UseProxy(IWebProxy proxy)
        {
            RequestConfigs[nameof(HttpWebRequest.Proxy)] = proxy;
            return this;
        }

        public IRestRequest UseUnsafeAuthenticatedConnectionSharing(bool unsafeAuthenticatedConnectionSharing)
        {
            RequestConfigs[nameof(HttpWebRequest.UnsafeAuthenticatedConnectionSharing)] = unsafeAuthenticatedConnectionSharing;
            return this;
        }

        public IRestRequest UsePreAuthenticate(bool preAuthenticate)
        {
            RequestConfigs[nameof(HttpWebRequest.PreAuthenticate)] = preAuthenticate;
            return this;
        }

        public IRestRequest UseReferer(string referer)
        {
            RequestConfigs[nameof(HttpWebRequest.Referer)] = referer;
            return this;
        }

        public IRestRequest UseMediaType(string mediaType)
        {
            RequestConfigs[nameof(HttpWebRequest.MediaType)] = mediaType;
            return this;
        }

        public IRestRequest UseAccept(string accept)
        {
            RequestConfigs[nameof(HttpWebRequest.Accept)] = accept;
            return this;
        }

        public IRestRequest UseDisableAutomaticCompression(bool disableAutomaticCompression)
        {
            DecompressionMethods v = disableAutomaticCompression
                ? DecompressionMethods.None
                : DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;

            RequestConfigs[nameof(HttpWebRequest.AutomaticDecompression)] = v;
            return this;
        }
    }
}