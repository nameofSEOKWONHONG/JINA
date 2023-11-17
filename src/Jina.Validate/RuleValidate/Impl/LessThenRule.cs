using eXtensionSharp;
using Jina.Validate.RuleValidate.Abstract;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Validate.RuleValidate.Impl
{
    public class LessThenRule : RuleValidatorBase, IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.LessThen;

        public LessThenRule()
        { }

        public override RuleValidateResult Execute(RuleValidateOption option)
        {
            var result = option.CompareValue.xValue<double>() > option.Value.xValue<double>();

            return new RuleValidateResult()
            {
                IsVaild = !result,
                Message = result.xIsFalse()
                    ? option.CustomMessage.xIsNotEmpty()
                        ? option.CustomMessage
                            : Localizer.xIsNotEmpty()
                                ? Localizer[""]
                                    : $"{option.Key}(은)는 {option.CompareValue} 보다 작아야 합니다."
                    : string.Empty
            };
        }
    }
}