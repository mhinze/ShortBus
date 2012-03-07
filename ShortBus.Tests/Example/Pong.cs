namespace ShortBus.Tests.Example
{
    public class Pong : IQueryHandler<PingMessage, string>
    {
        public string Handle(PingMessage request)
        {
            return "PONG!";
        }
    }
}