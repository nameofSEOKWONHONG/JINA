namespace Jina.Session.Abstract;

public interface ISessionCurrentUserBase
{
    string TenantId { get; }
    string TimeZone { get; }
    string UserId { get; }
    string UserName { get; }
}
    