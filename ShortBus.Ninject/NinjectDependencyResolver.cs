using Ninject;

namespace ShortBus.Ninject
{
    using System;
    using System.Collections.Generic;

    public class NinjectDependencyResolver : IDependencyResolver
    {
        readonly IKernel _container;

        public NinjectDependencyResolver(IKernel container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.Get(type);
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.GetAll<T>();
        }
    }
}