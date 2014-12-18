using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class Configuration<TObject> where TObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public TObject Object { get; private set; }
        public Dictionary<string, IConfigurationItem> PropertyConfigurations { get; private set; }
        public Validator<TObject> Validator { get; private set; }
        Action<string> propertyChangedAction;

        public Configuration(TObject obj,
            Action<string> propertyChangedAction,
            Action<string> errorsChangedAction)
        {
            PropertyConfigurations = new Dictionary<string, IConfigurationItem>();
            if (obj == null) throw new ArgumentNullException("obj");
            Object = obj;
            Object.PropertyChanged += Object_PropertyChanged;
            this.propertyChangedAction = propertyChangedAction;
            Validator = new Validator<TObject>(this, errorsChangedAction);
        }

        void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (propertyChangedAction != null)
            {
                var dependingProps = PropertyConfigurations
                    .Where(pc => pc.Value.Dependencies.Contains(e.PropertyName))
                    .Select(pc => pc.Key)
                    .ToList();
                foreach (var p in dependingProps) propertyChangedAction(p);
            }

            Validator.Validate(e.PropertyName);
        }

        public ConfigurationItem<T> Property<T>(Expression<Func<T>> selectorExpression)
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            ConfigurationItem<T> configItem;
            if (PropertyConfigurations.ContainsKey(propertyName))
                configItem = PropertyConfigurations[propertyName] as ConfigurationItem<T>;
            else
            {
                configItem = new ConfigurationItem<T>();
                PropertyConfigurations.Add(propertyName, configItem);
            }
            return configItem;
        }
    }
}
