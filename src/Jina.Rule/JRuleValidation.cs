using eXtensionSharp;
using Jina.ValidationRule.Models;

namespace Jina.ValidationRule;

public class JRuleValidation
{
    
}

public class Sample
{
    public void Run()
    {
        List<Rule> rules = new();
        var rule = new Rule();
        rule.Name = "NotEmpty";
        rule.Behavior = o => o.xIsNotEmpty();
        rule.Message = "{0} is empty";
        rules.Add(rule);

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