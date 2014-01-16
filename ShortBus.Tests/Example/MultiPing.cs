namespace ShortBus.Tests.Example
{
    public class DoublePing : IRequest<string> {}
    public class DoublePingALing : DoublePing {}
    public class TriplePing : IRequest<string> {}
    public class TriplePingALing : TriplePing {}
}