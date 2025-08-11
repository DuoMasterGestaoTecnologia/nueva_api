public record RegisterCommand(
    string Name,
    string Email,
    string Password,
    string? Document,
    string? Phone
) : IRequest<Response<CreateUserResponse>>;
