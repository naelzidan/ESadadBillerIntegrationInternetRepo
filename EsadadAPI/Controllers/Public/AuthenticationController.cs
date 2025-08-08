using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.DTOs.Public;
using Esadad.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Esadad.API.Controllers.Public
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {

        private readonly JwtSettings _jwtSettings;
        public AuthenticationController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthRequestDto request)
        {
            // return Ok(request);
            // var tranTimestamp1 = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            string jsonData = System.Text.Json.JsonSerializer.Serialize(request, options);
            var pubKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqPV7ZSSUq2fJ7/IeGlzb
AdioSSqMpoSplvxJqk0KG+kOpH9XyPTQHEtjM1Gyy2r99MvUlDhF9/+PBy8+pfz0
ds8sYEnq67Z7EEnITIzc0NZK1EzlST6MbCtrkySOuxVDpkb/RHxBXYlZVYryI+Lf
FEVfBouvJRb9ucdCFD/p0J//uQ9HjuJOHTCpvlrRHcNssC321xeyIsuiusyEcnBB
fxaWAQJZVrQyvf3lZWgtPiRcb3mS3lo+/7OQWcPvhd0eKrpa9gIbnTbeMb2BQ+8L
hORagnrzYzYtMxQTVspF0ZuuAxRCzHT75A3aGhLA/Ron1nnfJaqmU+OsT1zi0Mhb
MQIDAQAB
-----END PUBLIC KEY-----
";

            var billerPrivateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCo9XtlJJSrZ8nv
8h4aXNsB2KhJKoymhKmW/EmqTQob6Q6kf1fI9NAcS2MzUbLLav30y9SUOEX3/48H
Lz6l/PR2zyxgSerrtnsQSchMjNzQ1krUTOVJPoxsK2uTJI67FUOmRv9EfEFdiVlV
ivIj4t8URV8Gi68lFv25x0IUP+nQn/+5D0eO4k4dMKm+WtEdw2ywLfbXF7Iiy6K6
zIRycEF/FpYBAllWtDK9/eVlaC0+JFxveZLeWj7/s5BZw++F3R4qulr2AhudNt4x
vYFD7wuE5FqCevNjNi0zFBNWykXRm64DFELMdPvkDdoaEsD9GifWed8lqqZT46xP
XOLQyFsxAgMBAAECggEADfJTF1DsmE6u0hVTwjK6l0C5kqFZaKAxv+qFib7mbNH3
j43wnDNtGRc1+I7JXSL9rh73Jkb6eGA9f61sLlc4T+hSmurvFW1QlwWBuNccN31A
28//HmUndTRX75aIbs2KC7TEYUEg3VlqXA/Ml56+C3ydPYguBgTYUjH69K3z6go9
TWwvPEiT6WBIPPnioU+ZHZvlSYp7yN2TK8LBqwG59P/ubMLIJ36ghR4YU5+vN7Mh
KkgCHsPp6q0aTrMSbeKyUgRkx15eTjQcAI43Hi/+nFq6M8XNk1ip03Mct8wFBN0j
wENwfxAeCHrgsSRFHaF6avFBMw9AzuF61yNB3WPQiwKBgQDlNscRswi0GR004VMH
4u2GBg6rzg/k6tHgH7ogVNpDg/Q5WX1EjRAqIPbpbHFneLsCs0N4BU8R9SosO7cz
CayrMG9nGXGL+iETRYnpt69QGMYwfUaFvhcr3ihwD7q7Qejw6DnUC1a3TPZKLyZr
cUDyGTG1H7c56ZCuwhKt7q8whwKBgQC8tBlW63OaZ/lQnwNnWS1mHh0XwUbiy5v3
J9S+KZF12O9UCsTQdb+cyYTjXmO4kG/E5DuwWGQwsxzDZqN4kLY12cgYzGqCpzJd
nAz+CxYrP2HVHoRt5LrkKCbTUaBDCf2e9xGJZ/7MeMzqyjrc6+O7bJgBKOCGQt8/
kNp1lvEchwKBgD4Su45bgbvkITi03JuCJPjqowZ742oG/ZdIgEtJL2KhVX5Ccd4i
pYIDM1q7d2qiE2MD0P2r0mH2ltkrws0bjZs+nqy5Azr5HgPuDQ8yI1P5oZJ4GqUV
eYjzvNe8KsGTc9Xpzd9SwsUZHomwgyMNpJzrnb6DPEd+rSPmgtB/lwn5AoGBAIS8
HnLgjeGXr2yBXbCNrvx8xDQYdRdE54Fz2BanQLVnkflI1eZYXR8ZNUuF8pk5qBUU
AdRqaJdE9j+Qa/57tF+uwCyJZYZfu3LTOORdwgtLuzJhFAAE+11PzPeqHBPr7CWs
Xv6LU1RayLGC7OLHXtpQaZ+vNDfcxBJ/fttmAFXzAoGAbfNxa3Yk0fFUxvtEeQr5
Jh9CSzZXMZ47+s3cetF5ZNhdYZNCYK7Md+ygFXO4auZMrlrU7nGWFCc6pzbPBajz
2R4WAL/xrArCb6rqZgQGoCnevX9LtTecUxgSghgc1N9oa/DRx3lMk6Y79P0P0U2X
Ml9+ZBa7ewKYCqjDwOeaSrQ=
-----END PRIVATE KEY-----
";

            var res= DigitalSignatureJsonHelper.VerifyJson(jsonData, pubKey, "Data");
            if (request.Data.User == "esadad2025test" && request.Data.Secret == "maliq&ifci-l#2%km_qbl4=)j")
            //if (request.Data.User == "esadad" && request.Data.Secret == "b7c9f182a1d5436f0cba9e47e6f21ab7f0d8c45b6d9e23fa41")
            {
                // var token = GenerateJwtToken(request.Data.User);
                var (token, expiryDate, tranTimestamp) = GenerateToken(request);
                var response = new AuthResponseDto
                {
                    Data = new AuthResponseDataDto
                    {
                        Token = token,
                        ExpiryDate = expiryDate,
                        TranTimestamp = tranTimestamp
                    },
                    Signature = ""
                };
                response.Signature = DigitalSignatureJsonHelper.SignJson(JsonHelper.ToJsonString(response), billerPrivateKey, "data");

                return Ok(response);
            }
            var errorResponse = new AuthResponseDto
            {
                Data = new AuthResponseDataDto
                {
                    Status = "error",
                    Message = "Invalid user credentials",
                    ErrorCode = 401,
                    TranTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                Signature = "" // Replace with your signature logic GenerateBillerSignature()
            };

            return Unauthorized(errorResponse);

        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected() => Ok("You have accessed a protected route");


        private (string Token, string ExpiryDate, string TranTimestamp) GenerateToken(AuthRequestDto request)
        {
            var transactionTimestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var tranTimestampIso = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var uuid = Guid.NewGuid().ToString();

            var rawToken = $"{request.Data.User}:{transactionTimestamp}:{uuid}";
            var hashedToken = ComputeSha256Hash(rawToken);

            var expiryDate = DateTime.ParseExact(transactionTimestamp, "yyyyMMddHHmmss", null)
                                      .AddDays(7)
                                      .ToUniversalTime()
                                      .ToString("yyyy-MM-ddTHH:mm:ssZ");

            return (hashedToken, expiryDate, tranTimestampIso);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(rawData);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
