namespace Jina.Session.Abstract;

public interface ISessionCurrentUser
{
    string TenantId { get; }
    string Email { get; }
    string TimeZone { get; }
    string UserId { get; }
    string UserName { get; }
    string RoleName { get; }
    List<KeyValuePair<string, string>> Claims { get; }
}