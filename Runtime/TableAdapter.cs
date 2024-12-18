using UnityEngine;


namespace THEBADDEST.DatabaseModule
{
	
	public abstract class TableAdapter : ScriptableObject, ITableAdapter
	{
		
		public abstract void Serialize<T>(ITable<T> table);

		public abstract void Deserialize<T>(ITable<T> table);

	}


}