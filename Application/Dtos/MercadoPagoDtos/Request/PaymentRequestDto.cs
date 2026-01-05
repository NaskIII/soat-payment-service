using System.Text.Json.Serialization;

namespace Application.Dtos.MercadoPagoDtos.Request
{
    public record PaymentRequestDto(
            Guid external_reference,
            string title,
            decimal total_amount,
            string? notification_url,
            List<MercadoPagoItem> items
        );

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
