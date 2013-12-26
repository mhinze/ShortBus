namespace ShortBus
{
    using System;

    public interface IDependencyResolver
    {
        object GetInstance(Type type);
    }

    public static class DependencyResolver
    {
        public static IDependencyResolver Current;

        public static void SetResolver(IDependencyResolver resolver)
        {
            Current = resolver;
        }

        public static void SetResolver(Func<Type, object> getInstance)
        {
            Current = new DelegateDependencyResolver(getInstance);
        }

        class DelegateDependencyResolver : IDependencyResolver
        {
            readonly Func<Type, object> _getInstance;

            public DelegateDependencyResolver(Func<Type, object> getInstance)
            {
                _getInstance = getInstance;
            }

            public object GetInstance(Type type)
            {
                return _getInstance(type);
            }
        }
    }
}