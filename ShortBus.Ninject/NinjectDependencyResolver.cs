namespace ShortBus.Ninject
{
    using System;
    using global::Ninject;

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
    }
}