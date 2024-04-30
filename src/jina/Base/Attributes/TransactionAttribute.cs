using IsolationLevel = System.Data.IsolationLevel;

namespace Jina.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class TransactionOptionsAttribute : Attribute
	{
		public IsolationLevel IsolationLevel { get; }
		public TimeSpan Timeout { get; }
		
		public ENUM_TRANSACTION_DB_TYPE TransactionDbType { get; }

		
		/// <summary>
		/// Controller에 선언되는 TransactionOptions, EF는 명시적 Transaction, ADO는 TransactionScope로 동작함.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <param name="timeoutSeconds"></param>
		public TransactionOptionsAttribute(
			IsolationLevel isolationLevel = IsolationLevel.Snapshot
#if DEBUG			
			, int timeoutSeconds = 3200
			
#else
			, int timeoutSeconds = 5
#endif
			, ENUM_TRANSACTION_DB_TYPE type = ENUM_TRANSACTION_DB_TYPE.EF
		)
		{
			IsolationLevel = isolationLevel;
			Timeout = TimeSpan.FromSeconds(timeoutSeconds);
			TransactionDbType = type;
		}
	}
	
	public enum ENUM_TRANSACTION_DB_TYPE
	{
		EF,
		ADO
	}
}
