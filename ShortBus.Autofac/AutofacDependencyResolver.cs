namespace ShortBus.Autofac
{
    using System;
    using global::Autofac;

    public class AutofacDependencyResolver : IDependencyResolver
    {
        readonly ILifetimeScope _container;

        public AutofacDependencyResolver(ILifetimeScope container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.Resolve(type);
        }
    }
}