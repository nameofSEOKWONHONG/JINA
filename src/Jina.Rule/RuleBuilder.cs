using eXtensionSharp;
using Jina.ValidationRule.Models;

namespace Jina.ValidationRule;

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
    
    public bool TryValidate(out IEnumerable<RuleResult> validateResults)
    {
        validateResults = null;

        List<RuleResult> results = new List<RuleResult>();
        foreach (var row in _rows)
        {
            foreach (var col in row)
            {
                var selectedRule = _rules.First(m => col.RuleName == m.Name);
                var result = selectedRule.Behavior(col.Value);
                if (result.xIsFalse())
                {
                    results.Add(new RuleResult()
                    {
                        PropertyName = col.Key,
                        Message = string.Format(selectedRule.Message, col.Key)
                    });
                }    
            }
        }

        validateResults = results;

        return results.xIsEmpty();
    }
}