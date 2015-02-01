using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    class ObjectWrapper<TModel, TViewModel> : IWrapper
        where TModel : class, new()
        where TViewModel : ViewModelProxy<TModel>, new()
    {
        public Func<TModel> SourceGetterFunction { get; private set; }
        public object Value { get; private set; }

        public ObjectWrapper(Func<TModel> sourceGetterFunction)
        {
            if (sourceGetterFunction == null) throw new ArgumentNullException("sourceGetterFunction");
            SourceGetterFunction = sourceGetterFunction;
            Reset();
        }

        public void Reset()
        {
            Value = new TViewModel { Model = SourceGetterFunction() };
        }
    }
}
