﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public static class INotifyPropertyChangedEx
    {
        static public bool SetFieldAndNotify<T1, T2>(
            this INotifyPropertyChanged obj,
            ref T1 field,
            T1 value,
            Expression<Func<T2>> selectorExpression,
            Action<string> propertyChangedEventHandlerCaller) where T1 : T2
        {
            if (EqualityComparer<T1>.Default.Equals(field, value)) return false;
            field = value;
            if (propertyChangedEventHandlerCaller != null)
                propertyChangedEventHandlerCaller(PropertyEx.Name(selectorExpression));
            return true;
        }
    }
}
