using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Repository.Models;
using Repository.Models.Enum;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
        public async Task<CreatePaymentResult> CreatePaymentUrl(long orderId)
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
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Code = PaymentCode,
                Amount = order.FinalPrice,
                Method = (int)Method.PAYOS,
                OrderId = order.Id,
                Description = "Thanh toán của đơn hàng " + order.Id.ToString(),
                Status = PaymentStatus.UNPAID,
                TransactionIdResponse = null , 
            };
            await _unitOfWork.PaymentRepo.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            long expiredAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 600;

            var paymentData = new PaymentData(
                order.Id,
                (int)payment.Amount,
                "Thanh toán đơn hàng " +order.Id ,
                items, 
                _payOSSettings.CancelUrl,
                _payOSSettings.ReturnUrl,
                 null, null, null, null, null,
                expiredAt);

            var createPayment = await payOS.createPaymentLink(paymentData);
            payment.TransactionIdResponse = createPayment.paymentLinkId.ToString();
            await _unitOfWork.SaveAsync();
            return createPayment;
        }
    public async Task<PaymentLinkInformation> CancelOrder(string orderId, string reason)
        {
            try
            {
                PayOS payOS = new PayOS(_payOSSettings.ClientID, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);

                int orderCode = int.Parse(orderId);
                PaymentLinkInformation paymentLinkInformation = await payOS.cancelPaymentLink(orderCode, reason);
                return paymentLinkInformation;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
    }

        public async  Task<PaymentLinkInformation> GetPaymentInfo(long orderId)
        {
            PayOS payOS = new PayOS(_payOSSettings.ClientID, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);

            return await payOS.getPaymentLinkInformation(orderId);
        }

        public WebhookData VerifyWebhook(WebhookType webhook)
        {
            PayOS payOS = new PayOS(_payOSSettings.ClientID, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);

            return payOS.verifyPaymentWebhookData(webhook);
        }
    }
}
