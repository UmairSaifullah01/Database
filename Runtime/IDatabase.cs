namespace THEBADDEST.DatabaseModule
{


	public interface IDatabase
	{
		void Initialize();

		T GetTable<T>() where T : TableBase;

		TableBase GetTableByName(string tableName);

		void AddTable(TableBase table);
	}


}