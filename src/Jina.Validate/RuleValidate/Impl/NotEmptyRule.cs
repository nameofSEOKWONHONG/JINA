using eXtensionSharp;
using Jina.Validate.RuleValidate.Abstract;

namespace Jina.Validate.RuleValidate.Impl
{
	public class NotEmptyRule : RuleValidatorBase, IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.NotEmpty;

        public override RuleValidateResult Execute(RuleValidateOption option)
        {
            var result = option.Value.xIsNotEmpty();

            return new RuleValidateResult()
            {
                IsVaild = !result,
                Message = result.xIsFalse()
                    ? option.CustomMessage.xIsNotEmpty()
                        ? option.CustomMessage
                            : Localizer.xIsNotEmpty()
                                ? Localizer[""]
                                    : $"{option.Key}(은)는 빈값일 수 없습니다."
                    : string.Empty
            };
        }
    }
}