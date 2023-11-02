using Jina.ValidationRule.Models;

namespace Jina.ValidationRule.Rules.Abstract;

public interface IRule
{
    RulePattern Execute();
}