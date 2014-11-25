using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class Configuration
    {
        public Dictionary<string, IConfigurationItem> PropertyConfigurations { get; private set; }

        public Configuration()
        {
            PropertyConfigurations = new Dictionary<string, IConfigurationItem>();
        }

        public List<string> Validate(string propertyName, object value)
        {
            if (!PropertyConfigurations.ContainsKey(propertyName)) return null;
            var errs = PropertyConfigurations[propertyName].Rules
                                                           .Where(v => !v.Validate(value))
                                                           .Select(r => r.Message)
                                                           .ToList();
            return errs.Count == 0 ? null : errs;
        }

        public ConfigurationItem<T> Property<T>(Expression<Func<T>> selectorExpression)
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            ConfigurationItem<T> config;
            if (PropertyConfigurations.ContainsKey(propertyName))
                config = PropertyConfigurations[propertyName] as ConfigurationItem<T>;
            else
            {
                config = new ConfigurationItem<T>();
                PropertyConfigurations.Add(propertyName, config);
            }
            return config;
        }
    }
}
