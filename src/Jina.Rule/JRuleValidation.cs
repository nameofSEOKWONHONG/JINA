using System.Xml.XPath;
using eXtensionSharp;

namespace Jina.ValidationRule;

public class JRuleValidation
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

    }
    
    
}

public class RuleBuilder
{
    private readonly IEnumerable<RuleParameter> _row;
    private readonly IEnumerable<Rule> _rules;
    public RuleBuilder(IEnumerable<RuleParameter> row
    , IEnumerable<Rule> rules)
    {
        _row = row;
        _rules = rules;
    }
    

    public IEnumerable<RuleResult> Execute()
    {
        List<RuleResult> ruleResults = new();
        
        foreach (var row in _row)
        {
            var selectedRule = _rules.First(m => row.RuleName == m.Name);
            var result = selectedRule.Behavior(row.Value);
            if (result.xIsFalse())
            {
                ruleResults.Add(new RuleResult()
                {
                    PropertyName = row.Key,
                    Message = string.Format(selectedRule.Message, row.Key)
                });
            }
        }

        return ruleResults;
    }
}

public class Rule
{
    public string Name { get; set; }

    public Func<object, bool> Behavior { get; set; }
    
    public string Message { get; set; }
}

public class RuleParameter
{
    public string Key { get; set; }
    public object Value { get; set; }
    public string RuleName { get; set; }
}

public class RuleResult
{
    public string PropertyName { get; set; }
    public string Message { get; set; }
}