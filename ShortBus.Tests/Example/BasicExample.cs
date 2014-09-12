using System;
using System.Diagnostics;
using NUnit.Framework;
using ShortBus.StructureMap;
using StructureMap;
using System.Reflection;

namespace ShortBus.Tests.Example
{
    [TestFixture]
    public class BasicExample
    {
        private readonly IContainer _container;
        public BasicExample()
        {
            _container = new Container();

            _container.Configure(i =>
            {
                i.Scan(s =>
                {
                    s.AssemblyContainingType<IMediator>();
                    s.Assembly(Assembly.GetExecutingAssembly());
                    s.WithDefaultConventions();
                    s.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    s.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                i.For<IDependencyResolver>().Use(() => DependencyResolver.Current);
            });

            ShortBus.DependencyResolver.SetResolver(new StructureMapDependencyResolver(_container));
        }

        [Test]
        public void RequestResponse()
        {
            var query = new Ping();

            var mediator = _container.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void RequestResponseImplementationWithMultipleHandler()
        {
            var query = new TriplePing();

            var mediator = _container.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG! PONG! PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void RequestResponse_variant()
        {
            var query = new PingALing();

            var mediator = _container.GetInstance<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.Data, Is.EqualTo("PONG!"));
            Assert.That(pong.HasException(), Is.False);
        }

        [Test]
        public void RequestResponseImplementationWithMultipleHandler_variant()
        {
            var query = new DoublePingALing();

            var mediator = _container.GetInstance<IMediator>();

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

            var mediator = _container.GetInstance<IMediator>();

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

            var mediator = _container.GetInstance<IMediator>();

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
            var mediator = _container.GetInstance<IMediator>();
            var query = new Ping();

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
                mediator.Request(query);

            watch.Stop();

            Console.WriteLine(watch.Elapsed);
        }
    }
}