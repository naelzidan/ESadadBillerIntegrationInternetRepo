

using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOsJson
{
    public class PrePaidRequestDto
    {
      //  [JsonPropertyName("MsgHeader")]
        public PrepaidMsgHeader MsgHeader { get; set; }

       // [JsonPropertyName("MsgBody")]
        public PrePaidRequestBody MsgBody { get; set; }

       // [JsonPropertyName("MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }

    public class PrePaidRequestBody
    {
       // [JsonPropertyName("BillingInfo")]
        public PrepaidRequestBillingInfo BillingInfo { get; set; }
    }

    public class PrepaidRequestBillingInfo
    {
        //[JsonPropertyName("AcctInfo")]
        public PrepaidAcctInfo AcctInfo { get; set; }

        //[XmlElement(ElementName = "DueAmt")]
        public decimal DueAmt { get; set; }

        // [JsonPropertyName("ValidationCode")]
        public int ValidationCode { get; set; }

        //[JsonPropertyName("ServiceTypeDetails")]
        public ServiceTypeDetails ServiceTypeDetails { get; set; }

        //[JsonPropertyName("PayerInfo")]
        public PayerInfo PayerInfo { get; set; }
    }

    public class PrepaidMsgHeader
    {
       // [JsonPropertyName("TmStp")]
        public DateTime TmStp { get; set; }

       // [JsonPropertyName("GUID")]
        public Guid GUID { get; set; }

       // [JsonPropertyName("TrsInf")]
        public PrepaidTrsInf TrsInf { get; set; }
    }

    public class PrepaidTrsInf
    {
       // [JsonPropertyName("RcvCode")]
        public int RcvCode { get; set; }

        //[JsonPropertyName("ReqTyp")]
        public string ReqTyp { get; set; }
    }



}
