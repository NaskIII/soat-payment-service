using Application.Dtos.MercadoPagoDtos.Request;
using Application.Interfaces;
using Domain.Enums;
using Infraestructure.Dtos.MercadoPagoDtos.Request;
using Infraestructure.Dtos.MercadoPagoDtos.Response;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Infraestructure.PaymentGAteway
{
    public class MercadoPagoGateway : IPaymentGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly string accessToken;
        private readonly string baseUrl;
        private readonly string notificationUrl;
        private readonly string userId;
        private readonly string posId;

        public MercadoPagoGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;


            accessToken = _configuration["MercadoPago:AccessToken"] ?? throw new ArgumentNullException(nameof(accessToken));
            baseUrl = _configuration["MercadoPago:BaseUrl"] ?? throw new ArgumentNullException(nameof(baseUrl));
            notificationUrl = _configuration["MercadoPago:WebhookUrl"] ?? throw new ArgumentNullException(nameof(notificationUrl));
            userId = _configuration["MercadoPago:UserId"] ?? throw new ArgumentNullException(nameof(userId));
            posId = _configuration["MercadoPago:PosId"] ?? throw new ArgumentNullException(nameof(posId));
        }

        public async Task<(string, string)> GeneratePaymentQrCodeAsync(PaymentRequestDto order)
        {
            string requestUrl = $"{baseUrl}/instore/orders/qr/seller/collectors/{userId}/pos/{posId}/qrs";

            var paymentRequest = new MercadoPagoQrRequest
            {
                ExternalReference = order.external_reference.ToString(),
                Title = $"Pedido para FastFood - {order.external_reference}",
                Description = $"Pedido para FastFood - {order.external_reference}",
                NotificationUrl = notificationUrl,
                TotalAmount = order.total_amount,
                Items = order.items.Select(item => new Dtos.MercadoPagoDtos.Request.MercadoPagoItem
                {
                    Title = item.Title,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalAmount = item.TotalAmount,
                    UnitMeasure = item.UnitMeasure
                }).ToList()
            };

            var jsonRequest = JsonSerializer.Serialize(paymentRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var doc = JsonSerializer.Deserialize<MercadoPagoQrResponse>(jsonResponse);
            if (doc == null)
                throw new Exception();

            return (doc.InStoreOrderId, doc.QrData);
        }

        //public async Task<PaymentStatus> GetPaymentStatusAsync(string externalPaymentId)
        //{
        //    string requestUrl = $"{baseUrl}/v1/payments/{externalPaymentId}";

        //    var httpClient = _httpClientFactory.CreateClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        //    var response = await httpClient.GetAsync(requestUrl);
        //    response.EnsureSuccessStatusCode();

        //    var jsonResponse = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(jsonResponse);
        //    string? statusFromMercadoPago = doc.RootElement.GetProperty("status").GetString();

        //    switch (statusFromMercadoPago?.ToLower())
        //    {
        //        case "approved":
        //        case "authorized":
        //            return PaymentStatus.Completed;

        //        case "in_process":
        //        case "pending":
        //            return PaymentStatus.Pending;

        //        case "rejected":
        //        case "charged_back":
        //            return PaymentStatus.Failed;
        //        case "cancelled":
        //            return PaymentStatus.Cancelled;
        //        case "refunded":
        //            return PaymentStatus.Refunded;

        //        default:
        //            return PaymentStatus.Failed;
        //    }
        //}

        public async Task<(PaymentStatus, string)> GetPaymentStatusAsync(string externalPaymentId)
        {
            string requestUrl = $"{baseUrl}/v1/payments/{externalPaymentId}";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            string statusFromMercadoPago = root.GetProperty("status").GetString()!;
            string externalReference = root.GetProperty("external_reference").GetString()!;

            PaymentStatus domainStatus;
            switch (statusFromMercadoPago?.ToLower())
            {
                case "approved": case "authorized": domainStatus = PaymentStatus.Completed; break;
                case "in_process": case "pending": domainStatus = PaymentStatus.Pending; break;
                default: domainStatus = PaymentStatus.Failed; break;
            }

            return (domainStatus, externalReference);
        }
    }
}
