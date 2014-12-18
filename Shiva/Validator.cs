using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public class Validator<TObject> where TObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();
        public IReadOnlyDictionary<string, List<string>> PropertyErrors { get; set; }
        public Configuration<TObject> Configuration { get; private set; }
        Action<string> errorsChangedAction;

        public Validator(Configuration<TObject> config, Action<string> errorsChangedAction)
        {
            propertyErrors = new Dictionary<string, List<string>>();
            PropertyErrors = new ReadOnlyDictionary<string, List<string>>(propertyErrors);
            if (config == null) throw new ArgumentNullException("config");
            Configuration = config;
            this.errorsChangedAction = errorsChangedAction;
        }

        public void ClearErrors(string property)
        {
            if (!propertyErrors.ContainsKey(property)) return;

            propertyErrors.Remove(property);

            if (errorsChangedAction != null) errorsChangedAction(property);
        }

        public void AddError(string property, string error)
        {
            if (propertyErrors.ContainsKey(property))
                propertyErrors[property].Add(error);
            else
                propertyErrors.Add(property, new List<string>() { error });

            if (errorsChangedAction != null) errorsChangedAction(property);
        }

        public void AddErrors(string property, IEnumerable<string> errors)
        {
            if (!errors.Any()) return;

            if (propertyErrors.ContainsKey(property))
                propertyErrors[property].AddRange(errors);
            else
                propertyErrors.Add(property, errors.ToList());

            if (errorsChangedAction != null) errorsChangedAction(property);
        }

        public void Validate(string property)
        {
            if (!Configuration.PropertyConfigurations.ContainsKey(property)) return;

            var value = Dynamitey.Dynamic.InvokeGet(Configuration.Object, property);
            var errs = Configuration.PropertyConfigurations[property].Rules
                                                                     .Where(v => !v.Validate(value))
                                                                     .Select(r => r.Message);

            if (propertyErrors.ContainsKey(property)) propertyErrors.Remove(property);
            AddErrors(property, errs);
        }
    }
}
