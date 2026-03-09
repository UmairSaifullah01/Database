namespace THEBADDEST
{


	public interface IServiceLocator
	{
		void     RegisterService<TService>(TService service);
		void     UnregisterService<TService>();
		TService GetService<TService>();
		bool     TryGetService<TService>(out TService service);

		void Clear();

	}


}