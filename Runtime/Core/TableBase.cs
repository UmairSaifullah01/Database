using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public abstract class TableBase : ScriptableObject, ITable, IDatabaseComponent
	{
		/// <summary>
		/// Returns the table name for identification.
		/// </summary>
		public abstract string GetTableName();

		/// <summary>
		/// Initializes the table. Can be overridden for custom setup.
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// Gets the component name for display in the Database editor.
		/// </summary>
		/// <returns>The table name</returns>
		public virtual string GetComponentName()
		{
			return GetTableName();
		}
	}


}