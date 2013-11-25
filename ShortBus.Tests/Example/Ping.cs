namespace ShortBus.Tests.Example
{
    public class Ping : IQuery<string> {}
    public class PingALing : Ping {}

    public class PingA : IQuery<string> { }
    public class PingB : IQuery<string> { }
}