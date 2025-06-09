using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using Services.IServices;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PaymentBusiness
    {
        private readonly IPayOSService _payOSService;
        private readonly PayOSSettings _settings;

        public PaymentBusiness(IPayOSService payOSService, PayOSSettings settings)
        {
            _payOSService = payOSService;
            _settings = settings;
        }

        public async Task<string> GetPaymentUrlAsync(long amount, long orderId)
        {
            var description = $"Thanh toan don hang {orderId}"; // ✅ Tạo ở đây 1 lần duy nhất

            var dto = new PaymentRequest
            {
                amount = amount,
                description = description,        // ✅ Gán đúng cái vừa tạo
                orderCode = orderId,
                returnUrl = "https://viet-cafe.onrender.com/payment-success",
                cancelUrl = "https://viet-cafe.onrender.com/payment-cancel"
            };

            dto.signature = CreateSignature(dto); // ✅ Bây giờ signature khớp hoàn toàn với JSON
            return await _payOSService.CreatePaymentAsync(dto);
        }

        public string GetChecksumKey()
        {
            return _payOSService.GetChecksumKey();
        }
        public static long GuidToLong(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }
        private string CreateSignature(PaymentRequest request)
        {
            var rawData = $"{request.amount}{request.description}{request.orderCode}{request.returnUrl}{request.cancelUrl}";

            Console.WriteLine($"[DEBUG] RawData to HMAC: {rawData}");

            var keyBytes = Encoding.UTF8.GetBytes(_settings.ChecksumKey);
            var rawBytes = Encoding.UTF8.GetBytes(rawData);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(rawBytes);
            var signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            Console.WriteLine($"[DEBUG] Computed Signature: {signature}");

            return signature;
        }
        public async Task ProcessWebhookAsync(WebhookPayload payload)
        {
           
            Console.WriteLine($"[Webhook] Order {payload.orderCode} - Status: {payload.status}");
            await Task.CompletedTask;
        }
    }
}