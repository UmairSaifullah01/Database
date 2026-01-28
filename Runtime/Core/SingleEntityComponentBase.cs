using UnityEngine;

namespace THEBADDEST.DatabaseModule
{
	/// <summary>
	/// Abstract base class for single entity components.
	/// Similar to TableBase but for components that hold a single record.
	/// </summary>
	public abstract class SingleEntityComponentBase : ScriptableObject, IDatabaseComponent
	{
		[SerializeField]
		private string componentName;

		/// <summary>
		/// Gets the component name for display in the Database editor.
		/// </summary>
		/// <returns>The component's display name</returns>
		public virtual string GetComponentName()
		{
			return string.IsNullOrEmpty(componentName) ? GetType().Name : componentName;
		}

		/// <summary>
		/// Initializes the component. Called when the Database is initialized.
		/// </summary>
		public virtual void Initialize()
		{
			// Component initialization logic can be added here if needed
		}
	}
}
