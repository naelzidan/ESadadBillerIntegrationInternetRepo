using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.DTOsJson
{
    public class PaymentNotificationRequestDto
    {
        // [XmlElement(ElementName = "MsgHeader")]
        public RequestMsgHeader MsgHeader { get; set; }

        //[XmlElement(ElementName = "MsgBody")]
        public PaymNotificationRequestRequestMsgBody MsgBody { get; set; }
        // [XmlElement(ElementName = "MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }


    public class PaymNotificationRequestRequestMsgBody
    {
        public PaymNotificationRequestTransactions Transactions { get; set; }
    }

    public class PaymNotificationRequestTransactions
    {
        public PaymNotificationRequestTrxInf TrxInf { get; set; }
    }

    public class PaymNotificationRequestTrxInf
    {
        public PaymNotificationAcctInfo AcctInfo { get; set; }
        public long JOEBPPSTrx { get; set; } // Mandatory, Integer, Long
        public string BankTrxID { get; set; } // Mandatory, String, No length restriction provided
        public int BankCode { get; set; } // Mandatory, Integer, Up to 3 digits
        public string PmtStatus { get; set; } // Optional, String
        public decimal DueAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public decimal PaidAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public decimal FeesAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public bool FeesOnBiller { get; set; } // Boolean, Yes/No
        public DateTime ProcessDate { get; set; } // Mandatory
        [JsonIgnore] // System.Text.Json
        public DateTime STMTDate { get; set; } // Mandatory

        [JsonPropertyName("STMTDate")]
        public string STMTDateJson
        {
            get => STMTDate.ToString("yyyy-MM-dd");
            set => STMTDate = DateTime.Parse(value);
        }
        public string AccessChannel { get; set; } // Optional, Enum, Up to 15 chars
        public string PaymentMethod { get; set; } // Optional, String, Up to 15 chars
        public string PaymentType { get; set; } // Optional, String, Up to 15 chars
        public string Currency { get; set; } // Optional, String, Up to 3 chars
        public ServiceTypeDetails ServiceTypeDetails { get; set; }
        public SubPmts SubPmts { get; set; }
    }

    public class PaymNotificationAcctInfo
    {
 
        public string BillingNo { get; set; }
        public string BillNo { get; set; }
        public int BillerCode { get; set; }
    }
}
