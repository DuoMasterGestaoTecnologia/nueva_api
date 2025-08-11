namespace OmniSuite.Application.User.Responses
{
    public class CreateUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }

        public CreateUserResponse(Guid id, string email, string nome)
        {
            Id = id;
            Email = email;
            Nome = nome;
        }
    }
}
