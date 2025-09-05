using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OmniSuite.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DigitalProductStatusEnum
    {
        [Description("Ativo")]
        Active,

        [Description("Inativo")]
        Inactive,

        [Description("Rascunho")]
        Draft,

        [Description("Arquivado")]
        Archived
    }
}
