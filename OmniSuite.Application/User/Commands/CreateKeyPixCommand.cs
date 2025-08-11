namespace OmniSuite.Application.User.Commands
{
    public record CreateKeyPixCommand(
        Guid? Id,
        string pixKeyValue, 
        string type,
        string beneficiaryName,
        string beneficiaryCPF
    ) : IRequest<Response<CreateKeyPixResponse>>;
}
