﻿namespace ShortBus.Windsor
{
    using System;
    using System.Collections.Generic;
    using Castle.Windsor;

    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorDependencyResolver(IWindsorContainer container)
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