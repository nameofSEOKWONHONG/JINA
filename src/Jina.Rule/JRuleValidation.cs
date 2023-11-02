using eXtensionSharp;
using Jina.ValidationRule.Models;
using Jina.ValidationRule.Rules;
using Jina.ValidationRule.Rules.Abstract;

namespace Jina.ValidationRule;

public class JRuleValidation
{
    
}

public class Sample
{
    public void Run()
    {
        var rulePatterns = new List<IRule>();
        rulePatterns.Add(new NotEmptyRule());

        var rules = new List<RulePattern>();
        foreach (var rulePattern in rulePatterns)
        {
            rules.Add(rulePattern.Execute());
        }
        
        List<RuleParameter> row = new();
        row.Add(new RuleParameter()
        {
            Key = "Name",
            Value = "",
            RuleName = "NotEmpty"
        });
        
        var ruleBuilder = new RuleBuilder(row, rules);
        if (ruleBuilder.TryValidate(out var results))
        {
            foreach (var item in results)
            {
                
            }
        }
    }
}