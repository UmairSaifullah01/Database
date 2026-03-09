using UnityEngine;
using THEBADDEST;

namespace THEBADDEST.DatabaseModule
{
    /// <summary>
    /// Extension methods for UnityEngine.Object to easily access the global service locator
    /// </summary>
    public static class ObjectServiceExtensions
    {
        /// <summary>
        /// Gets a service from the global service locator
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="global">Whether to use the global service locator (always true, kept for API consistency)</param>
        /// <returns>The service instance</returns>
        public static T GetService<T>(this Object obj, bool global = true)
        {
            return ServiceLocator.Global.GetService<T>();
        }

        /// <summary>
        /// Registers a service to the global service locator
        /// </summary>
        /// <typeparam name="T">The type of service to register</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="service">The service instance to register</param>
        /// <param name="global">Whether to use the global service locator (always true, kept for API consistency)</param>
        public static void RegisterService<T>(this Object obj, T service, bool global = true)
        {
            ServiceLocator.Global.RegisterService(service);
        }

        /// <summary>
        /// Unregisters a service from the global service locator
        /// </summary>
        /// <typeparam name="T">The type of service to unregister</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="global">Whether to use the global service locator (always true, kept for API consistency)</param>
        public static void UnregisterService<T>(this Object obj, bool global = true)
        {
            ServiceLocator.Global.UnregisterService<T>();
        }

        /// <summary>
        /// Tries to get a service from the global service locator
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="service">The service instance if found</param>
        /// <param name="global">Whether to use the global service locator (always true, kept for API consistency)</param>
        /// <returns>True if the service was found, false otherwise</returns>
        public static bool TryGetService<T>(this Object obj, out T service, bool global = true)
        {
            return ServiceLocator.Global.TryGetService(out service);
        }

        #region Scene-based extensions

        /// <summary>
        /// Gets a service from the scene service locator
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="sceneName">The scene name for the scene-scoped service locator</param>
        /// <returns>The service instance</returns>
        public static T GetServiceScene<T>(this Object obj, string sceneName)
        {
            return ServiceLocator.GetServiceScene<T>(sceneName);
        }

        /// <summary>
        /// Registers a service to the scene service locator
        /// </summary>
        /// <typeparam name="T">The type of service to register</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="sceneName">The scene name for the scene-scoped service locator</param>
        /// <param name="service">The service instance to register</param>
        public static void RegisterServiceScene<T>(this Object obj, string sceneName, T service)
        {
            ServiceLocator.RegisterServiceScene(sceneName, service);
        }

        /// <summary>
        /// Unregisters a service from the scene service locator
        /// </summary>
        /// <typeparam name="T">The type of service to unregister</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="sceneName">The scene name for the scene-scoped service locator</param>
        public static void UnregisterServiceScene<T>(this Object obj, string sceneName)
        {
            ServiceLocator.UnregisterServiceScene<T>(sceneName);
        }

        /// <summary>
        /// Tries to get a service from the scene service locator
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <param name="obj">The Unity object (used for extension method syntax)</param>
        /// <param name="sceneName">The scene name for the scene-scoped service locator</param>
        /// <param name="service">The service instance if found</param>
        /// <returns>True if the service was found, false otherwise</returns>
        public static bool TryGetServiceScene<T>(this Object obj, string sceneName, out T service)
        {
            return ServiceLocator.TryGetServiceScene(sceneName, out service);
        }

        #endregion
    }
}

