using eXtensionSharp;
using Hangfire;
using Jina.Database.Abstract;
using Jina.Lang.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Jina.Session.Abstract;

public interface ISessionContext
{
    bool IsUnknown => TenantId.xIsEmpty();

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
    
    IDbContext DbContext { get; }
    
    IDbProviderBase DbProvider { get; }

    IHttpContextAccessor HttpContextAccessor { get; }
    
    IHttpClientFactory HttpClientFactory { get; }
    
    IBackgroundJobClient JobClient { get; }
    
    IDistributedCache DistributedCache { get; }

    bool IsDecrypt { get; set; }
}