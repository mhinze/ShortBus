using Microsoft.Practices.Unity;

namespace ShortBus.Unity
{
    using System;
    using System.Collections.Generic;

    public class UnityDependencyResolver : IDependencyResolver
    {
        readonly IUnityContainer _container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.Resolve(type);
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.ResolveAll<T>();
        }
    }
}