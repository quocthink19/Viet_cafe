using Azure.Core;
using Microsoft.Extensions.Options;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSSettings _settings;
        private readonly HttpClient _httpClient;

        public PayOSService(IOptions<PayOSSettings> options)
        {
          ;
            _settings = options.Value;
            Console.WriteLine($"[PayOSService] ClientId: {_settings.ClientId}");
            Console.WriteLine($"[PayOSService] ApiKey: {_settings.ApiKey}");
            Console.WriteLine($"[PayOSService] ChecksumKey: {_settings.ChecksumKey}");
            _httpClient = new HttpClient();
           // _httpClient.BaseAddress = new Uri("https://api-merchant.payos.vn/");
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
        }

        public async Task<string> CreatePaymentAsync(PaymentRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            Console.WriteLine($"[PayOSService] Request JSON: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", content);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[PayOSService] Response: {body}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Yêu cầu thất bại: {response.StatusCode} - {body}");
            }

            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("checkoutUrl", out var urlElement))
            {
                return urlElement.GetString();
            }
            else
            {
                throw new Exception("Không lấy được checkoutUrl từ phản hồi PayOS.");
            }
        }



        public string GetChecksumKey() => _settings.ChecksumKey;
    }
}