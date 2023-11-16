using Jina.Session.Abstract;

namespace Jina.Session;

public class SessionContextBase : ISessionContext
{
    public string TenantId { get; set; }
    public ISessionCurrentUser CurrentUser { get; }
    public ISessionDateTime CurrentTime { get; }

    public SessionContextBase(string tenantId
        , ISessionCurrentUser currentUser
        , ISessionDateTime dateTime)
    {
        TenantId = tenantId;
        CurrentUser = currentUser;
        CurrentTime = dateTime;
    }
}