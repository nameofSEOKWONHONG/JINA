using eXtensionSharp;
using IsolationLevel = System.Data.IsolationLevel;

namespace Jina.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class TransactionOptionsAttribute : Attribute
	{
		public IsolationLevel IsolationLevel { get; }
		public TimeSpan Timeout { get; }
		
		public ENUM_DB_PROVIDER_TYPE DbProviderType { get; }

		
		/// <summary>
		/// Controller에 선언되는 TransactionOptions, EF는 명시적 Transaction, ADO는 TransactionScope로 동작함.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <param name="timeoutSeconds"></param>
		public TransactionOptionsAttribute(
			IsolationLevel isolationLevel = IsolationLevel.Snapshot
			, ENUM_DB_PROVIDER_TYPE providerType = ENUM_DB_PROVIDER_TYPE.EF
			, int timeoutSeconds = 5
		)
		{
			IsolationLevel = isolationLevel;
			#if DEBUG
			Timeout = TimeSpan.FromSeconds(3200);
			#else
			Timeout = TimeSpan.FromSeconds(timeoutSeconds);
			#endif
			DbProviderType = providerType;
		}
	}
	
	public enum ENUM_DB_PROVIDER_TYPE
	{
		EF,
		FreeSql
	}
}
