using System.Linq.Expressions;
using eXtensionSharp;
using FluentValidation;
using Jina.Lang.Abstract;

namespace Jina.Validate;

public abstract class Validator<T> : AbstractValidator<T>
{
    protected ILocalizer Localizer;
    protected Validator()
    {
    }

    protected Validator(ILocalizer localizer)
    {
        Localizer = localizer;
    }

    protected void NotEmpty<TProperty>(Expression<Func<T, TProperty>> expression, string message = null)
    {
        var builder = RuleFor(expression)
                .NotEmpty()
            ;

        if (message.xIsNotEmpty())
        {
            builder.WithMessage(message);
        }
    }

    protected void MinLength(Expression<Func<T, string>> expression, int minLimit)
    {
        var rule = RuleFor(expression);
        rule.MinimumLength(minLimit)
            ;
    }

    protected void MaxLength(Expression<Func<T, string>> expression, int maxLimit)
    {
        var rule = RuleFor(expression);
        rule.MaximumLength(maxLimit)
            ;
    }

    protected void MinMaxLength(Expression<Func<T, string>> expression, int minLimit, int maxLimit)
    {
        var rule = RuleFor(expression);
        if (minLimit.xIsNotEmptyNumber())
        {
            rule.MinimumLength(minLimit)
                ;
        }
        if (maxLimit.xIsNotEmptyNumber())
        {
            rule.MaximumLength(maxLimit)
                ;
        }
    }

    protected void GreaterThan<TProperty>(Expression<Func<T, TProperty>> expression, TProperty limit)
        where TProperty : IComparable<TProperty>, IComparable
    {
        RuleFor(expression)
            .GreaterThan(limit)
            ;
    }

    protected void LessThan<TProperty>(Expression<Func<T, TProperty>> expression, TProperty limit)
        where TProperty : IComparable<TProperty>, IComparable
    {
        RuleFor(expression)
            .LessThan(limit)
            ;
    }
        
    private IEnumerable<string> ValidateValue(T arg)
    {
        var result = Validate(arg);
        if (result.IsValid)
            return new string[0];
        return result.Errors.Select(e => e.ErrorMessage);
    }

    public Func<T, IEnumerable<string>> Validation => ValidateValue;
}