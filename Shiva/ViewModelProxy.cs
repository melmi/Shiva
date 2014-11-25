using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public abstract class ViewModelProxy<T> : DynamicObject, INotifyPropertyChanged, IEditableObject, INotifyDataErrorInfo
        where T : class, new()
    {
        PropertyInfo[] objectProperties;

        T originalModel, dirtyModel;
        protected T Model
        {
            get { if (editing)return dirtyModel; else return originalModel; }
            set
            {
                originalModel = value;
                objectProperties =
                    typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanRead && p.CanWrite)
                             .Where(p => isSupportedType(p.PropertyType))
                             .ToArray();
                editing = false;
                dirtyModel = null;
            }
        }

        public ViewModelProxy()
        {
            Configuration = new Configuration();
        }

        #region static methods

        static void copy(T src, T dst, IEnumerable<PropertyInfo> infos)
        {
            foreach (var info in infos)
                info.SetValue(dst, info.GetValue(src));
        }

        static bool isSupportedType(Type t)
        {
            return t.IsEnum || 
                   t.IsPrimitive || 
                   t == typeof(string) || 
                   t == typeof(DateTime) || 
                   t == typeof(TimeSpan) || 
                   t == typeof(DateTimeOffset);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));

            var dependingProps = Configuration.PropertyConfigurations
                .Where(pc => pc.Value.Dependencies.Contains(property))
                .Select(pc => pc.Key)
                .ToList();
            foreach (var p in dependingProps) OnPropertyChanged(p);
        }

        #endregion

        #region DynamicObject

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var pi = objectProperties.FirstOrDefault((p) => p.Name == binder.Name);

            if (pi != null)
            {
                result = Model != null ? pi.GetValue(Model, null) : null;
                return true;
            }
            else
                return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var pi = objectProperties.FirstOrDefault((p) => p.Name == binder.Name);

            if (pi != null && Model != null)
            {
                object val = null;
                bool convertErr = false;
                try { val = Convert.ChangeType(value, pi.PropertyType); }
                catch { convertErr = true; }

                if (!convertErr && val.Equals(getMember(pi.Name))) return false;

                bool errsChanged = false;
                if (propertyErrors.ContainsKey(pi.Name))
                {
                    propertyErrors.Remove(pi.Name);
                    errsChanged = true;
                }

                if (convertErr)
                {
                    errsChanged = true;
                    propertyErrors.Add(pi.Name, new List<string>() { "Value format error." });
                }
                else
                {
                    pi.SetValue(Model, val, null);


                    var errs = Configuration.Validate(pi.Name, val);
                    if (errs != null && errs.Count > 0)
                    {
                        propertyErrors.Add(pi.Name, errs);
                        errsChanged = true;
                    } OnPropertyChanged(binder.Name);
                }

                if (errsChanged) OnErrorsChanged(pi.Name);

                return !convertErr;
            }
            else
                return base.TrySetMember(binder, value);
        }

        #endregion

        #region IEditableObject

        bool editing = false;

        public void BeginEdit()
        {
            if (editing) return;
            editing = true;
            dirtyModel = new T();
            copy(originalModel, dirtyModel, objectProperties);
        }

        public void CancelEdit()
        {
            editing = false;
            dirtyModel = null;
        }

        public void EndEdit()
        {
            if (!editing) return;
            editing = false;
            copy(dirtyModel, originalModel, objectProperties);
            dirtyModel = null;
        }

        #endregion

        #region INotifyDataErrorInfo

        public Configuration Configuration { get; private set; }

        Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (!propertyErrors.ContainsKey(propertyName)) return null;
            return propertyErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return propertyErrors.Count > 0; }
        }

        #endregion

        #region Get/Set

        object getMember(string memberName)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
                CSharpBinderFlags.None,
                memberName,
                this.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, this);
        }

        void setMember(string memberName, object value)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.SetMember(
                CSharpBinderFlags.None,
                memberName,
                this.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            callsite.Target(callsite, this, value);
        }

        public TValue GetMember<TValue>(Expression<Func<TValue>> selectorExpression)
        {
            return (TValue)getMember(PropertyEx.Name(selectorExpression));
        }

        public void SetMember<TValue>(Expression<Func<TValue>> selectorExpression, TValue value)
        {
            setMember(PropertyEx.Name(selectorExpression), value);
        }

        #endregion
    }
}
