using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Silver.Pay.Extensions
{
    public static class WeChartPaySecurityHelper
    {

        public static string EncryptParameters(string p12FilePath, string privateKeyPassword, string parameters)
        {
            using (var pfx = new X509Certificate2(p12FilePath, privateKeyPassword))
            {
                // 从p12证书中提取私钥
                using (var rsaPrivateKey = (RSA)pfx.PrivateKey)
                {
                    // 使用私钥进行RSA加密
                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(parameters);
                    byte[] encryptedData = rsaPrivateKey.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
                    string value = BitConverter.ToString(encryptedData).Replace("-", "");
                    return value;
                }
            }
        }
         

    }
}
