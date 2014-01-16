namespace ShortBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IDependencyResolver
    {
        object GetInstance(Type type);
        IEnumerable<T> GetInstances<T>();
    }

    public static class DependencyResolver
    {
        public static IDependencyResolver Current;

        public static void SetResolver(IDependencyResolver resolver)
        {
            Current = resolver;
        }

        public static void SetResolver(Func<Type, object> getInstance, Func<Type, IEnumerable<object>> getInstances)
        {
            Current = new DelegateDependencyResolver(getInstance, getInstances);
        }

        class DelegateDependencyResolver : IDependencyResolver
        {
            readonly Func<Type, object> _getInstance;
            readonly Func<Type, IEnumerable<object>> _getInstances;

            public DelegateDependencyResolver(Func<Type, object> getInstance, Func<Type, IEnumerable<object>> getInstances)
            {
                _getInstance = getInstance;
                _getInstances = getInstances;
            }

            public object GetInstance(Type type)
            {
                return _getInstance(type);
            }

            public IEnumerable<T> GetInstances<T>()
            {
                return _getInstances(typeof (T)).OfType<T>();
            }
        }
    }
}