using Jina.Validate.RuleValidate.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Validate.RuleValidate.Abstract
{
    public class RuleValidateRequest
    {
        public ENUM_VALIDATE_RULE ValidateRule { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public object CompareValue { get; set; }
    }

    public class RuleValidateResult
    {
        public bool IsVaild { get; set; }
        public string Message { get; set; }
    }
}