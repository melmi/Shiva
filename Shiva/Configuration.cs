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
    public class Configuration<T> where T : class, new()
    {
        Dictionary<string, List<string>> propertyErrors;

        public ViewModelProxy<T> ViewModel { get; private set; }
        public Action<string> ErrorsChangedAction { get; private set; }
        public Action<string> PropertyChangedAction { get; private set; }
        public Dictionary<string, IConfigurationItem> PropertyConfigurations { get; private set; }
        public IReadOnlyDictionary<string, List<string>> PropertyErrors { get; private set; }
        public Dictionary<string, IWrapper> Wrappers { get; private set; }

        public Configuration(ViewModelProxy<T> viewModel,
            Action<string> propertyChangedAction,
            Action<string> errorsChangedAction)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (propertyChangedAction == null) throw new ArgumentNullException("propertyChangedAction");
            if (errorsChangedAction == null) throw new ArgumentNullException("errorsChangedAction");

            ViewModel = viewModel;
            PropertyConfigurations = new Dictionary<string, IConfigurationItem>();
            ViewModel.PropertyChanged += Object_PropertyChanged;
            PropertyChangedAction = propertyChangedAction;
            ErrorsChangedAction = errorsChangedAction;
            propertyErrors = new Dictionary<string, List<string>>();
            PropertyErrors = new ReadOnlyDictionary<string, List<string>>(propertyErrors);
            Wrappers = new Dictionary<string, IWrapper>();
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

        public ConfigurationItem<TProperty> Property<TProperty>(Expression<Func<TProperty>> selectorExpression)
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            ConfigurationItem<TProperty> configItem;
            if (PropertyConfigurations.ContainsKey(propertyName))
                configItem = PropertyConfigurations[propertyName] as ConfigurationItem<TProperty>;
            else
            {
                configItem = new ConfigurationItem<TProperty>();
                PropertyConfigurations.Add(propertyName, configItem);
            }
            return configItem;
        }

        public void WrapObject<TModel, TViewModel>(Expression<Func<TModel>> selectorExpression)
            where TModel : class, new()
            where TViewModel : ViewModelProxy<TModel>, new()
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            Func<TModel> sourceGetterFunction = () =>
            {
                if (ViewModel.Model == null)
                    return null;
                else
                    return (TModel)Dynamitey.Dynamic.InvokeGet(ViewModel.Model, propertyName);
            };
            Wrappers.Add(propertyName, new ObjectWrapperByModel<TModel, TViewModel>(sourceGetterFunction));
        }

        public void WrapObject<TModel, TViewModel>(Expression<Func<TModel>> selectorExpression, Func<TViewModel> viewModelGetter)
            where TModel : class, new()
            where TViewModel : ViewModelProxy<TModel>
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            Wrappers.Add(propertyName, new ObjectWrapperByViewModel<TModel, TViewModel>(viewModelGetter));
        }

        public void WrapList<TModel, TViewModel>(Expression<Func<IList<TModel>>> selectorExpression)
            where TModel : class, new()
            where TViewModel : ViewModelProxy<TModel>, new()
        {
            string propertyName = PropertyEx.Name(selectorExpression);
            Func<IList<TModel>> sourceGetterFunction = () =>
            {
                if (ViewModel.Model == null)
                    return null;
                else
                    return (IList<TModel>)Dynamitey.Dynamic.InvokeGet(ViewModel.Model, propertyName);
            };
            Wrappers.Add(propertyName, new ListWrapper<TModel, TViewModel>(sourceGetterFunction));
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

            var value = Dynamitey.Dynamic.InvokeGet(ViewModel, property);
            var errs = PropertyConfigurations[property].Rules
                                                                     .Where(v => !v.Validate(value))
                                                                     .Select(r => r.Message);

            if (propertyErrors.ContainsKey(property)) propertyErrors.Remove(property);
            AddErrors(property, errs);
        }

        #endregion
    }
}
