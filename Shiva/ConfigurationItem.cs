using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class ConfigurationItem<T> : IConfigurationItem
    {
        public List<IValidationRule> Rules { get; private set; }
        public List<string> Dependencies { get; private set; }

        public ConfigurationItem()
        {
            Rules = new List<IValidationRule>();
            Dependencies = new List<string>();
        }

        public ConfigurationItem<T> Enforce(Func<T, bool> rule, string message)
        {
            Rules.Add(new ValidationRule<T> { Rule = rule, Message = message });
            return this;
        }

        public ConfigurationItem<T> DependsOn(Expression<Func<T>> selectorExpression)
        {
             Dependencies.Add(PropertyEx.Name(selectorExpression));
            return this;
        }
    }
}
