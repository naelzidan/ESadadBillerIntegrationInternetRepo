using Esadad.Infrastructure.DTOsJson;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.InterfacesJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace EsadadAPI.Controllers.Public
{
    [Route("public/prepaid")]
    [ApiController]
    public class PrepaidController : ControllerBase
    {
        // private readonly IBillPullService _billPullService;
        //IBillPullService billPullService,
        private readonly Esadad.Infrastructure.Interfaces.ICommonService _commonService;
        private readonly IPrepaidValidationJsonService _prepaidValidationService;
        public PrepaidController(IPrepaidValidationJsonService prepaidValidationService, Esadad.Infrastructure.Interfaces.ICommonService commonService)
        {
           _prepaidValidationService = prepaidValidationService;
            _commonService = commonService;
        }

        [HttpPost("PrepaidValidation")]
        public IActionResult PrepaidValidation([FromQuery(Name = "GUID")] Guid guid,
                                     [FromBody] PrePaidRequestDto prePaidRequestDto)
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

            string? billingNumber = prePaidRequestDto.MsgBody.BillingInfo.AcctInfo.BillingNo;
            string? serviceType = prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType;
            string? prepaidCat = prePaidRequestDto.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat;
            int validatioCode = prePaidRequestDto.MsgBody.BillingInfo.ValidationCode;

            //Log to EsadadTransactionsLogs Table
            var tranLog = _commonService.InsertLogJson(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), prePaidRequestDto);

            PrePaidResponseDto prePaidResponseDto = null;
            if (!DigitalSignatureJsonHelper.VerifyJson(JsonHelper.ToJsonString(prePaidRequestDto,true), pubKey, "msgBody"))
            {

                prePaidResponseDto = _prepaidValidationService.GetInvalidSignatureResponse(guid, billingNumber, serviceType, prepaidCat, validatioCode);

                return Ok(prePaidResponseDto);
            }
            else
            {
                //Log Response
                prePaidResponseDto = _prepaidValidationService.GetResponse(guid, prePaidRequestDto);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false

                };
                var x = JsonSerializer.Serialize(prePaidResponseDto, options);
                return Ok(prePaidResponseDto);
            }


            return Ok(new PrePaidResponseDto());





           // return Ok(prePaidRequestDto);

        }

    }
}
