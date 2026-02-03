namespace THEBADDEST.DatabaseModule
{
	/// <summary>
	/// Runtime interface that all database components must implement.
	/// Components include tables, scriptable object references, and single entity references.
	/// </summary>
	public interface IDatabaseComponent
	{
		/// <summary>
		/// Gets the display name for this component (shown in the Database editor).
		/// </summary>
		/// <returns>The component's display name</returns>
		string GetComponentName();

		/// <summary>
		/// Initializes the component. Called when the Database is initialized.
		/// </summary>
		void Initialize();
	}
}
