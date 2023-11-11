using eXtensionSharp;
using Jina.Base.Service;
using Jina.Base.Service.Abstract;
using Jina.Base.Validator;
using Microsoft.Extensions.DependencyInjection;

namespace Jina.Test;

public class Tests
{   
    ServiceProvider _serviceProvider;
    [SetUp]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddScoped<ITestService, TestJService>();
        _serviceProvider = sc.BuildServiceProvider();
    }

    [Test]
    public async Task Test1()
    {
        var expected = "Hello, Jina";
        var request = "Jina";
        string result = string.Empty;
        
        var service = _serviceProvider.GetService<ITestService>();

        JServiceInvoker<Request, string>.Invoke(service)
            .AddFilter(() => request.xIsNotEmpty())
            .SetParameter(() => new()
            {
                Name = request
            })
            .SetValidator(new RequestValidator())
            .OnValidated(r => result = r.Errors.Select(m => m.ErrorMessage).First())
            .ExecutedAsync(r => result = r);;
        
        Assert.That(result, Is.EqualTo(expected));
    }
}

public interface ITestService
    : IServiceImplBase<Request, string>
        , IScopeService
{
    
}

public class TestJService
    : JServiceImplBase<TestJService, Request, string>
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

public class RequestValidator : JValidatorBase<Request>
{
    public RequestValidator()
    {
        NotEmpty(m => m.Name);
    }
}
 