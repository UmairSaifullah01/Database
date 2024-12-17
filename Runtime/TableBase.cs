using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public abstract class TableBase : ScriptableObject ,ITable
	{
		/// <summary>
		/// Returns the table name for identification.
		/// </summary>
		public abstract string GetTableName();

		/// <summary>
		/// Initializes the table. Can be overridden for custom setup.
		/// </summary>
		public virtual void Initialize() { }
	}


}