namespace Esadad.Core.Models
{
    public class BillerService
    {
        public required string ServiceTypeCode { get; set; }
        
        public required string ServiceNameArabic { get; set; }
        
        public required string ServiceNameEnglish { get; set; }
        
        public required string PaymentType { get; set; }
        
        public required string Currency { get; set; }
        
        public required string IBAN { get; set; }

        public int BankCode { get; set; }
        
        public bool PartialPayment { get; set; }

        public decimal MinimumAmount { get; set; }
    }
}