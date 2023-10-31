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
    private readonly List<IEnumerable<RuleParameter>> _rows;
    private readonly IEnumerable<Rule> _rules;
    
    public RuleBuilder(IEnumerable<RuleParameter> row
    , IEnumerable<Rule> rules)
    {
        _rows = new List<IEnumerable<RuleParameter>> { row };
        _rules = rules;
    }

    public RuleBuilder(List<IEnumerable<RuleParameter>> rows
        , IEnumerable<Rule> rules)
    {
        _rows = rows;
        _rules = rules;
    }
    
    public IEnumerable<RuleResult> Validate()
    {
        List<RuleResult> ruleResults = new();
        
        foreach (var row in _rows)
        {
            foreach (var col in row)
            {
                var selectedRule = _rules.First(m => col.RuleName == m.Name);
                var result = selectedRule.Behavior(col.Value);
                if (result.xIsFalse())
                {
                    ruleResults.Add(new RuleResult()
                    {
                        PropertyName = col.Key,
                        Message = string.Format(selectedRule.Message, col.Key)
                    });
                }    
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