using Esadad.Core.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.Helpers
{
    public static class DigitalSignatureJsonHelper
    {
        public static string SignJson(string fullJson, string privateKeyPemText, string? elementToSign = null)
        {
            string jsonToSign;
            using var document = JsonDocument.Parse(fullJson);

            JsonElement targetElement;
            if (string.IsNullOrEmpty(elementToSign))
            {
                targetElement = document.RootElement;
            }
            else
            {
                if (!document.RootElement.TryGetProperty(elementToSign, out targetElement))
                    document.RootElement.TryGetProperty(elementToSign.CapitalizeFirst(), out targetElement);

               if( !(targetElement.ValueKind == JsonValueKind.Object || targetElement.ValueKind == JsonValueKind.String))
                    throw new Exception($"Element '{elementToSign}' not found in the JSON.");
            }

            jsonToSign = ExtractOrderedValuesConcatenated(targetElement);

            // Convert to UTF-8 bytes
            var jsonBytes = Encoding.UTF8.GetBytes(jsonToSign);

            // Load private key
            using var rsa = RSA.Create();
            ImportPrivateKeyFromPem(rsa, privateKeyPemText);

            // Sign using SHA256
            var signatureBytes = rsa.SignData(jsonBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Return Base64 signature
            return Convert.ToBase64String(signatureBytes);
        }

        // Verify the signature using public key
        public static bool VerifyJson(string fullJson, string publicKeyPemText, string? elementToVerify = null)
        {
            using var document = JsonDocument.Parse(fullJson);
            string jsonToVerify;

            JsonElement targetElement;
            if (string.IsNullOrEmpty(elementToVerify))
            {
                targetElement = document.RootElement;
            }
            else
            {
                if (!document.RootElement.TryGetProperty(elementToVerify, out targetElement))
                    throw new Exception($"Element '{elementToVerify}' not found in the JSON.");
            }

            jsonToVerify = ExtractOrderedValuesConcatenated(targetElement);

            // Try to locate the signature
            JsonElement sigJsonElement;
            if (!document.RootElement.TryGetProperty("signature", out sigJsonElement))
                document.RootElement.TryGetProperty("Signature", out sigJsonElement);

            if (!(sigJsonElement.ValueKind == JsonValueKind.String))
            {
                JsonElement msgFooter;
                JsonElement security;
                if (!document.RootElement.TryGetProperty("msgFooter", out msgFooter))
                    document.RootElement.TryGetProperty("MsgFooter", out msgFooter);
                if (!msgFooter.TryGetProperty("security", out security))
                    msgFooter.TryGetProperty("Security", out security);

                if (!security.TryGetProperty("signature", out sigJsonElement))
                    security.TryGetProperty("Signature", out sigJsonElement);

                if (!(sigJsonElement.ValueKind == JsonValueKind.String))
                    throw new Exception("Signature not found in the JSON.");
            }
            

            var signatureBase64 = sigJsonElement.GetString();
            if (string.IsNullOrWhiteSpace(signatureBase64))
                throw new Exception("Signature is empty or null.");

            // Convert to UTF-8 bytes
            var jsonToVerifyBytes = Encoding.UTF8.GetBytes(jsonToVerify);

            // Load public key
            using var rsa = RSA.Create();
            ImportPublicKeyFromPem(rsa, publicKeyPemText);

            // Convert signature from Base64
            var signatureBytes = Convert.FromBase64String(signatureBase64);

            // Verify
            return rsa.VerifyData(jsonToVerifyBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        // Concatenate all values in a JSON object (flat) ordered by property name
        // ✅ Flatten and concatenate primitive values (strings, numbers, bools) in deterministic order
        private static string ExtractOrderedValuesConcatenated(JsonElement element)
        {
            var valuesInOrder = new List<string>();

            void Traverse(JsonElement elem)
            {
                switch (elem.ValueKind)
                {
                    case JsonValueKind.Object:
                        foreach (var prop in elem.EnumerateObject())
                        {
                            Traverse(prop.Value);
                        }
                        break;

                    case JsonValueKind.Array:
                        foreach (var item in elem.EnumerateArray())
                        {
                            Traverse(item);
                        }
                        break;

                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        valuesInOrder.Add(elem.ToString());
                        break;

                    // Optional: skip nulls or include as "null"
                    case JsonValueKind.Null:
                        valuesInOrder.Add("null");
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported value kind '{elem.ValueKind}'");
                }
            }

            Traverse(element);
            return string.Join("|", valuesInOrder);
        }

        // Sign the JSON (only MsgBody) with private key
        public static string SignJsonOld(string fullJson, string privateKeyPemText, string? elementToSign = null)
        {
            string jsonToSign;

            if (string.IsNullOrEmpty(elementToSign))
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false

                };
                // Sign the entire JSON
                jsonToSign = JsonSerializer.Serialize(fullJson, options);
            }
            else
            {
                // Parse the full JSON
                using var document = JsonDocument.Parse(fullJson);
                if (!document.RootElement.TryGetProperty(elementToSign, out var element))
                {
                    throw new Exception($"Element '{elementToSign}' not found in the JSON.");
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false

                };

                // Serialize the selected element back to a string
                jsonToSign = JsonSerializer.Serialize(element, options);
            }



            // Convert to UTF-8 bytes
            var jsonBytes = Encoding.UTF8.GetBytes(jsonToSign);

            // Load private key
            using var rsa = RSA.Create();
            ImportPrivateKeyFromPem(rsa, privateKeyPemText);

            // Sign using SHA256
            var signatureBytes = rsa.SignData(jsonBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Return Base64 signature
            return Convert.ToBase64String(signatureBytes);
        }

        // Verify the signature using public key
        public static bool VerifyJsonOld(string fullJson, string publicKeyPemText, string? elementToVerify = null)
        {
            var document = JsonDocument.Parse(fullJson);
            string jsonToVerify;
            string signature;

            if (string.IsNullOrEmpty(elementToVerify))
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false

                };
                // Sign the entire JSON
                jsonToVerify = JsonSerializer.Serialize(fullJson, options);
                // Sign the entire JSON
                // jsonToVerify = fullJson;
            }
            else
            {

                if (!document.RootElement.TryGetProperty(elementToVerify, out var element))
                {
                    throw new Exception("MsgBody not found in the JSON.");
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false

                };

                // Serialize the selected element back to a string
                //jsonToSign = JsonSerializer.Serialize(element, );
                jsonToVerify = JsonSerializer.Serialize(element, options);
            }

            JsonElement sigJsonElement;
            sigJsonElement = document.RootElement.GetProperty("Signature");
            if (sigJsonElement.ValueKind == JsonValueKind.Null || sigJsonElement.ValueKind == JsonValueKind.Undefined)
            {
                var msgFooterElement = document.RootElement.GetProperty("MsgFooter");
                if (msgFooterElement.ValueKind == JsonValueKind.Null || msgFooterElement.ValueKind == JsonValueKind.Undefined)
                {
                    var securityElement = msgFooterElement.GetProperty("Security");
                    if (securityElement.ValueKind == JsonValueKind.Null || securityElement.ValueKind == JsonValueKind.Undefined)
                    {
                        sigJsonElement = securityElement.GetProperty("Signature");

                    }
                }

            }
            if (sigJsonElement.ValueKind == JsonValueKind.Null || sigJsonElement.ValueKind == JsonValueKind.Undefined)
            {
                throw new Exception("Signature not found in the JSON");
            }





            //    var signatureBase64 = string.IsNullOrEmpty(authSignatureElement.GetString()) ? authSignatureElement.GetString() : signatureElement.GetString();


            //if (string.IsNullOrEmpty(signatureBase64))
            //{
            //    throw new Exception("Signature not found in the JSON");
            //}


            // Convert to UTF-8 bytes
            var jsonToVerifyBytes = Encoding.UTF8.GetBytes(jsonToVerify);

            // Load public key
            using var rsa = RSA.Create();
            ImportPublicKeyFromPem(rsa, publicKeyPemText);

            //rsa.FromXmlString(publicKeyXml);

            // Convert signature from Base64
            var signatureBytes = Convert.FromBase64String(sigJsonElement.GetString());

            // Verify
            return rsa.VerifyData(jsonToVerifyBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        private static void ImportPrivateKeyFromPem(RSA rsa, string pem)
        {
            pem = NormalizePem(pem);

            if (pem.Contains("BEGIN RSA PRIVATE KEY"))
            {
                var keyBytes = DecodePem(pem, "RSA PRIVATE KEY");
                rsa.ImportRSAPrivateKey(keyBytes, out _);
            }
            else if (pem.Contains("BEGIN PRIVATE KEY"))
            {
                var keyBytes = DecodePem(pem, "PRIVATE KEY");
                rsa.ImportPkcs8PrivateKey(keyBytes, out _);
            }
            else
            {
                throw new ArgumentException("Unsupported PEM format for private key.");
            }
        }

        private static void ImportPublicKeyFromPem(RSA rsa, string pem)
        {
            pem = NormalizePem(pem);

            if (pem.Contains("BEGIN PUBLIC KEY"))
            {
                var keyBytes = DecodePem(pem, "PUBLIC KEY");
                rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
            }
            else
            {
                throw new ArgumentException("Unsupported PEM format for public key.");
            }
        }

        private static byte[] DecodePem(string pem, string section)
        {
            var header = $"-----BEGIN {section}-----";
            var footer = $"-----END {section}-----";

            var start = pem.IndexOf(header) + header.Length;
            var end = pem.IndexOf(footer, start);
            var base64 = pem.Substring(start, end - start).Replace("\n", "").Replace("\r", "").Trim();

            return Convert.FromBase64String(base64);
        }

        private static string NormalizePem(string pem)
        {
            return pem.Trim().Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
