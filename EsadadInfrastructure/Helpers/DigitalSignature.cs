using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Esadad.Infrastructure.MemCache;

namespace Esadad.Infrastructure.Helpers
{
    public class DigitalSignature
    {
        public static bool VerifySignature(XmlElement xmlElement)
        {
            string signatureValue = xmlElement.SelectSingleNode("//Signature")?.InnerText;

            string msgbodyValue = XElement.Parse(xmlElement.SelectSingleNode("//MsgBody").OuterXml).ToString(SaveOptions.DisableFormatting);

            if (string.IsNullOrEmpty(signatureValue))
            {
                return false;

            }

            // Load the public key from the certificate (CRT file)
            string eSadadCertPath = MemoryCache.Certificates.CertInfos.First(c => c.Type == "Public").Path;

            // Verify the signature
            bool isSignatureValid = Verify(msgbodyValue, signatureValue, eSadadCertPath);

            if (isSignatureValid)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static bool Verify(string data, string signature, string eSadadCertPath)
        {
            if (!string.IsNullOrEmpty(signature)
                  && !string.IsNullOrEmpty(data)
                  && !string.IsNullOrEmpty(eSadadCertPath)
                  )
            {
                X509Certificate2 certificate = new X509Certificate2(eSadadCertPath);
                if (certificate != null)
                {
                    RSA rsaPublicKey = certificate.GetRSAPublicKey();

                    bool result = false;
                    if (rsaPublicKey != null)
                    {
                        result = rsaPublicKey.VerifyData(
                          Encoding.Unicode.GetBytes(data),
                           Convert.FromBase64String(signature),
                           HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    }
                    return result;
                }
            }
            return false;

        }

        public static string SignMessage(XmlElement xmlElement)
        {
            string msgbodyValue = XElement.Parse(xmlElement.SelectSingleNode("//MsgBody").OuterXml).ToString(SaveOptions.DisableFormatting);

            // Load the private key from the certificate (pfx file)
            var billerCertPfx = MemoryCache.Certificates.CertInfos.First(c => c.Type == "Private");

            // Verify the signature
            string signedSignature = Sign(msgbodyValue, billerCertPfx.Path, billerCertPfx.Password);


            return signedSignature;
        }

        private static string Sign(string data, string BillerCertPath, string BillerCertPassword)
        {
            try
            {
                if (!string.IsNullOrEmpty(data)
                    && !string.IsNullOrEmpty(BillerCertPath)
                    && !string.IsNullOrEmpty(BillerCertPassword)
                )
                {
                    X509Certificate2 certificate = new X509Certificate2(BillerCertPath, BillerCertPassword);

                    // Extract the private key
                    RSA rsa = certificate.GetRSAPrivateKey();

                    // Sign the message
                    byte[] signature = rsa.SignData(Encoding.Unicode.GetBytes(data),
                                                       HashAlgorithmName.SHA256,
                                                       RSASignaturePadding.Pkcs1);
                    //Convert Signature to Base64String
                    return Convert.ToBase64String(signature);
                }
                return "";

            }
            catch (Exception e)
            {
                throw;

            }

        }
    }
}