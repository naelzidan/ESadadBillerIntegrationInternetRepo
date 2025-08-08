using Azure;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.InterfacesJson;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Esadad.Infrastructure.ServicesJson
{
    public class PaymentNotificationJsonService(EsadadIntegrationDbContext context, ICommonService commonService) : IPaymentNotificationJsonService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly ICommonService _commonService = commonService;
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


        public PaymentNotificationResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType)
        {
            try
            {

                PaymentNotificationResponseDto response = new PaymentNotificationResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BLRPMTNTFRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 2,
                            ErrorDesc = "Invalid Signature",
                            Severity = "Error"
                        }
                    },
                    MsgBody = new PaymentNotificationResponseBody() { }
                };

                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        //Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };

                response.MsgFooter = msgFooter;

                //Log to EsadadTransactionsLogs Table
                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(response), response);

                return response;
            }
            catch
            {
                throw;
            }
        }

        public PaymentNotificationResponseDto GetPaymentNotificationResponse(Guid guid,
                                                                          string billingNumber,
                                                                          string serviceType,
                                                                          PaymentNotificationResponseTrxInf paymentNotificationRequestTrxInf)
        {
            try
            {

                // Payment Procedure 

                /* 
                   - Fill PaymentNotificationResponseDto Object
                   - check if theer is record availabel at paymentLog (Guid and PayemntPosted = true)

                        - Available 
                            - Log Response and return result 

                        - Not available 
                            - call stored procedure to add fees to student accounting system (guid, studentNo, servicetype)
                            - ensure  stored procedure on the end update  table [EsadadPaymentsLogs]= true
                            - log Reponse and return result  
                
                 */



                PaymentNotificationResponseDto paymentNotificationResponseDto;
                paymentNotificationResponseDto = new()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BLRPMTNTFRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new PaymentNotificationResponseBody()
                    {
                        Transactions = new PaymentNotificationResponseTransactions()
                        {
                            TrxInf = new PaymentNotificationResponseTrxInf()
                            {
                                JOEBPPSTrx = paymentNotificationRequestTrxInf.JOEBPPSTrx,
                                ProcessDate = paymentNotificationRequestTrxInf.ProcessDate,
                                STMTDate = paymentNotificationRequestTrxInf.STMTDate,
                                Result = new Result()
                                {
                                    ErrorCode = 0,
                                    ErrorDesc = "Success",
                                    Severity = "Info"
                                }
                            }
                        }
                    }
                };

                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        //Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(paymentNotificationResponseDto))
                    }
                };

                paymentNotificationResponseDto.MsgFooter = msgFooter;

                var existing = _context.EsadadPaymentsLogs
                                       .FirstOrDefault(a => a.Guid.Equals(guid.ToString())
                                                          && a.IsPaymentPosted == true);

                if (existing != null)
                {
                    _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(),
                                                    guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(paymentNotificationResponseDto));
                    return paymentNotificationResponseDto;
                }


                using var transaction = _context.Database.BeginTransaction();

                try
                {

                    bool isPaymentAdded = false;
                    /*
                    isPaymentAdded = Call your Internal System for adding the reflecting/ adding the payment                  
                    */

                    if (isPaymentAdded)
                    {
                        var esadadPaymentsLogLatest = _context.EsadadPaymentsLogs
                            .Where(a => a.Guid == guid.ToString())
                            .OrderByDescending(a => a.InsertDate)
                            .FirstOrDefault();

                        if (esadadPaymentsLogLatest != null)
                        {
                            esadadPaymentsLogLatest.IsPaymentPosted = true;
                            _context.SaveChanges();
                        }

                        _commonService.InsertLog(
                            TransactionTypeEnum.Response.ToString(),
                            ApiTypeEnum.ReceivePaymentNotification.ToString(),
                            guid.ToString(),
                            ObjectToXmlHelper.ObjectToXmlElement(paymentNotificationResponseDto)
                        );

                        // Commit transaction only if everything succeeded
                        transaction.Commit();
                        return paymentNotificationResponseDto;
                    }
                    else
                    {
                        transaction.Rollback();
                        throw new Exception("Payment Not added");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}