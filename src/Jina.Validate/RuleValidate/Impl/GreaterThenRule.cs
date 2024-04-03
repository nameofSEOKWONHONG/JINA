using eXtensionSharp;
using Jina.Validate.RuleValidate.Abstract;

namespace Jina.Validate.RuleValidate.Impl
{
	public class GreaterThenRule : RuleValidatorBase, IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.GraterThen;

        public override RuleValidateResult Execute(RuleValidateOption option)
        {
            var result = option.CompareValue.xValue<double>() < option.Value.xValue<double>();

            return new RuleValidateResult()
            {
                IsVaild = !result,
                Message = result.xIsFalse()
                    ? option.CustomMessage.xIsNotEmpty()
                        ? option.CustomMessage
                            : Localizer.xIsNotEmpty()
                                ? Localizer[""]
                                    : $"{option.Key}(은)는 {option.CompareValue} 보다 커야 합니다."
                    : string.Empty
            };
        }
    }
}