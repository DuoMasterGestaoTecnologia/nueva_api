using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniSuite.Infrastructure.Services.FlowpagService.Responses
{
    using System.Text.Json.Serialization;

    public class WithdrawPixRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("receiver")]
        public ReceiverInfo Receiver { get; set; }

        [JsonPropertyName("pix")]
        public PixInfo Pix { get; set; }
    }

    public class ReceiverInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("document")]
        public string Document { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class PixInfo
    {
        [JsonPropertyName("pix_type")]
        public string PixType { get; set; }

        [JsonPropertyName("pix_key")]
        public string PixKey { get; set; }
    }

    public class WithdrawPixResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("payment")]
        public WithdrawPixPayment Payment { get; set; }
    }

    public class WithdrawPixPayment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

}
