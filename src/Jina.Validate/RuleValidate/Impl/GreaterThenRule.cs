using eXtensionSharp;
using Jina.Lang;
using Jina.Lang.Abstract;
using Jina.Validate.RuleValidate.Abstract;

namespace Jina.Validate.RuleValidate.Impl
{
    public class GreaterThenRule : RuleValidatorBase, IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.GraterThen;

        public override RuleValidateResult Execute(RuleValidateRequest request)
        {
            var result = request.CompareValue.xValue<Int32>() < request.Value.xValue<Int32>();

            return new RuleValidateResult()
            {
                IsVaild = !result,
                Message = Localizer.xIsNotEmpty()
                    ? Localizer[""].xValue($"{request.Key}(은)는 {request.CompareValue} 보다 커야 합니다.")
                    : $"{request.Key}(은)는 {request.CompareValue} 보다 커야 합니다."
            };
        }
    }
}