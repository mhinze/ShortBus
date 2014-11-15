namespace ShortBus.Unity
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;

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
            List<T> results = new List<T>(_container.ResolveAll<T>());

            try
            {
                results.Add(_container.Resolve<T>()); // needed to resolve unnamed instances.
            }
            catch(ResolutionFailedException)
            {
                // Unity throws an error if it can't resolve a type. In this case we don't care if it failed to resolve.
            }

            return results;
        }
    }
}