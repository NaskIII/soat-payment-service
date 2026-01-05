using System.Text.Json.Serialization;

namespace Application.Dtos.WebhookDtos.Request
{
    public class MercadoPagoWebhookRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("data")]
        public WebhookData? Data { get; set; }

        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("resource")]
        public string? Resource { get; set; }
    }

    public class WebhookData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}