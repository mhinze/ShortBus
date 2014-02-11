﻿namespace ShortBus.Tests.Containers
{
    using System.Linq;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using global::Autofac;
    using global::Ninject;
    using global::StructureMap;
    using Microsoft.Practices.Unity;
    using NUnit.Framework;
    using ShortBus.Autofac;
    using ShortBus.Ninject;
    using ShortBus.StructureMap;
    using ShortBus.Unity;
    using ShortBus.Windsor;

    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void AutofacResolve()
        {
            var builder = new ContainerBuilder();
            var registerSingle = new RegisterSingle();
            var multipleRegistrations = new[] {new RegisterMultiple(), new RegisterMultiple()};
            builder.RegisterInstance(registerSingle);
            builder.RegisterInstance(multipleRegistrations[0]);
            builder.RegisterInstance(multipleRegistrations[1]);

            var resolver = new AutofacDependencyResolver(builder.Build());

            var resolvedSingle = (RegisterSingle) resolver.GetInstance(typeof (RegisterSingle));
            var resolvedMultiple = resolver.GetInstances<RegisterMultiple>().ToArray();
            Assert.That(resolvedSingle, Is.EqualTo(registerSingle));
            Assert.That(resolvedMultiple.Count(), Is.EqualTo(2));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[0]));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[1]));
        }

        [Test]
        public void NinjectResolve()
        {
            var kernel = new StandardKernel();
            var registerSingle = new RegisterSingle();
            var multipleRegistrations = new[] {new RegisterMultiple(), new RegisterMultiple()};
            kernel.Bind<RegisterSingle>().ToConstant(registerSingle);
            kernel.Bind<RegisterMultiple>().ToConstant(multipleRegistrations[0]);
            kernel.Bind<RegisterMultiple>().ToConstant(multipleRegistrations[1]);

            var resolver = new NinjectDependencyResolver(kernel);

            var resolvedSingle = (RegisterSingle) resolver.GetInstance(typeof (RegisterSingle));
            var resolvedMultiple = resolver.GetInstances<RegisterMultiple>().ToArray();
            Assert.That(resolvedSingle, Is.EqualTo(registerSingle));
            Assert.That(resolvedMultiple.Count(), Is.EqualTo(2));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[0]));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[1]));
        }

        [Test]
        public void StructureMapResolve()
        {
            var registerSingle = new RegisterSingle();
            var multipleRegistrations = new[] {new RegisterMultiple(), new RegisterMultiple()};

            ObjectFactory.Initialize(i =>
            {
                i.Register(registerSingle);
                i.Register(multipleRegistrations[0]);
                i.Register(multipleRegistrations[1]);
            });

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

            var resolvedSingle = (RegisterSingle) resolver.GetInstance(typeof (RegisterSingle));
            var resolvedMultiple = resolver.GetInstances<RegisterMultiple>().ToArray();
            Assert.That(resolvedSingle, Is.EqualTo(registerSingle));
            Assert.That(resolvedMultiple.Count(), Is.EqualTo(2));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[0]));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[1]));
        }

        [Test]
        public void UnityResolve()
        {
            var container = new UnityContainer();
            var registerSingle = new RegisterSingle();
            var multipleRegistrations = new[] {new RegisterMultiple(), new RegisterMultiple()};
            container.RegisterInstance(registerSingle);
            //unity requires a name when registering multiple instances of the same type or nothing will be resolved.
            container.RegisterInstance("instance1", multipleRegistrations[0]);
            container.RegisterInstance("instance2", multipleRegistrations[1]);

            var resolver = new UnityDependencyResolver(container);

            var resolvedSingle = (RegisterSingle) resolver.GetInstance(typeof (RegisterSingle));
            var resolvedMultiple = resolver.GetInstances<RegisterMultiple>().ToArray();
            Assert.That(resolvedSingle, Is.EqualTo(registerSingle));
            Assert.That(resolvedMultiple.Count(), Is.EqualTo(2));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[0]));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[1]));
        }

        [Test]
        public void WindsorResolve()
        {
            var container = new WindsorContainer();
            var registerSingle = new RegisterSingle();
            var multipleRegistrations = new[] {new RegisterMultiple(), new RegisterMultiple()};
            //windsor requires a name when registering multiple instances of the same type or nothing will be resolved.
            container.Register(Component.For<RegisterSingle>().Instance(registerSingle));
            container.Register(Component.For<RegisterMultiple>().Instance(multipleRegistrations[0]).Named("instance1"));
            container.Register(Component.For<RegisterMultiple>().Instance(multipleRegistrations[1]).Named("instance2"));

            var resolver = new WindsorDependencyResolver(container);

            var resolvedSingle = (RegisterSingle) resolver.GetInstance(typeof (RegisterSingle));
            var resolvedMultiple = resolver.GetInstances<RegisterMultiple>().ToArray();
            Assert.That(resolvedSingle, Is.EqualTo(registerSingle));
            Assert.That(resolvedMultiple.Count(), Is.EqualTo(2));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[0]));
            Assert.That(resolvedMultiple.Contains(multipleRegistrations[1]));
        }

        private class RegisterSingle {}

        private class RegisterMultiple {}
    }
}