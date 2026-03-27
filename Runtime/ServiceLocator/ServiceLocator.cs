using System;
using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST
{
	public partial class ServiceLocator : IServiceLocator
	{
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

		// Private constructor to prevent instantiation (used by static partial)
		private ServiceLocator() { }

		public void RegisterService<TService>(TService service)
		{
			if (service == null)
			{
				Debug.LogWarning($"Attempted to register null service of type {typeof(TService).Name}");
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

			Debug.LogWarning($"Service of type {typeof(TService).Name} is not registered.");
			return default;
		}

		public bool TryGetService<TService>(out TService service)
		{
			if (_services.TryGetValue(typeof(TService), out var obj))
			{
				service = (TService)obj;
				return true;
			}

			service = default;
			return false;
		}

		public void Clear()
		{
			_services.Clear();
		}
	}
}
