using Esadad.Infrastructure.DTOs;
using System.Xml;

namespace Esadad.Infrastructure.Interfaces
{
    public interface IBillPullService
    {
        public BillPullResponse BillPull(Guid guid, XmlElement xmlElement);

        public BillPullResponse GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType);
    }
}