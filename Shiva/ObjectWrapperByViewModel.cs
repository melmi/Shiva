using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    class ObjectWrapperByViewModel<TModel, TViewModel> : IWrapper
        where TModel : class, new()
        where TViewModel : ViewModelProxy<TModel>
    {
        public Func<TViewModel> SourceGetterFunction { get; private set; }
        public object Value { get; private set; }

        public ObjectWrapperByViewModel(Func<TViewModel> sourceGetterFunction)
        {
            if (sourceGetterFunction == null) throw new ArgumentNullException("sourceGetterFunction");
            SourceGetterFunction = sourceGetterFunction;
            Reset();
        }

        public void Reset()
        {
            Value = SourceGetterFunction();
        }
    }
}
