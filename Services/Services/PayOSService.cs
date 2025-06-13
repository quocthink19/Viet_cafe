using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Repository.Models;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSSettings _payOSSettings;
        private readonly IUnitOfWork _unitOfWork;
        public PayOSService(IOptions<PayOSSettings> payOSSettings, IUnitOfWork unitOfWork)
        {
            _payOSSettings = payOSSettings.Value;
            _unitOfWork = unitOfWork;
        }
        public async Task<CreatePaymentResult> CreatePaymentUrl(Guid orderId)
        {
            PayOS payOS = new PayOS(_payOSSettings.ClientID, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);
            var order = await _unitOfWork.OrderRepo.GetById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            List<ItemData> items = new List<ItemData>();
            foreach (var orderDetail in order.OrderItems)
            {
                if (orderDetail == null)
                {
                    throw new Exception("danh sách đơn hàng đang null");
                }
                    ItemData item = new ItemData(
                  orderDetail.Name,
                  (int)orderDetail.Quantity,
                  (int)orderDetail.UnitPrice * (int)orderDetail.Quantity
                  );
                    items.Add(item);
            }
            Random random = new Random();
            var PaymentCode = $"POS{random.Next(1000000, 9999999)}";
            /*  var payment = new Payment
              {

              }*/
            return null;
        }
    public Task<PaymentLinkInformation> CancelOrder(string orderId, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentLinkInformation> GetPaymentInfo(int orderId)
        {
            throw new NotImplementedException();
        }

        public WebhookData VerifyWebhook(WebhookType webhook)
        {
            throw new NotImplementedException();
        }
    }
}
