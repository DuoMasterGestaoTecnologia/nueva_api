namespace OmniSuite.Application.Callback.Commands
{
    public class CallbackCommand : IRequest<Response<bool>>
    {
        public string Id { get; set; }

        public string Status { get; set; }
    }
}
