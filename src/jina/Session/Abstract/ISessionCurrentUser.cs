namespace Jina.Session.Abstract;

public interface ISessionCurrentUser
{
    string TenantId { get; }
    string TimeZone { get; }
    string UserId { get; }
    string UserName { get; }
}
    