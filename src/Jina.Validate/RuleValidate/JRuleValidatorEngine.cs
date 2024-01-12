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

        public bool TryValidate(RuleValidateOption option, out string message)
        {
            var rule = _ruleValidators.First(m => m.ValidateRule == option.ValidateRule).xAs<RuleValidatorBase>();
            rule.Prepare(null);
            var result = rule.Execute(option);
            message = result.Message;
            return result.IsVaild;
        }

        public IEnumerable<(bool IsValid, string Key, string Message)> Validates(IEnumerable<RuleValidateOption> requests)
        {
            var result = new List<(bool IsValid, string Key, string Message)>();
            foreach (var item in requests)
            {
                if (TryValidate(item, out var msg))
                {
                    result.Add(new(false, item.Key, msg));
                }
                else
                {
                    result.Add(new(true, item.Key, string.Empty));
                }
            }
            return result;
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