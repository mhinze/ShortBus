namespace ShortBus.Tests.Example
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using global::Autofac;
    using global::Autofac.Features.Variance;
    using NUnit.Framework;
    using ShortBus.Autofac;

    [TestFixture]
    public class AutofacBasicExample
    {
        private ILifetimeScope RootScope { get; set; }
        private ILifetimeScope TestScope { get; set; }

        [TestFixtureSetUp]
        public void SetUpRootScope()
        {
            var builder = new ContainerBuilder();

            // this is needed to allow the Mediator to resolve contravariant handlers (not enabled by default in Autofac)
            builder.RegisterSource(new ContravariantRegistrationSource());

            builder.RegisterAssemblyTypes(typeof (IMediator).Assembly, GetType().Assembly)
                .AsClosedTypesOf(typeof (IRequestHandler<,>))
                .AsImplementedInterfaces();

            builder.RegisterType<Mediator>().AsImplementedInterfaces().InstancePerLifetimeScope();

            // to allow ShortBus to resolve lifetime-scoped dependencies properly, 
            // we really can't use the default approach of setting the static (global) dependency resolver, 
            // since that resolves instances from the root scope passed into it, rather than 
            // the current lifetime scope at the time of resolution.  
            // Resolving from the root scope can cause resource leaks, or in the case of components with a 
            // specific scope affinity (AutofacWebRequest, for example) it would fail outright, 
            // since that scope doesn't exist at the root level.
            builder.RegisterType<AutofacDependencyResolver>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            RootScope = builder.Build();

            RootScope.ComponentRegistry.Sources.ToList()
                .ForEach(s => Console.WriteLine("{0} ({1})", s.GetType().Name, s));
        }

        [TestFixtureTearDown]
        public void TearDownRootScope()
        {
            RootScope.Dispose();
            RootScope = null;
        }

        [SetUp]
        public void BeginTestScope()
        {
            TestScope = RootScope.BeginLifetimeScope("testScope");
        }

        [TearDown]
        public void EndTestScope()
        {
            TestScope.Dispose();
            TestScope = null;
        }

        [Test]
        public void RequestResponse()
        {
            var query = new Ping();

            var mediator = TestScope.Resolve<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.HasException(), Is.False,
                pong.Exception == null ? string.Empty : pong.Exception.ToString());
            Assert.That(pong.Data, Is.EqualTo("PONG!"));
        }

        [Test]
        public void RequestResponseImplementationWithMultipleHandler()
        {
            var query = new TriplePing();

            var mediator = TestScope.Resolve<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.HasException(), Is.False,
                pong.Exception == null ? string.Empty : pong.Exception.ToString());
            Assert.That(pong.Data, Is.EqualTo("PONG! PONG! PONG!"));
        }

        [Test]
        public void RequestResponse_variant()
        {
            var query = new PingALing();

            var mediator = TestScope.Resolve<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.HasException(), Is.False,
                pong.Exception == null ? string.Empty : pong.Exception.ToString());
            Assert.That(pong.Data, Is.EqualTo("PONG!"));
        }

        [Test]
        public void RequestResponseImplementationWithMultipleHandler_variant()
        {
            var query = new DoublePingALing();

            var mediator = TestScope.Resolve<IMediator>();

            var pong = mediator.Request(query);

            Assert.That(pong.HasException(), Is.False,
                pong.Exception == null ? string.Empty : pong.Exception.ToString());
            Assert.That(pong.Data, Is.EqualTo("PONG! PONG!"));
        }

        [Test]
        public void Send_void()
        {
            var command = new PrintText
            {
                Format = "This is a {0} message",
                Args = new object[] {"text"}
            };

            var mediator = TestScope.Resolve<IMediator>();

            var response = mediator.Request(command);

            Assert.That(response.HasException(), Is.False,
                response.Exception == null ? string.Empty : response.Exception.ToString());
        }

        [Test]
        public void Send_void_variant()
        {
            var command = new PrintTextSpecial
            {
                Format = "This is a {0} message",
                Args = new object[] {"text"}
            };

            var mediator = TestScope.Resolve<IMediator>();

            var response = mediator.Request(command);

            Assert.That(response.HasException(), Is.False,
                response.Exception == null ? string.Empty : response.Exception.ToString());
        }

        [Test]
        public void Send_result()
        {
            var command = new CommandWithResult();

            var mediator = TestScope.Resolve<IMediator>();

            var response = mediator.Request(command);

            Assert.That(response.Data, Is.EqualTo("foo"),
                response.Exception == null ? string.Empty : response.Exception.ToString());
        }

        [Test, Explicit]
        public void Perf()
        {
            var mediator = TestScope.Resolve<IMediator>();
            var query = new Ping();

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 10000; i++)
                mediator.Request(query);

            watch.Stop();

            Console.WriteLine(watch.Elapsed);
        }
    }
}