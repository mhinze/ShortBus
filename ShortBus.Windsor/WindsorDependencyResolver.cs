namespace ShortBus.Windsor
{
    using System;
    using Castle.Windsor;

    public class WindsorDependencyResolver : IDependencyResolver
    {
        readonly IWindsorContainer _container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.Resolve(type);
        }
    }
}