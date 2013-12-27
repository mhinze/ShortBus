namespace ShortBus.Autofac
{
    using System;
    using global::Autofac;

    public class AutofacDependencyResolver : IDependencyResolver
    {
        readonly IContainer _container;

        public AutofacDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.Resolve(type);
        }
    }
}