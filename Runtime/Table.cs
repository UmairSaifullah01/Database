using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public abstract class Table<T> : TableBase,ITable<T>
	{
		[SerializeField] private string  tableName;
		[SerializeField] private List<T> entries = new List<T>();

		public IReadOnlyList<T> Entries => entries;

		public override string GetTableName() => string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;

		public T GetRecord(int id)
		{
			return id < entries.Count ? entries[id] : default;
		}

		public void RemoveRecord(int id)
		{
			if (id < entries.Count)
			{
				entries.RemoveAt(id);
			}	
		}

		public void AddRecord(T record)
		{
			entries.Add(record);
		}

		public void UpdateRecord(int id, T record)
		{
			entries[id] = record;
		}
		
	}


}