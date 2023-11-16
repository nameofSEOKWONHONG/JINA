using Jina.Lang.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Validate.RuleValidate.Abstract
{
    public abstract class RuleValidatorBase
    {
        protected ILocalizer Localizer;

        protected RuleValidatorBase()
        {
        }

        public virtual void Prepare(ILocalizer localizer)
        {
            this.Localizer = localizer;
        }

        public abstract RuleValidateResult Execute(RuleValidateRequest request);
    }
}