namespace THEBADDEST.DatabaseModule
{


	public interface ITable
	{

		/// <summary>
		/// Returns the table name for identification.
		/// </summary>
		string GetTableName();

		/// <summary>
		/// Initializes the table. Can be overridden for custom setup.
		/// </summary>
		void Initialize();

	}
	public interface ITable<TRecord> :ITable
	{
		/// <summary>
		/// Retrieves a record from the table.
		/// </summary>
		/// <returns>A record from the table.</returns>
		TRecord GetRecord(int id);
		
		/// <summary>
		/// Removes a record from the table.
		/// </summary>
		/// <param name="id">The ID of the record to remove.</param>
		void RemoveRecord(int id);

		/// <summary>
		/// Adds a new record to the table.
		/// </summary>
		/// <param name="record">The record to add.</param>
		void AddRecord(TRecord record);

		/// <summary>
		/// Updates an existing record in the table.
		/// </summary>
		/// <param name="id">The ID of the record to update.</param>
		/// <param name="record">The new record data.</param>
		void UpdateRecord(int id, TRecord record);
	}

}