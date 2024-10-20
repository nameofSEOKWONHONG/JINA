using eXtensionSharp;
using Jina.Base.Service;
using Jina.Session.Abstract;
using Jina.Validate;
using Microsoft.Extensions.Logging;

namespace Jina.Test.Services;

public class TestJService
    : ServiceImplCore<TestJService, Request, string>
        , ITestService
{
    public TestJService(ILogger<TestJService> logger) : base(logger)
    {
        
    }

    public ISessionContext Context { get; }

    public override Task<bool> OnExecutingAsync()
    {
        if (this.Request.xIsEmpty())
        {
            this.Result = "failed";
            return Task.FromResult(false);
        }

        if (this.Request.Name.xIsEmpty())
        {
            this.Result = "failed";
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync()
    {
        this.Result = $"Hello, {this.Request.Name}";
        return Task.CompletedTask;
    }
}

public class Request
{
    public string Name { get; set; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        NotEmpty(m => m.Name);
    }
}