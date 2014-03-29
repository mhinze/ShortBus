namespace ShortBus.Tests.Containers
{
    using Autofac;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using global::Autofac;
    using global::Ninject;
    using global::StructureMap;
    using Microsoft.Practices.Unity;
    using Mef;
    using Ninject;
    using NUnit.Framework;
    using StructureMap;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Unity;
    using Windsor;

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
        public void MefResolveSingleInstance()
        {
            var container = new CompositionContainer();
            var registered = new Registered();
            container.ComposeExportedValue(registered);

            var resolver = new MefDependencyResolver(container);

            var resolved = (Registered)resolver.GetInstance(typeof(Registered));

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
    }
}