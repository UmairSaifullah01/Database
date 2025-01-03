using System;
using System.Collections.Generic;

namespace THEBADDEST
{
	public class ServiceLocator : IServiceLocator
	{
		private static readonly Lazy<ServiceLocator> _global =
			new Lazy<ServiceLocator>(() => new ServiceLocator());

		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

		// Private constructor to prevent instantiation
		private ServiceLocator() { }

		// Public property for global access
		public static ServiceLocator Global => _global.Value;

		public void RegisterService<TService>(TService service)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			_services[typeof(TService)] = service;
		}

		public void UnregisterService<TService>()
		{
			_services.Remove(typeof(TService));
		}

		public TService GetService<TService>()
		{
			if (_services.TryGetValue(typeof(TService), out var service))
			{
				return (TService)service;
			}

			throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
		}

		public void Clear()
		{
			_services.Clear();
		}

	}
}