namespace Esadad.Core.Models
{
    public class Biller
    {
        public int Code { get; set; }

        public List<BillerService> Services { get; set; }
    }
}