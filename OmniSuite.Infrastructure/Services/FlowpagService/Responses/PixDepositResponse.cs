using System.Text.Json.Serialization;

namespace OmniSuite.Infrastructure.Services.FlowpagService.Responses
{

    public class PixDepositResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("payment")]
        public PixPayment Payment { get; set; }
    }

    public class PixPayment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public string PixCopyPasteCode { get; set; }

        [JsonPropertyName("expiration")]
        public int Expiration { get; set; }

        [JsonPropertyName("image_base64")]
        public string QrCodeBase64 { get; set; }
    }

}
