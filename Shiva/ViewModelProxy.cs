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
        public Configuration<ViewModelProxy<T>> Configuration { get; private set; }
        static PropertyInfo[] objectProperties;

        T originalModel, dirtyModel;
        public T Model
        {
            get { if (editing)return dirtyModel; else return originalModel; }
            set
            {
                originalModel = value;
                editing = false;
                dirtyModel = null;
            }
        }

        public ViewModelProxy()
        {
            Configuration = new Configuration<ViewModelProxy<T>>(this, OnPropertyChanged, OnErrorsChanged);
        }

        #region static

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

        static ViewModelProxy()
        {
            objectProperties =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p.CanRead && p.CanWrite)
                         .Where(p => isSupportedType(p.PropertyType))
                         .ToArray();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
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

                var oldVal = Dynamitey.Dynamic.InvokeGet(this, pi.Name);
                if (!convertErr &&
                    Dynamitey.Dynamic.InvokeBinaryOperator(val, ExpressionType.Equal, oldVal))
                    return false;

                Configuration.Validator.ClearErrors(pi.Name);

                if (convertErr)
                    Configuration.Validator.AddError(pi.Name, "Value format error.");
                else
                {
                    pi.SetValue(Model, val, null);
                    OnPropertyChanged(binder.Name);
                }

                return !convertErr;
            }
            else
                return base.TrySetMember(binder, value);
        }

        #endregion

        #region IEditableObject

        bool editing = false;

        public virtual void BeginEdit()
        {
            if (editing) return;
            editing = true;
            dirtyModel = new T();
            copy(originalModel, dirtyModel, objectProperties);
        }

        public virtual void CancelEdit()
        {
            editing = false;
            dirtyModel = null;
        }

        public virtual void EndEdit()
        {
            if (!editing) return;
            editing = false;
            copy(dirtyModel, originalModel, objectProperties);
            dirtyModel = null;
        }

        #endregion

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(string property)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(property));
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (!Configuration.Validator.PropertyErrors.ContainsKey(propertyName)) return null;
            return Configuration.Validator.PropertyErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return Configuration.Validator.PropertyErrors.Count > 0; }
        }

        #endregion

        #region Get/Set

        public TValue GetMember<TValue>(Expression<Func<TValue>> selectorExpression)
        {
            return Dynamitey.Dynamic.InvokeGet(this, PropertyEx.Name(selectorExpression));
        }

        public void SetMember<TValue>(Expression<Func<TValue>> selectorExpression, TValue value)
        {
            Dynamitey.Dynamic.InvokeSet(this, PropertyEx.Name(selectorExpression), value);
        }

        #endregion
    }
}
