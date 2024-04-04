using Jina.Lang.Abstract;

namespace Jina.Session.Abstract;

public interface ISessionContext
{
    string TenantId { get; }

    /// <summary>
    /// Token 정보 중 사용자 정보
    /// </summary>
    ISessionCurrentUser CurrentUser { get; }

    /// <summary>
    /// UTC 타임 정보
    /// </summary>
    ISessionDateTime CurrentTime { get; }

    ILocalizer Localizer { get; }

    bool IsDecrypt { get; }
}