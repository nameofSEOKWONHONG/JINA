using eXtensionSharp;
using Jina.Lang.Abstract;
using Jina.Validate.RuleValidate.Abstract;

namespace Jina.Validate.RuleValidate.Impl
{
    public class NotEmptyRule : RuleValidatorBase, IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.NotEmpty;

        public override RuleValidateResult Execute(RuleValidateRequest request)
        {
            var result = request.Value.xIsNotEmpty();

            return new RuleValidateResult()
            {
                IsVaild = !result,
                Message = Localizer.xIsNotEmpty()
                    ? Localizer[""].xValue($"{request.Key}(은)는 빈값일 수 없습니다.")
                    : $"{request.Key}(은)는 빈값일 수 없습니다."
            };
        }
    }
}