using NUnit.Framework;
using StructureMap;

namespace ShortBus.Tests.Example
{
    [TestFixture]
    public class BasicExample
    {
        public BasicExample()
        {
            ObjectFactory.Initialize(i => i.Scan(s =>
                {
                    s.AssemblyContainingType<IBus>();
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                    s.ConnectImplementationsToTypesClosing((typeof (IQueryHandler<,>)));
                    s.AddAllTypesOf(typeof (ICommandHandler<>));
                }));
        }

        [Test]
        public void RequestResponse()
        {
            var ping = new Ping();

            var bus = ObjectFactory.GetInstance<IBus>();

            var pong = bus.Request(ping);

            Assert.That(pong.Data, Is.EqualTo("PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void Send()
        {
            var message = new TextMessage
                {
                    Format = "This is a {0} message",
                    Args = new object[] {"text"}
                };

            var bus = ObjectFactory.GetInstance<IBus>();

            var response = bus.Send(message);

            Assert.That(response.HasException(), Is.False);
        }
    }
}