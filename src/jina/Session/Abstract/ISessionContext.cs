namespace Jina.Session.Abstract;

/// <summary>
/// 작성자 : 홍석원
/// 작성일 : 2023.11.08
/// 구현내용 : SessionContext 정보
/// 비고 :
/// 수정내역 :
/// </summary>
public interface ISessionContext
{
    public string TenantId { get; set; }
    /// <summary>
    /// Token 정보 중 사용자 정보
    /// </summary>
    public ISessionCurrentUser CurrentUser { get; }
    /// <summary>
    /// UTC 타임 정보
    /// </summary>
    public ISessionDateTime CurrentTime { get; }
}