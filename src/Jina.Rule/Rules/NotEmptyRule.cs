using eXtensionSharp;
using Jina.ValidationRule.Models;
using Jina.ValidationRule.Rules.Abstract;

namespace Jina.ValidationRule.Rules;

public class NotEmptyRule : IRule
{
    public NotEmptyRule()
    {
        
    }

    public RulePattern Execute()
    {
        return new RulePattern()
        {
            Name = "NotEmpty",
            Behavior = o => o.xIsNotEmpty(),
            Message = "{0} is empty",
        };
    }
}