using System;
using System.Security.Cryptography;

namespace Plugins.ToolKits.EncryptExtensions
{
    public interface IRSAProvider
    {
        string GetPrivateKey();
        string GetPublicKey();
    }


    public static class RSAEncrypter
    {
        public static IRSAProvider GerProvider()
        {
            return new RSAProvider();
        }


        //public static byte[] Encrypt(byte[] data, string publicKey)
        //{
        //    var msInput = new MemoryStream(data);

        //    var msOutput = new MemoryStream();
        //    var rsa = new RSACryptoServiceProvider();

        //    var keyBuffer = Convert.FromBase64String(publicKey);

        //    rsa.ImportCspBlob(keyBuffer);
        //    try
        //    {
        //        var keySize = rsa.KeySize / 8;
        //        var bufferSize = keySize - 11;
        //        var buffer = new byte[bufferSize];
        //        var readLen = msInput.Read(buffer, 0, bufferSize);
        //        while (readLen > 0)
        //        {
        //            var dataToEnc = new byte[readLen];

        //            Array.Copy(buffer, 0, dataToEnc, 0, readLen);

        //            var encData = rsa.Encrypt(dataToEnc, false);

        //            msOutput.Write(encData, 0, encData.Length);

        //            readLen = msInput.Read(buffer, 0, bufferSize);
        //        }

        //        return msOutput.ToArray();
        //    }
        //    finally
        //    {
        //        msInput.Close();
        //        msOutput.Close();
        //        rsa.Clear();
        //    }
        //}

        //public static byte[] Decrypt(byte[] data, string privateKey)
        //{
        //    var RSA = new RSACryptoServiceProvider();
        //    var keybuff = Convert.FromBase64String(privateKey);
        //    RSA.ImportCspBlob(keybuff);

        //    var keySize = RSA.KeySize / 8;

        //    var buffer = new byte[keySize];

        //    var msInput = new MemoryStream(data);

        //    var msOutput = new MemoryStream();

        //    try
        //    {
        //        var readLen = msInput.Read(buffer, 0, keySize);

        //        while (readLen > 0)
        //        {
        //            var dataToDec = new byte[readLen];

        //            Array.Copy(buffer, 0, dataToDec, 0, readLen);

        //            var decData = RSA.Decrypt(dataToDec, false);

        //            msOutput.Write(decData, 0, decData.Length);

        //            readLen = msInput.Read(buffer, 0, keySize);
        //        }

        //        return msOutput.ToArray(); //得到解密结果
        //    }
        //    finally
        //    {
        //        msInput.Close();
        //        msOutput.Close();

        //        RSA.Clear();
        //    }
        //}


        internal class RSAProvider : IRSAProvider, IDisposable
        {
            private readonly RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider();

            public void Dispose()
            {
                _rsa?.Clear();
                _rsa?.Dispose();
            }

            public string GetPrivateKey()
            {
                return Convert.ToBase64String(_rsa.ExportCspBlob(true));
            }

            public string GetPublicKey()
            {
                return Convert.ToBase64String(_rsa.ExportCspBlob(false));
            }
        }
    }
}