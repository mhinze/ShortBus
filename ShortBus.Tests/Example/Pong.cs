namespace ShortBus.Tests.Example
{
    public class Pong : IRequestHandler<Ping, string>
    {
        public string Handle(Ping request)
        {
            return "PONG!";
        }
    }
}