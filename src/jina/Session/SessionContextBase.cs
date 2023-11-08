using Jina.Session.Abstract;

namespace Jina.Session;

public class SessionContextBase : ISessionContextBase
{
    public string TenantId { get; set; }
    public ISessionCurrentUserBase CurrentUser { get; }
    public ISessionDateTimeBase CurrentTime { get; }

    public SessionContextBase(string tenantId
        , ISessionCurrentUserBase currentUser
        , ISessionDateTimeBase dateTimeBase)
    {
        TenantId = tenantId;
        CurrentUser = currentUser;
        CurrentTime = dateTimeBase;
    }
}