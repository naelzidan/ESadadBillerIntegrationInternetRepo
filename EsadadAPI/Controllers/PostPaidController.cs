using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.Helpers;

using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Esadad.Infrastructure.Enums;

namespace EsadadAPI.Controllers
{
    [Route("postpaid")]
    [ApiController]
    [Consumes("application/xml")]
    [Produces("application/xml")]
    public class PostPaidController : ControllerBase
    {
        private readonly IBillPullService _billPullService;
        private readonly ICommonService _commonService;

        public PostPaidController(IBillPullService billPullService, ICommonService commonService)
        {
            _billPullService = billPullService;
            _commonService = commonService;
        }

        //[HttpGet]
        //public IActionResult Get()
        //{
        //   return Ok(MemoryCache.Biller);
        //}

        [HttpPost("BillPull")]
        public IActionResult BillPull([FromQuery(Name = "GUID")] Guid guid,
                                      [FromBody] XmlElement xmlElement,
                                      [FromQuery(Name = "username")] string? username = null,
                                      [FromQuery(Name = "password")] string? password = null)
        {
            // Log Request 
            string? billingNumber = xmlElement.SelectSingleNode("//BillingNo")?.InnerText;
            string? serviceType = xmlElement.SelectSingleNode("//ServiceType")?.InnerText;

            //Log to EsadadTransactionsLogs Table
            var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(), xmlElement);

            BillPullResponse billPullResponse = null;
            if (!DigitalSignature.VerifySignature(xmlElement))
            {
                
                billPullResponse = _billPullService.GetInvalidSignatureResponse(guid, billingNumber, serviceType);

                return Ok(billPullResponse);
            }
            else
            {
                //Log Response
                billPullResponse = _billPullService.BillPull(guid, xmlElement);
                return Ok(billPullResponse);
            }
        }
    }
}
