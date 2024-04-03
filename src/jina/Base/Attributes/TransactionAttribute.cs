using System.Transactions;

namespace Jina.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TransactionOptionsAttribute : Attribute
	{
		public TransactionScopeOption ScopeOption { get; }
		public IsolationLevel IsolationLevel { get; }
		public TimeSpan Timeout { get; }

		public TransactionOptionsAttribute(TransactionScopeOption scopeOption, IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted, int timeoutSeconds = 5)
		{
			ScopeOption = scopeOption;
			IsolationLevel = isolationLevel;
			Timeout = TimeSpan.FromSeconds(timeoutSeconds);
		}
	}
}
