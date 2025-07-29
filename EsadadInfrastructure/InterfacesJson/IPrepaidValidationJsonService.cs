using Esadad.Infrastructure.DTOsJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Esadad.Infrastructure.InterfacesJson
{
    public interface IPrepaidValidationJsonService
    {
        public PrePaidResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode);
        public PrePaidResponseDto GetResponse(Guid guid, PrePaidRequestDto prePaidRequestDto);

    }
}
