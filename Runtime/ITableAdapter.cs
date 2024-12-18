namespace THEBADDEST.DatabaseModule
{


	public interface ITableAdapter
	{
		void Serialize<T>(ITable<T>  table);

		void Deserialize<T>(ITable<T> table);

	}


}