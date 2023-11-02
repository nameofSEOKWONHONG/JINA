namespace Jina.ValidationRule.Models;

public class RulePattern
{
    public string Name { get; set; }

    public Func<object, bool> Behavior { get; set; }
    
    public string Message { get; set; }
}