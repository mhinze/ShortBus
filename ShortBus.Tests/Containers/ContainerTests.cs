
namespace ShortBus.Tests.Containers
{
    using Autofac;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using global::Autofac;
    using global::Ninject;
    using global::StructureMap;
    using Microsoft.Practices.Unity;
    using Ninject;
    using NUnit.Framework;
    using StructureMap;
    using Unity;
    using Windsor;
	using SimpleInjector;

    [TestFixture]
    public class ContainerTests
    {
        public ContainerTests() { }

        class Registered { }

        [Test]
        public void AutofacResolveSingleInstance()
        {
            var builder = new ContainerBuilder();
            var registered = new Registered();
            builder.RegisterInstance(registered);

            var resolver = new AutofacDependencyResolver(builder.Build());

            var resolved = (Registered) resolver.GetInstance(typeof (Registered));

            Assert.That(resolved, Is.EqualTo(registered));
        }

        [Test]
        public void NinjectResolveSingleInstance()
        {
            var kernel = new StandardKernel();
            var registered = new Registered();
            kernel.Bind<Registered>().ToConstant(registered);

            var resolver = new NinjectDependencyResolver(kernel);

            var resolved = (Registered) resolver.GetInstance(typeof (Registered));

            Assert.That(resolved, Is.EqualTo(registered));
        }

        [Test]
        public void StructureMapResolveSingleInstance()
        {
            var registered = new Registered();

            ObjectFactory.Initialize(i => i.Register(registered));

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var resolved = (Registered) resolver.GetInstance(typeof (Registered));

            Assert.That(resolved, Is.EqualTo(registered));
        }

        [Test]
        public void UnityResolveSingleInstance()
        {
            var container = new UnityContainer();
            var registered = new Registered();
            container.RegisterInstance(registered);

            var resolver = new UnityDependencyResolver(container);

            var resolved = (Registered) resolver.GetInstance(typeof (Registered));

            Assert.That(resolved, Is.EqualTo(registered));
        }

        [Test]
        public void WindsorResolveSingleInstance()
        {
            var container = new WindsorContainer();
            var registered = new Registered();
            container.Register(Component.For<Registered>().Instance(registered));

            var resolver = new WindsorDependencyResolver(container);

            var resolved = (Registered) resolver.GetInstance(typeof (Registered));

            Assert.That(resolved, Is.EqualTo(registered));
        }

	    [Test]
	    public void SimpleInjectorResolveSingleInstance()
	    {
		    var container = new global::SimpleInjector.Container();
		    var registered = new Registered();
			container.RegisterSingle(typeof(Registered), registered);

		    var resolver = new SimpleInjectorDependencyResolver(container);
		    
			var resolved = resolver.GetInstance(typeof (Registered));

			Assert.That(resolved, Is.EqualTo(registered));
	    }
    }
}