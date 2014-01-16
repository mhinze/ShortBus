namespace ShortBus.Tests.Example
{
    public class MultiPong 
    : IRequestHandler<DoublePing, string>,
      IRequestHandler<TriplePing, string>
    {
        public string Handle(DoublePing request)
        {
            return "PONG! PONG!";
        }

        public string Handle(TriplePing request)
        {
            return "PONG! PONG! PONG!";
        }
    }
}