using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public static class IOnNotifyPropertyChangedEx
    {
        static public bool SetFieldAndNotify<T1, T2>(
            this IOnNotifyPropertyChanged @object,
            ref T1 field,
            T1 value,
            Expression<Func<T2>> selectorExpression) where T1 : T2
        {
            if (EqualityComparer<T1>.Default.Equals(field, value)) return false;
            field = value;
            @object.OnNotifyPropertyChanged(PropertyEx.Name(selectorExpression));
            return true;
        }
    }
}
