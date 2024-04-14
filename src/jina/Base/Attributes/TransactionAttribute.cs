using System.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace Jina.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class TransactionOptionsAttribute : Attribute
	{
		public IsolationLevel IsolationLevel { get; }
		public TimeSpan Timeout { get; }

		public TransactionOptionsAttribute(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted, int timeoutSeconds = 3200)
		{
			IsolationLevel = isolationLevel;
			Timeout = TimeSpan.FromSeconds(timeoutSeconds);
		}
	}
}
