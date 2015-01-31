using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class Configuration<TObject> where TObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        Dictionary<string, List<string>> propertyErrors;

        public TObject Object { get; private set; }
        public Action<string> ErrorsChangedAction { get; private set; }
        public Action<string> PropertyChangedAction { get; private set; }
        public Dictionary<string, IConfigurationItem> PropertyConfigurations { get; private set; }
        public IReadOnlyDictionary<string, List<string>> PropertyErrors { get; private set; }

        public Configuration(TObject obj,
            Action<string> propertyChangedAction,
            Action<string> errorsChangedAction)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (propertyChangedAction == null) throw new ArgumentNullException("propertyChangedAction");
            if (errorsChangedAction == null) throw new ArgumentNullException("errorsChangedAction");

            Object = obj;
            PropertyConfigurations = new Dictionary<string, IConfigurationItem>();
            Object.PropertyChanged += Object_PropertyChanged;
            PropertyChangedAction = propertyChangedAction;
            ErrorsChangedAction = errorsChangedAction;
            propertyErrors = new Dictionary<string, List<string>>();
            PropertyErrors = new ReadOnlyDictionary<string, List<string>>(propertyErrors);
        }

        void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChangedAction != null)
            {
                var dependingProps = PropertyConfigurations
                    .Where(pc => pc.Value.Dependencies.Contains(e.PropertyName))
                    .Select(pc => pc.Key)
                    .ToList();
                foreach (var p in dependingProps) PropertyChangedAction(p);
            }

            Validate(e.PropertyName);
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

        #region Property Errors

        public void ClearErrors(string property)
        {
            if (!propertyErrors.ContainsKey(property)) return;

            propertyErrors.Remove(property);

            ErrorsChangedAction(property);
        }

        public void AddError(string property, string error)
        {
            if (propertyErrors.ContainsKey(property))
                propertyErrors[property].Add(error);
            else
                propertyErrors.Add(property, new List<string>() { error });

            ErrorsChangedAction(property);
        }

        public void AddErrors(string property, IEnumerable<string> errors)
        {
            if (!errors.Any()) return;

            if (propertyErrors.ContainsKey(property))
                propertyErrors[property].AddRange(errors);
            else
                propertyErrors.Add(property, errors.ToList());

            ErrorsChangedAction(property);
        }

        public void Validate(string property)
        {
            if (!PropertyConfigurations.ContainsKey(property)) return;

            var value = Dynamitey.Dynamic.InvokeGet(Object, property);
            var errs = PropertyConfigurations[property].Rules
                                                                     .Where(v => !v.Validate(value))
                                                                     .Select(r => r.Message);

            if (propertyErrors.ContainsKey(property)) propertyErrors.Remove(property);
            AddErrors(property, errs);
        }
        
        #endregion
    }
}
