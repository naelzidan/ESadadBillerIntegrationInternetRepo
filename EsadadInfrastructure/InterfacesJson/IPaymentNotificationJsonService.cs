using Esadad.Infrastructure.DTOs;
using System.Xml;

namespace Esadad.Infrastructure.InterfacesJson
{
    public interface IPaymentNotificationJsonService
    {
        public PaymentNotificationResponseDto GetPaymentNotificationResponse(Guid guid, string billingNumber, string serviceType, PaymentNotificationResponseTrxInf paymentNotificationRequestTrxInf);

        public PaymentNotificationResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType);
    }
}