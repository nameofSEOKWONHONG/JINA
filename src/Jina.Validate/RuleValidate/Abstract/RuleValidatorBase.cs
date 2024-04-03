using Jina.Lang.Abstract;

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

        public abstract RuleValidateResult Execute(RuleValidateOption request);
    }
}