using eXtensionSharp;
using Jina.Validate.RuleValidate.Abstract;
using System.Reflection;

namespace Jina.Validate.RuleValidate
{
    public class JRuleValidatorEngine
    {
        private static Lazy<JRuleValidatorEngine> _instance = new Lazy<JRuleValidatorEngine>(() => new JRuleValidatorEngine());
        public static JRuleValidatorEngine EngineInstance => _instance.Value;

        private readonly IEnumerable<IRuleValidator> _ruleValidators;

        private JRuleValidatorEngine()
        {
            _ruleValidators = GetRules();
        }

        public bool TryValidate(RuleValidateRequest request, out string message)
        {
            message = string.Empty;

            var rule = _ruleValidators.First(m => m.ValidateRule == request.ValidateRule);

            rule.Prepare(null);
            var result = rule.Execute(request.Key, request.Value);
            if (result.IsVaild.xIsFalse())
            {
                message = result.Message;
                return false;
            }

            return true;
        }

        private IEnumerable<IRuleValidator> GetRules()
        {
            var type = typeof(IRuleValidator);

            var rules = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsAssignableFrom(x) && !x.IsInterface)
                .Select(x => Activator.CreateInstance(x).xAs<IRuleValidator>())
                .ToList();

            return rules;
        }
    }
}