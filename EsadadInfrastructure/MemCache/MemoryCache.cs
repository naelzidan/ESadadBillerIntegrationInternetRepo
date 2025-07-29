using Esadad.Core.Models;

namespace Esadad.Infrastructure.MemCache
{
    public static class MemoryCache
    {
        public static Biller Biller = new Biller();
 
        public static Certificates Certificates = new Certificates();

        public static Dictionary<string,int> Currencies = new Dictionary<string, int>();
    }
}