namespace ShortBus.Tests.Example
{
    public class Pong : IQueryHandler<Ping, string>
    {
        public string Handle(Ping request)
        {
            return "PONG!";
        }
    }

    public class PongAB : IQueryHandler<PingA, string>, IQueryHandler<PingB, string>
    {
        public string Handle(PingA request)
        {
            return "PONG-A!";
        }

        public string Handle(PingB request)
        {
            return "PONG-B!";
        }
    }
}