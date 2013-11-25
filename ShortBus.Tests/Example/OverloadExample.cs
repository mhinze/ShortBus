using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap;

namespace ShortBus.Tests.Example
{
    [TestFixture]
    public class OverloadExample
    {
        public OverloadExample()
        {
            ObjectFactory.Initialize(i => i.Scan(s =>
                {
                    s.AssemblyContainingType<IMediator>();
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                    s.AddAllTypesOf((typeof (IQueryHandler<,>)));
                    s.AddAllTypesOf(typeof (ICommandHandler<>));
                }));
        }

        [Test]
        public void ImplementMultipleQueryHandlers()
        {
            var querya = new PingA();
            var queryb = new PingB();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var ponga = mediator.Request(querya);

            Assert.That(ponga.Data, Is.EqualTo("PONG-A!"));
            Assert.That(ponga.HasException(), Is.False);

            var pongb = mediator.Request(queryb);

            Assert.That(pongb.Data, Is.EqualTo("PONG-B!"));
            Assert.That(pongb.HasException(), Is.False);
        }

        [Test]
        public void Send()
        {
            var commandA = new PrintTextA
                {
                    Format = "This is a {0} message for {1}",
                    Args = new object[] {"text", "A"}
                };

            var commandB = new PrintTextB
            {
                Format = "This is a {0} message for {1}",
                Args = new object[] { "text", "B" }
            };

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var responseA = mediator.Send(commandA);

            Assert.That(responseA.HasException(), Is.False);

            var responseB = mediator.Send(commandB);

            Assert.That(responseB.HasException(), Is.False);
        }
    }
}