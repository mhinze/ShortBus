namespace ShortBus.Tests.Example
{
    public class Pong : IQueryHandler<Ping, string>
    {
        public string Handle(Ping request)
        {
            return "PONG!";
        }
    }
}