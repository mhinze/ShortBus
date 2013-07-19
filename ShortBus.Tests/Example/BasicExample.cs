using System;
using System.Diagnostics;
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
                    s.AssemblyContainingType<IMediator>();
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                    s.AddAllTypesOf((typeof (IQueryHandler<,>)));
                    s.AddAllTypesOf(typeof (ICommandHandler<>));
                }));
        }

        [Test]
        public void RequestResponse()
        {
            var query = new Ping();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void RequestResponse_variant()
        {
            var query = new PingALing();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void Send()
        {
            var command = new PrintText
                {
                    Format = "This is a {0} message",
                    Args = new object[] {"text"}
                };

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var response = mediator.Send(command);

            Assert.That(response.HasException(), Is.False);
        }

        [Test]
        public void Send_variant()
        {
            var command = new PrintTextSpecial
                {
                    Format = "This is a {0} message",
                    Args = new object[] {"text"}
                };

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var response = mediator.Send(command);

            Assert.That(response.HasException(), Is.False);
        }

        [Test, Explicit]
        public void Perf()
        {
            var mediator = ObjectFactory.GetInstance<IMediator>();
            var query = new Ping();

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
                mediator.Request(query);

            watch.Stop();

            Console.WriteLine(watch.Elapsed);
        }
    }
}