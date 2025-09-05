using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OmniSuite.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DigitalProductTypeEnum
    {
        [Description("E-book")]
        Ebook,

        [Description("Curso Online")]
        OnlineCourse,

        [Description("Software")]
        Software,

        [Description("Template")]
        Template,

        [Description("Plugin")]
        Plugin,

        [Description("Assinatura")]
        Subscription,

        [Description("Outros")]
        Other
    }
}
