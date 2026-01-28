using UnityEngine;

namespace THEBADDEST.DatabaseModule
{
	/// <summary>
	/// Generic component that holds a single entity/record of type T.
	/// Similar to Table&lt;T&gt; but with only one record instead of a list.
	/// </summary>
	/// <typeparam name="T">The type of entity/record this component holds</typeparam>
	public abstract class SingleEntityComponent<T> : SingleEntityComponentBase
	{
		[SerializeField]
		private T data;

		/// <summary>
		/// The single entity/record data held by this component.
		/// </summary>
		public T Data
		{
			get => data;
			set => data = value;
		}

		/// <summary>
		/// Gets the component name for display in the Database editor.
		/// </summary>
		/// <returns>The component's display name</returns>
		public override string GetComponentName()
		{
			var baseName = base.GetComponentName();
			if (string.IsNullOrEmpty(baseName) || baseName == GetType().Name)
			{
				return typeof(T).Name;
			}
			return baseName;
		}
	}
}
