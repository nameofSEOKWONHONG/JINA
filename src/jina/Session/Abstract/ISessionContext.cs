namespace Jina.Session.Abstract;

public interface ISessionContext
{
    public string TenantId { get; }

    /// <summary>
    /// Token 정보 중 사용자 정보
    /// </summary>
    public ISessionCurrentUser CurrentUser { get; }

    /// <summary>
    /// UTC 타임 정보
    /// </summary>
    public ISessionDateTime CurrentTime { get; }
}