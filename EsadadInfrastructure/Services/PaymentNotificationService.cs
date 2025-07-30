using Azure;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Esadad.Infrastructure.Services
{
    public class PaymentNotificationService(EsadadIntegrationDbContext context, ICommonService commonService) : IPaymentNotificationService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly ICommonService _commonService = commonService;

        public PaymentNotificationResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, XmlElement xmlElement)
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
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };

                response.MsgFooter = msgFooter;

                //Log to EsadadTransactionsLogs Table
                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(), xmlElement);

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
                                                                          PaymentNotificationResponseTrxInf paymentNotificationRequestTrxInf,
                                                                          XmlElement xmlElement)
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
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(paymentNotificationResponseDto))
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