using eXtensionSharp;
using Jina.Base.Service;
using Jina.Base.Service.Abstract;
using Jina.Validate;
using Jina.Validate.RuleValidate;
using Microsoft.Extensions.DependencyInjection;
using System;
using Jina.Test.Services;

namespace Jina.Test;

public class Tests
{
#pragma warning disable NUnit1032
    private ServiceProvider _serviceProvider;
#pragma warning restore NUnit1032

    public Tests()
    {
    }

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

        await ServiceInvoker<Request, string>.Invoke(service)
            .AddFilter(() => request.xIsNotEmpty())
            .SetParameter(() => new()
            {
                Name = request
            })
            .SetValidator(new RequestValidator())
            .OnValidated(r => result = r.Errors.Select(m => m.ErrorMessage).First())
            .OnExecutedAsync(r => result = r); ;

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void object_to_notempty()
    {
        object o = "";
        Assert.IsTrue(o.xIsNotEmpty());
    }

    [Test]
    public void rule_validator_test()
    {
        if (JRuleValidatorEngine.Engine.TryValidate(new Validate.RuleValidate.Abstract.RuleValidateOption()
        {
            ValidateRule = ENUM_VALIDATE_RULE.NotEmpty,
            Key = "Name",
            Value = "hello"
        }, out var messageA))
        {
            Assert.That(messageA, Is.Not.Empty);
            TestContext.Out.WriteLine(messageA);
        }

        if (JRuleValidatorEngine.Engine.TryValidate(new Validate.RuleValidate.Abstract.RuleValidateOption()
        {
            ValidateRule = ENUM_VALIDATE_RULE.GraterThen,
            Key = "Age",
            Value = 31,
            CompareValue = 30
        }, out var messageB))
        {
            Assert.That(messageB, Is.Not.Empty);
            TestContext.Out.WriteLine(messageB);
        }
    }
}