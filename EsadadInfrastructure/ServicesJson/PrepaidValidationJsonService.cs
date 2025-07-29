using Esadad.Infrastructure.DTOsJson;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.InterfacesJson;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Esadad.Infrastructure.ServicesJson
{
    public class PrepaidValidationJsonService(EsadadIntegrationDbContext context, Interfaces.ICommonService commonService) : IPrepaidValidationJsonService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly Interfaces.ICommonService _commonService = commonService;

        string billerPrivateKey = @"-----BEGIN PRIVATE KEY-----
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
        public PrePaidResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode)
        {
            try
            {

                PrePaidResponseDto response = new PrePaidResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILRPREPADVALRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new PrePaidResponseBody()
                    {
                        BillingInfo = new BillingInfo()
                        {
                             Result = new Result()
                             {
                                 ErrorCode = 2,
                                 ErrorDesc = "Invalid Signature",
                                 Severity = "Error"
                             },
                             AcctInfo = new PrepaidAcctInfo()
                             {
                                 BillingNo = billingNumber,
                                 BillerCode = MemoryCache.Biller.Code
                             },
                             DueAmt=0,
                             Currency= MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency,
                             ValidationCode = validationCode,
                             ServiceTypeDetails = new ServiceTypeDetails()
                             {
                                  ServiceType= serviceType
                             },
                             SubPmts = new SubPmts()
                             {
                                 SubPmt = new SubPmt()
                                 {
                                     Amount = CurrencyHelper.AdjustDecimal(0, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate),
                                     SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).BankCode,
                                     AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).IBAN
                                 }
                             }
                        }
                    }
                };

                if(prepaidCat != null || prepaidCat != "")
                {
                    response.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat = prepaidCat;
                }

             

                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };
                response.MsgFooter = msgFooter;

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(response), response);

                return response;
            }
            catch
            {
                throw;
            }
        }


        public PrePaidResponseDto GetResponse(Guid guid, PrePaidRequestDto prePaidRequestDto)
        {
            try
            {

                //var prepaidValidationRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidRequestDto());


                PrePaidResponseDto response = new PrePaidResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILRPREPADVALRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new PrePaidResponseBody()
                    {
                        BillingInfo = new BillingInfo()
                        {
                            Result = new Result()
                            {
                                ErrorCode = 0,
                                ErrorDesc = "Success",
                                Severity = "Info"
                            },
                            AcctInfo = new PrepaidAcctInfo()
                            {
                                BillingNo = prePaidRequestDto.MsgBody.BillingInfo.AcctInfo.BillingNo,
                                BillerCode = MemoryCache.Biller.Code
                            },
                            DueAmt = prePaidRequestDto.MsgBody.BillingInfo.DueAmt,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).Currency,
                            ValidationCode = prePaidRequestDto.MsgBody.BillingInfo.ValidationCode,
                            ServiceTypeDetails = new ServiceTypeDetails()
                            {
                                ServiceType = prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType
                            },
                            SubPmts = new SubPmts()
                            {
                                SubPmt = new SubPmt()
                                {

                                    // rertrive service type category value (Replace 0)
                                    Amount = CurrencyHelper.AdjustDecimal(prePaidRequestDto.MsgBody.BillingInfo.DueAmt, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).Currency], DecimalAdjustment.Truncate),
                                    SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).BankCode,
                                    AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).IBAN
                                }
                            },
                             AdditionalInfo= new AdditionalInfo()
                             {
                                 CustName= prePaidRequestDto.MsgBody.BillingInfo.AcctInfo.BillingNo,
                                 FreeText="عزيزي المستخدم، شكرا لاستخدامك ESadad. سيتم ارسال رسالة تاكيد عملةية الشحن لرقم المحمول. في حال الخصم وعدم وصول التأكيد يرجى التواصل على رقم 022123456."
                             }
                        }
                    }
                };

                if (prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat != null )
                {
                    response.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat = prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat;
                }


                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignatureJsonHelper.SignJson(JsonHelper.ToJsonString(response,true), billerPrivateKey,"msgBody")
                    }
                };
                response.MsgFooter = msgFooter;

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLogJson(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), JsonHelper.ToJsonString(response), response);

                return response;
            }
            catch
            {
                throw;
            }

        }

    }
}
