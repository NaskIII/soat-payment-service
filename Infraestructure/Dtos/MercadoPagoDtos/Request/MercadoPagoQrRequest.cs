using System.Text.Json.Serialization;

namespace Infraestructure.Dtos.MercadoPagoDtos.Request
{
    public class MercadoPagoQrRequest
    {
        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; } = default!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("description")]
        public string Description { get ; set; } = default!;

        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; } = default!;

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("items")]
        public List<MercadoPagoItem> Items { get; set; } = default!;
    }

    public class MercadoPagoItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("unit_measure")]
        public string UnitMeasure { get; set; } = "unit";
    }
}
