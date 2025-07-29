using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Infrastructure.DTOs.Public
{
    public class AuthRequestDto
    {
        public AuthRequestData Data { get; set; }
        public string Signature { get; set; }
    }

    public class AuthRequestData
    {
        public string User { get; set; }
        public string Secret { get; set; }
        public string TranTimestamp { get; set; }
    }
}
