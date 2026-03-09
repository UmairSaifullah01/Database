using UnityEngine;
using UnityEngine.SceneManagement;

namespace THEBADDEST
{
	/// <summary>
	/// Clears the scene's ServiceLocator when a Unity scene is unloaded.
	/// Place this component on a GameObject that stays active for the lifetime of the app
	/// (e.g. in a bootstrap scene or a root object with DontDestroyOnLoad) so it receives
	/// unload events for every scene.
	/// </summary>
	public class ServiceLocatorBootStrap : MonoBehaviour
	{
		private void OnEnable()
		{
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

		private void OnSceneUnloaded(Scene scene)
		{
			ServiceLocator.ClearScene(scene.name);
		}
	}
}
