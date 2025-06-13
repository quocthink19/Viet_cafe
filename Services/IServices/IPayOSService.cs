using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentUrl(long orderId);

        WebhookData VerifyWebhook(WebhookType webhook);
        Task<PaymentLinkInformation> GetPaymentInfo(long orderId);
        Task<PaymentLinkInformation> CancelOrder(string orderId, string reason);
    }
}
