using System.Text.Json.Serialization;

namespace OmniSuite.Infrastructure.Services.FlowpagService.Request
{
    public class PixDepositRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("expiration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Expiration { get; set; }

        [JsonPropertyName("reject_unknown_payer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RejectUnknownPayer { get; set; } = false;

        [JsonPropertyName("payer")]
        public PayerInfo Payer { get; set; }
    }

    public class PayerInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("document")]
        public string Document { get; set; }

        [JsonPropertyName("phone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }

        [JsonPropertyName("address")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PayerAddress Address { get; set; }
    }

    public class PayerAddress
    {
        [JsonPropertyName("street")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Street { get; set; }

        [JsonPropertyName("number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Number { get; set; }

        [JsonPropertyName("complement")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Complement { get; set; }

        [JsonPropertyName("district")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string District { get; set; }

        [JsonPropertyName("city")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string City { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PostalCode { get; set; }
    }
}
