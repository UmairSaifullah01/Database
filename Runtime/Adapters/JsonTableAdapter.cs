using System.Linq;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{

	[CreateAssetMenu(menuName = "THEBADDEST/Database/JsonTableAdapter", fileName = "JsonTableAdapter")]
	public class JsonTableAdapter : TableAdapter
	{
		[SerializeField]public string json = "";
		public override void Serialize<T>(ITable<T> table)
		{
			var wrapper = new JsonWrapper<T> { records = table.Entries.ToArray() };
			json=JsonUtility.ToJson(wrapper);
		}
		
		public override void Deserialize<T>(ITable<T> table)
		{
			if (string.IsNullOrEmpty(json)) return;
			var records = JsonUtility.FromJson<JsonWrapper<T>>(json).records;
			table.Clear();
			if (records != null)
			{
				foreach (var record in records)
				{
					if (record != null)
						table.AddRecord(record);
				}
			}
		}
		
		private class JsonWrapper<T>
		{
			public T[] records;
		}
	}


}