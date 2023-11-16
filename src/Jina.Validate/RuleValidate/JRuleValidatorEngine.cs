using eXtensionSharp;
using Jina.Validate.RuleValidate.Abstract;
using System.Reflection;

namespace Jina.Validate.RuleValidate
{
    public class JRuleValidatorEngine
    {
        private static Lazy<JRuleValidatorEngine> _instance = new Lazy<JRuleValidatorEngine>(() => new JRuleValidatorEngine());
        public static JRuleValidatorEngine Engine => _instance.Value;

        private readonly IEnumerable<IRuleValidator> _ruleValidators;

        private JRuleValidatorEngine()
        {
            _ruleValidators = GetRules();
        }

        public bool TryValidate(RuleValidateRequest request, out string message)
        {
            message = string.Empty;

            var rule = _ruleValidators.First(m => m.ValidateRule == request.ValidateRule).xAs<RuleValidatorBase>();
            rule.Prepare(null);
            var result = rule.Execute(request);
            message = result.Message;
            return result.IsVaild;
        }

        private IEnumerable<IRuleValidator> GetRules()
        {
            var type = typeof(IRuleValidator);
            var rules = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface)
                .Select(x => Activator.CreateInstance(x).xAs<IRuleValidator>())
                .ToList();

            return rules;
        }
    }
}