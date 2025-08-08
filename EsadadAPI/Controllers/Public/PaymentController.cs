using Esadad.Infrastructure.DTOsJson;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.InterfacesJson;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace EsadadAPI.Controllers.Public
{
    [Route("public/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentNotificationJsonService _paymentNotificationService;
        private readonly ICommonService _commonService;

        public PaymentController(IPaymentNotificationJsonService paymentNotificationService, ICommonService commonService)
        {
            _paymentNotificationService = paymentNotificationService;
            _commonService = commonService;
        }

        [HttpPost("ReceivePaymentNotification")]
        public IActionResult ReceivePaymentNotification([FromQuery(Name = "GUID")] Guid guid,
                                                        [FromBody] PaymentNotificationRequestDto paymentNotificationRequestDto)
        {
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

            string? billingNumber = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.AcctInfo.BillingNo;
            string? serviceType = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType;
            string? prepaidCat = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.ServiceTypeDetails.PrepaidCat;
           // int validatioCode = prePaidRequestDto.MsgBody.BillingInfo.ValidationCode;


            var tranLog = _commonService.InsertLogJson(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(), paymentNotificationRequestDto);

            var paymLog = _commonService.InsertPaymentLog(guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(paymentNotificationRequestDto));

            Esadad.Infrastructure.DTOs.PaymentNotificationResponseDto paymentNotificationResponse;

            if (!DigitalSignatureJsonHelper.VerifyJson(JsonHelper.ToJsonString(paymentNotificationRequestDto, true), pubKey, "msgBody"))
            {
                //return Ok(new PaymentNotificationResponseDto());
                paymentNotificationResponse = _paymentNotificationService.GetInvalidSignatureResponse(guid, billingNumber, serviceType);
                return Ok(paymentNotificationResponse);
            }

            var requestTrxInfo = new Esadad.Infrastructure.DTOs.PaymentNotificationResponseTrxInf()
            {
                JOEBPPSTrx = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.JOEBPPSTrx.ToString(),
                ProcessDate = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.ProcessDate,
                STMTDate = paymentNotificationRequestDto.MsgBody.Transactions.TrxInf.STMTDate.ToString()
            };

            paymentNotificationResponse = _paymentNotificationService.GetPaymentNotificationResponse(guid, billingNumber, serviceType, requestTrxInfo);
            return Ok(paymentNotificationResponse);

            //return Ok(new Esadad.Infrastructure.DTOs.PaymentNotificationResponseDto());
        }
    }
}