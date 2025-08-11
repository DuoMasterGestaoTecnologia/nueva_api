namespace OmniSuite.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserStatusEnum
    {
        [Description("Inativado")]
        inactive,

        [Description("Cadastrado")]
        registered,

        [Description("Aprovado")]
        approved
    }
}
