namespace OmniSuite.Application.User.Responses
{
    public record UserPixResponse(
        Guid Id, 
        string PixKeyValue, 
        string Type, 
        string BeneficiaryName,
        string BeneficiaryCPF
    );
}
