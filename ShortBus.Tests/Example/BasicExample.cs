using System;
using System.Diagnostics;
using NUnit.Framework;
using ShortBus.StructureMap;
using StructureMap;

namespace ShortBus.Tests.Example
{
    [TestFixture]
    public class BasicExample
    {
        public BasicExample()
        {
            ObjectFactory.Initialize(i =>
            {
                i.Scan(s =>
                {
                    s.AssemblyContainingType<IMediator>();
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                    s.AddAllTypesOf((typeof(IRequestHandler<,>)));
                    s.AddAllTypesOf(typeof(INotificationHandler<>));
                });

                i.For<IDependencyResolver>().Use(() => DependencyResolver.Current);
            });

            ShortBus.DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container));
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
        public void RequestResponseImplementationWithMultipleHandler()
        {
            var query = new TriplePing();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG! PONG! PONG!"));
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
        public void RequestResponseImplementationWithMultipleHandler_variant()
        {
            var query = new DoublePingALing();

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG! PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void Send_void()
        {
            var command = new PrintText
            {
                Format = "This is a {0} message",
                Args = new object[] { "text" }
            };

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var response = mediator.Request(command);

            Assert.That(response.HasException(), Is.False, response.Exception == null ? string.Empty : response.Exception.ToString());
        }

        [Test]
        public void Send_void_variant()
        {
            var command = new PrintTextSpecial
            {
                Format = "This is a {0} message",
                Args = new object[] { "text" }
            };

            var mediator = ObjectFactory.GetInstance<IMediator>();

            var response = mediator.Request(command);

            Assert.That(response.HasException(), Is.False);
        }

        [Test]
        public void Send_result()
        {
            var command = new CommandWithResult();

            var mediator = new Mediator(DependencyResolver.Current);

            var response = mediator.Request(command);

            Assert.That(response.Data, Is.EqualTo("foo"));
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