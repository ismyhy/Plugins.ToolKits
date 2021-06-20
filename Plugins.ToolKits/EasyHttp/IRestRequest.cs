
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EasyHttp
{
    public interface IRestRequest : IDisposable
    {
        IRestRequest UseFileWriter([NotNull] Action<IFileWriter> fileWriterFunc);
        IRestRequest AddParameter<TParam>([NotNull] string key, [NotNull] TParam parameter);
        IRestRequest AddParameters([NotNull] IEnumerable<KeyValuePair<string, object>> parameters);
        IRestRequest AddParameter<TParam>([NotNull] TParam requestBody, DataFormat dataFormat = DataFormat.Json);
        IRestRequest AddFile([NotNull] string filePath);
        IRestRequest AddFiles([NotNull] IEnumerable<string> filePaths);
        IRestRequest AddFile([NotNull] string fileName, [NotNull] Stream stream);
        IRestRequest AddHeader([NotNull] string name, [NotNull] string value);
        IRestRequest AddCookie([NotNull] string name, [NotNull] string value);
        Task<IRestResponse> ExecuteAsync();
        IRestResponse Execute();
        Task<TType> ExecuteAsync<TType>();
        TType Execute<TType>();

        Method Method { get; }

        Encoding Encoding { get; }

        #region Config

        IRestRequest UseTimeout(int millisecondsTimeout);
        IRestRequest UseReadWriteTimeout(int millisecondsReadWriteTimeout);
        IRestRequest UseContinueTimeout(int millisecondsContinueTimeout);
        IRestRequest UseUserAgent(string userAgent);
        IRestRequest UseMethod(Method method);
        IRestRequest UseUrl([NotNull] string url);
        IRestRequest UseContentType([NotNull] string contentType);
        IRestRequest UseConnectionGroupName(string connectionGroupName);
        IRestRequest UseDefaultCredentials(bool useDefaultCredentials);
        IRestRequest UseExpect(bool expect);
        IRestRequest UseClientCertificates([NotNull] X509CertificateCollection clientCertificates);

        IRestRequest UseRemoteCertificateValidationCallback(
            [NotNull] RemoteCertificateValidationCallback remoteCertificateValidationCallback);

        IRestRequest UseIfModifiedSince(DateTime ifModifiedSince);
        IRestRequest UseDate(DateTime date);
        IRestRequest UseSendChunked(string sendChunked);
        IRestRequest UseConnection(string connection);
        IRestRequest UseTransferEncoding(string transferEncoding);
        IRestRequest UseKeepAlive(bool keepAlive);
        IRestRequest UsePipelined(bool pipelined);
        IRestRequest UseAllowReadStreamBuffering(bool allowReadStreamBuffering);
        IRestRequest UseAllowWriteStreamBuffering(bool allowWriteStreamBuffering);
        IRestRequest UseAllowAutoRedirect(bool allowAutoRedirect);


        IRestRequest UseMaximumResponseHeadersLength(int maximumResponseHeadersLength);
        IRestRequest UseProxy(IWebProxy proxy);

        IRestRequest UseUnsafeAuthenticatedConnectionSharing(bool unsafeAuthenticatedConnectionSharing);
        IRestRequest UsePreAuthenticate(bool preAuthenticate);
        IRestRequest UseReferer(string referer);
        IRestRequest UseMediaType(string mediaType);
        IRestRequest UseAccept(string accept);
        IRestRequest UseDisableAutomaticCompression(bool disableAutomaticCompression);

        IRestRequest UseAlwaysMultipartFormData(bool alwaysMultipartFormData);

        #endregion
    }
}