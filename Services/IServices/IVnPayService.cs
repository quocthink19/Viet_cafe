using Microsoft.AspNetCore.Http;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;


namespace Services.IServices
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequest vnPayRequest);
        Task<string> CreatePaymentUrlAsync(HttpContext context, VnPayRequest vnPayRequest);
        VnPayResponseDTO PaymentExcute(IQueryCollection collection);
    }
}
