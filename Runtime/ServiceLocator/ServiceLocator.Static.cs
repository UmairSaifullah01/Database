using System;
using System.Collections.Generic;

namespace THEBADDEST
{
	public partial class ServiceLocator
	{
		private static readonly Lazy<ServiceLocator> _global =
			new Lazy<ServiceLocator>(() => new ServiceLocator());

		private static readonly Dictionary<string, ServiceLocator> _sceneLocators =
			new Dictionary<string, ServiceLocator>();

		public static ServiceLocator Global => _global.Value;

		#region Global static API

		public static void RegisterServiceGlobal<TService>(TService service)
		{
			_global.Value.RegisterService(service);
		}

		public static void UnregisterServiceGlobal<TService>()
		{
			_global.Value.UnregisterService<TService>();
		}

		public static TService GetServiceGlobal<TService>()
		{
			return _global.Value.GetService<TService>();
		}

		public static bool TryGetServiceGlobal<TService>(out TService service)
		{
			return _global.Value.TryGetService(out service);
		}

		public static void ClearGlobal()
		{
			_global.Value.Clear();
		}

		#endregion

		#region Scene static API

		private static ServiceLocator GetOrCreateSceneLocator(string sceneName)
		{
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException("Scene name cannot be null or empty.", nameof(sceneName));

			if (!_sceneLocators.TryGetValue(sceneName, out var locator))
			{
				locator = new ServiceLocator();
				_sceneLocators[sceneName] = locator;
			}

			return locator;
		}

		/// <summary>
		/// Gets the service locator for the given scene, creating it if it does not exist.
		/// </summary>
		/// <param name="sceneName">The scene name for the scene-scoped service locator</param>
		/// <returns>The ServiceLocator instance for the scene</returns>
		public static ServiceLocator GetSceneLocator(string sceneName)
		{
			return GetOrCreateSceneLocator(sceneName);
		}

		public static void RegisterServiceScene<TService>(string sceneName, TService service)
		{
			GetOrCreateSceneLocator(sceneName).RegisterService(service);
		}

		public static void UnregisterServiceScene<TService>(string sceneName)
		{
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException("Scene name cannot be null or empty.", nameof(sceneName));

			if (_sceneLocators.TryGetValue(sceneName, out var locator))
				locator.UnregisterService<TService>();
		}

		public static TService GetServiceScene<TService>(string sceneName)
		{
			return GetOrCreateSceneLocator(sceneName).GetService<TService>();
		}

		public static bool TryGetServiceScene<TService>(string sceneName, out TService service)
		{
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException("Scene name cannot be null or empty.", nameof(sceneName));

			if (_sceneLocators.TryGetValue(sceneName, out var locator))
				return locator.TryGetService(out service);

			service = default;
			return false;
		}

		public static void ClearScene(string sceneName)
		{
			if (string.IsNullOrEmpty(sceneName))
				throw new ArgumentException("Scene name cannot be null or empty.", nameof(sceneName));

			if (_sceneLocators.TryGetValue(sceneName, out var locator))
			{
				locator.Clear();
				_sceneLocators.Remove(sceneName);
			}
		}

		#endregion
	}
}
