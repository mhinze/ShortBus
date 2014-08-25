namespace ShortBus.SimpleInjector
{
	using System;
	using System.Collections.Generic;
	using global::SimpleInjector;

	public class SimpleInjectorDependencyResolver : IDependencyResolver
	{
		private readonly Container _container;

		public SimpleInjectorDependencyResolver(Container container)
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