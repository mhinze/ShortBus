namespace ShortBus.Tests.Example
{
    public class AutoResponder : IHandler<PingMessage, string>
    {
        public string Handle(PingMessage request)
        {
            return "PONG!";
        }
    }

    public class PingMessage : IRequest<string> {}
}