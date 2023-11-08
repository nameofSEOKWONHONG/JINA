namespace Jina.Session.Abstract;

public interface ISessionDateTimeBase
{
    DateTime Now { get; }
    DateTime ToLocal(DateTime utc);
    DateTime ToUtc(DateTime local);
}