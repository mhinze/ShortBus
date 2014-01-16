namespace ShortBus.StructureMap
{
    using System;
    using System.Collections.Generic;
    using global::StructureMap;

    public class StructureMapDependencyResolver : IDependencyResolver
    {
        readonly IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.GetAllInstances<T>();
        }
    }
}