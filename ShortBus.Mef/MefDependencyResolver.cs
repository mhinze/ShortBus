namespace ShortBus.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    public class MefDependencyResolver : IDependencyResolver
    {
        readonly CompositionContainer _container;

        public MefDependencyResolver(CompositionContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.GetExport<object>(AttributedModelServices.GetContractName(type));
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.GetExportedValues<T>();
        }
    }
}