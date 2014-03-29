using System.Linq;

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
            return GetInstance(type, null);
        }

        object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            var enumerable = exports as object[] ?? exports.ToArray();
            if (enumerable.Any())
                return enumerable.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        public IEnumerable<T> GetInstances<T>()
        {
            return _container.GetExportedValues<T>();
        }
    }
}