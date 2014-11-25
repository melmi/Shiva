using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class ValidationRule<T> : IValidationRule
    {
        public Func<T, bool> Rule { get; set; }
        public string Message { get; set; }
        public bool Validate(object value)
        {
            return Rule((T)value);
        } 
    }
}
