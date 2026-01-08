using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infraestructure.OrderGateway
{
    public class UpdateOrderStatusGateway : IUpdateOrderStatusGateway 
    {

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        public UpdateOrderStatusGateway(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus orderStatus)
        {
            var orderRequest = new
            {
                OrderId = orderId,
                OrderStatus = orderStatus
            };

            var token = await Authenticate();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"/api/v1/order/{orderId}/status", orderRequest);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> Authenticate()
        {
            var request = new
            {
                clientId = _configuration["OrderService:ClientId"],
                clientSecret = _configuration["OrderService:ClientSecret"]
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/service-account/authenticate", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            return content!["token"];
        }
    }
}
