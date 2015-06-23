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
    public abstract class ViewModelProxy<T> :
        DynamicObject, INotifyPropertyChanged, IEditableObject, INotifyDataErrorInfo
        where T : class, new()
    {
        public Configuration<T> Configuration { get; private set; }
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

                foreach (var i in objectProperties) OnPropertyChanged(i.Name);
                foreach (var i in Configuration.Wrappers)
                {
                    if (i.Value != null) i.Value.Reset();
                    OnPropertyChanged(i.Key);
                }
            }
        }

        public ViewModelProxy()
        {
            Configuration = new Configuration<T>(this, OnPropertyChanged, OnErrorsChanged);
        }

        #region static

        // http://stackoverflow.com/questions/2023210/
        static T clone(T obj)
        {
            if (obj == null) return null;
            if (obj is ICloneable) return (T)((ICloneable)obj).Clone();

            System.Reflection.MethodInfo inst = obj.GetType().GetMethod("MemberwiseClone",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (inst != null)
                return (T)inst.Invoke(obj, null);
            else
                return null;
        }

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

            if (Configuration.Wrappers.ContainsKey(binder.Name))
            {
                result = Configuration.Wrappers[binder.Name].Value;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var pi = objectProperties.FirstOrDefault((p) => p.Name == binder.Name);
            if (pi != null && Model != null)
            {
                object val = null;
                bool convertErr = false;

                if (value != null && pi.PropertyType.Equals(value.GetType()))
                    val = value;
                else
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(pi.PropertyType);
                        val = converter.ConvertFrom(value);
                    }
                    catch { convertErr = true; }

                var oldVal = Dynamitey.Dynamic.InvokeGet(this, pi.Name);
                if (!convertErr &&
                    Dynamitey.Dynamic.InvokeBinaryOperator(val, ExpressionType.Equal, oldVal))
                    return false;

                Configuration.ClearErrors(pi.Name);

                if (convertErr)
                    Configuration.AddError(pi.Name, "Value format error.");
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
            dirtyModel = clone(originalModel);
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
            if (!Configuration.PropertyErrors.ContainsKey(propertyName)) return null;
            return Configuration.PropertyErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return Configuration.PropertyErrors.Count > 0; }
        }

        #endregion

        #region Get/Set

        public TValue GetMember<TValue>(Expression<Func<TValue>> selectorExpression)
        {
            return Dynamitey.Dynamic.InvokeGet(this, PropertyEx.Name(selectorExpression));
        }

        public object GetWrappedMember<T>(Expression<Func<T>> selectorExpression)
        {
            var name = PropertyEx.Name(selectorExpression);
            return Configuration.Wrappers.ContainsKey(name) ? Configuration.Wrappers[name].Value : null;
        }

        public void SetMember<TValue>(Expression<Func<TValue>> selectorExpression, TValue value)
        {
            Dynamitey.Dynamic.InvokeSet(this, PropertyEx.Name(selectorExpression), value);
        }

        #endregion
    }
}
