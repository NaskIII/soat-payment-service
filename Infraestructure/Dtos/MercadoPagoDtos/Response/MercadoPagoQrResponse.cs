using System.Text.Json.Serialization;

namespace Infraestructure.Dtos.MercadoPagoDtos.Response
{
    internal class MercadoPagoQrResponse
    {
        [JsonPropertyName("qr_data")]
        public string QrData { get; set; } = default!;

        [JsonPropertyName("in_store_order_id")]
        public string InStoreOrderId { get; set; } = default!;
    }
}
