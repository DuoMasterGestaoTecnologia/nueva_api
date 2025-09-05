using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OmniSuite.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DigitalProductPurchaseStatusEnum
    {
        [Description("Pendente")]
        Pending,

        [Description("Pago")]
        Paid,

        [Description("Entregue")]
        Delivered,

        [Description("Cancelado")]
        Cancelled,

        [Description("Reembolsado")]
        Refunded
    }
}
