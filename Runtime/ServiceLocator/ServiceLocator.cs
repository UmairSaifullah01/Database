using System;
using System.Collections.Generic;


namespace THEBADDEST
{


	public class ServiceLocator : IServiceLocator
	{
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

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
			if (_services.TryGetValue(typeof(TService), out var service))
			{
				_services.Remove(typeof(TService));
			}
		}

		public TService GetService<TService>()
		{
			if (_services.TryGetValue(typeof(TService), out var service))
			{
				return (TService)service;
			}

			throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
		}
	}


}