using eXtensionSharp;
using Jina.Base.Service;
using Jina.Validate;

namespace Jina.Test.Services;

public class TestJService
    : ServiceImplBase<TestJService, Request, string>
        , ITestService
{
    public TestJService()
    {
    }

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