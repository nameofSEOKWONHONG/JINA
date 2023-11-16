namespace Jina.Session.Abstract;

public interface ISessionDateTime
{
    DateTime Now { get; }
    DateTime ToLocal(DateTime utc);
    DateTime ToUtc(DateTime local);
}