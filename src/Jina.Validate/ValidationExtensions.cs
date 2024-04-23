using FluentValidation.Results;

namespace Jina.Validate;

public static class ValidationExtensions
{
    public static Dictionary<string, string> vToDictionary(this List<ValidationFailure> items)
    {
        return items.Select(m => new KeyValuePair<string, string>(key: m.PropertyName, value: m.ErrorMessage))
            .ToDictionary();
    }
}