using eXtensionSharp;
using Hangfire;
using Jina.Database;
using Jina.Database.Abstract;
using Jina.Lang.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;

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
    
    IDbContext DbContext { get; }
    
    IDbProviderBase DbProvider { get; }

    IHttpContextAccessor HttpContextAccessor { get; }
    
    IHttpClientFactory HttpClientFactory { get; }
    
    IBackgroundJobClient JobClient { get; }
    
    IDistributedCache DistributedCache { get; }

    IFSql FSql { get; }
    
    bool IsDecrypt { get; set; }
}

public interface ISessionContextInitializer
{
    Task InitializeAsync(IdentityUser<string> user);
}