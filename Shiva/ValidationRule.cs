using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class ValidationRule<T> : IValidationRule
    {
        public Func<T, bool> Rule { get; private set; }
        public string Message { get; private set; }
        
        public ValidationRule(Func<T,bool> rule, string message)
        {
            if (rule == null) throw new ArgumentNullException("rule");
            if (message == null) throw new ArgumentNullException("message");

            Rule = rule;
            Message = message;
        }
        
            public bool Validate(object value)
        {
            return Rule((T)value);
        } 
    }
}
