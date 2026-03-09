using UnityEngine;

namespace THEBADDEST.DatabaseModule
{
	/// <summary>
	/// Component that holds a reference to any ScriptableObject (e.g., SoundSystem)
	/// for inspection and management within the Database system.
	/// </summary>
	[CreateAssetMenu(menuName = "THEBADDEST/Database/Database Component", fileName = "DatabaseComponent")]
	public class DatabaseComponent : ScriptableObject, IDatabaseComponent
	{
		[SerializeField]
		private string componentName = "New Component";

		[SerializeField]
		private ScriptableObject targetScriptable;

		/// <summary>
		/// The ScriptableObject reference that this component wraps.
		/// </summary>
		public ScriptableObject TargetScriptable
		{
			get => targetScriptable;
			set => targetScriptable = value;
		}

		/// <summary>
		/// Gets the component name for display in the Database editor.
		/// Returns the ScriptableObject name when assigned, otherwise a fallback.
		/// </summary>
		/// <returns>The component's display name</returns>
		public string GetComponentName()
		{
			if (targetScriptable != null)
				return targetScriptable.name;
			return !string.IsNullOrEmpty(componentName) ? componentName : "Unassigned";
		}

		/// <summary>
		/// Initializes the component. Called when the Database is initialized.
		/// </summary>
		public void Initialize()
		{
			// Component initialization logic can be added here if needed
		}
	}
}
