using Esadad.Core.Entities;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.DTOsJson;

using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using System.Xml;

namespace Esadad.Infrastructure.Services
{
    public class CommonService(EsadadIntegrationDbContext context) : ICommonService
    {
        private readonly EsadadIntegrationDbContext _context = context;

        public EsadadTransactionLog InsertLog(string transactionType, string apiName, string guid, XmlElement xmlElement, Object responseObject=null)
        {
           
            //BillPullRequest billPullRequestObj = null;
            EsadadTransactionLog esadadTransactionLog = null;

           
            //PaymentNotificationRequestDto paymentNotificationRequestDtoObj = null;

            if (transactionType.ToLower() == "request")
            {
                if (apiName == "BillPull")
                {
                    var billPullRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new BillPullRequest());
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = billPullRequestObj.MsgHeader.TmStp,
                        BillingNumber = billPullRequestObj.MsgBody.AcctInfo.BillingNo,
                        BillNumber = billPullRequestObj.MsgBody.AcctInfo.BillNo,
                        ServiceType = billPullRequestObj.MsgBody.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullRequestObj.MsgBody.ServiceType).Currency,
                        TranXmlElement = xmlElement.OuterXml
                    };

                }
                else if (apiName == "ReceivePaymentNotification")
                {
                    var paymentNotificationRequestDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationRequestDto());
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = paymentNotificationRequestDtoObj.MsgHeader.TmStp,
                        BillingNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillingNo,
                        BillNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                        ServiceType = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType).Currency,
                        ValidationCode = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                        PrepaidCat= paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.PrepaidCat,
                        TranXmlElement = xmlElement.OuterXml
                    };
                }
                else if (apiName == "PrepaidValidation")
                {
                    var prepaidValidationRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new DTOs.PrePaidRequestDto());
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = prepaidValidationRequestObj.MsgHeader.TmStp,
                        BillingNumber = prepaidValidationRequestObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                        ServiceType = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                        PrepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                        TranXmlElement = xmlElement.OuterXml
                    };

                }

            }
            else if (transactionType.ToLower() == "response" && xmlElement != null)
                {
                if (apiName == "BillPull")
                {
                    var billPullResponseObj = (BillPullResponse) responseObject;
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = billPullResponseObj.MsgHeader.TmStp,
                        BillingNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillingNo,
                        BillNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillNo,
                        ServiceType = billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType).Currency,
                        TranXmlElement = xmlElement.OuterXml
                    };

                }
                else if (apiName == "ReceivePaymentNotification")
                {
                    var paymentNotificationResponseDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationResponseDto());
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = paymentNotificationResponseDtoObj.MsgHeader.TmStp,                       
                        TranXmlElement = xmlElement.OuterXml
                    };

                }
                else if (apiName == "PrepaidValidation")
                {
                    var prepaidValidationResponseObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new DTOs.PrePaidResponseDto());
                    esadadTransactionLog = new EsadadTransactionLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = prepaidValidationResponseObj.MsgHeader.TmStp,
                        BillingNumber = prepaidValidationResponseObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                        ServiceType = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                        PrepaidCat = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                        TranXmlElement = xmlElement.OuterXml
                    };

                }


            }

            var query = _context.EsadadTransactionsLogs.Add(esadadTransactionLog).Entity;

            _context.SaveChanges();
            return query;
        }

        public EsadadTransactionJsonLog InsertLogJson(string transactionType, string apiName, string guid, object jsonObject, Object responseObject = null)
        {

            //BillPullRequest billPullRequestObj = null;
            EsadadTransactionJsonLog esadadTransactionJsonLog = null;


            //PaymentNotificationRequestDto paymentNotificationRequestDtoObj = null;

            if (transactionType.ToLower() == "request")
            {
                if (apiName == "BillPull")
                {
                    var billPullRequestObj = ObjectCasterHelper.CastTo<BillPullRequest>(jsonObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = billPullRequestObj.MsgHeader.TmStp,
                        BillingNumber = billPullRequestObj.MsgBody.AcctInfo.BillingNo,
                        BillNumber = billPullRequestObj.MsgBody.AcctInfo.BillNo,
                        ServiceType = billPullRequestObj.MsgBody.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullRequestObj.MsgBody.ServiceType).Currency,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };

                }
                else if (apiName == "ReceivePaymentNotification")
                {

                    //var paymentNotificationRequestDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationRequestDto());
                    var paymentNotificationRequestDtoObj = ObjectCasterHelper.CastTo<PaymentNotificationRequestDto>(jsonObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = paymentNotificationRequestDtoObj.MsgHeader.TmStp,
                        BillingNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillingNo,
                        BillNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                        ServiceType = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType).Currency,
                        ValidationCode = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                        PrepaidCat = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.PrepaidCat,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };
                }
                else if (apiName == "PrepaidValidation")
                {
                  //  var prepaidValidationRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidRequestDto());
                    var prepaidValidationRequestObj = ObjectCasterHelper.CastTo<DTOsJson.PrePaidRequestDto>(jsonObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = prepaidValidationRequestObj.MsgHeader.TmStp,
                        BillingNumber = prepaidValidationRequestObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                        ServiceType = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                        PrepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };

                }

            }
            else if (transactionType.ToLower() == "response" && jsonObject != null)
            {
                if (apiName == "BillPull")
                {
                    //var billPullResponseObj = (BillPullResponse)responseObject;
                    var billPullResponseObj = ObjectCasterHelper.CastTo<BillPullResponse>(jsonObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = billPullResponseObj.MsgHeader.TmStp,
                        BillingNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillingNo,
                        BillNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillNo,
                        ServiceType = billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType,
                        Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType).Currency,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };

                }
                else if (apiName == "ReceivePaymentNotification")
                {
                    //var paymentNotificationResponseDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationResponseDto());
                    var paymentNotificationResponseDtoObj = ObjectCasterHelper.CastTo<PaymentNotificationResponseDto>(jsonObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = paymentNotificationResponseDtoObj.MsgHeader.TmStp,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };

                }
                else if (apiName == "PrepaidValidation")
                {
                  //  var prepaidValidationResponseObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidResponseDto());
                    var prepaidValidationResponseObj = ObjectCasterHelper.CastTo<DTOsJson.PrePaidResponseDto>(responseObject);
                    esadadTransactionJsonLog = new EsadadTransactionJsonLog
                    {
                        TransactionType = transactionType,
                        ApiName = apiName,
                        Guid = guid,
                        Timestamp = prepaidValidationResponseObj.MsgHeader.TmStp,
                        BillingNumber = prepaidValidationResponseObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                        ServiceType = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                        PrepaidCat = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                        TranJson = JsonHelper.ToJsonString(jsonObject)
                    };

                }


            }

            var query = _context.EsadadTransactionsJsonLogs.Add(esadadTransactionJsonLog).Entity;

            _context.SaveChanges();
            return query;
        }


        EsadadPaymentLog ICommonService.InsertPaymentLog(string transactionType, string apiName, string guid, XmlElement requestElement)
        {
            throw new NotImplementedException();
        }
    }
}