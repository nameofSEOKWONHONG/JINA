using Jina.Lang.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Validate.RuleValidate.Abstract
{
    public interface IRuleValidator
    {
        ENUM_VALIDATE_RULE ValidateRule { get; }

        void Prepare(ILocalizer localizer);

        RuleValidateResult Execute(string key, object o);
    }
}