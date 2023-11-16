using eXtensionSharp;
using Jina.Lang.Abstract;
using Jina.Validate.RuleValidate.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Validate.RuleValidate.Impl
{
    public class NotEmptyRule : IRuleValidator
    {
        public ENUM_VALIDATE_RULE ValidateRule => ENUM_VALIDATE_RULE.NotEmpty;

        private ILocalizer _localizer;

        public void Prepare(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        public RuleValidateResult Execute(string key, object o)
        {
            var result = o.xIsNotEmpty();

            return new RuleValidateResult()
            {
                IsVaild = result,
                Message = _localizer[""].xValue($"{key}는 빈값일 수 없습니다.")
            };
        }
    }
}