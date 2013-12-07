using Autofac;

namespace ShortBus.Autofac
{
    using System;
    using System.Collections.Generic;

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

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.Resolve<IEnumerable<T>>();
        }
    }
}