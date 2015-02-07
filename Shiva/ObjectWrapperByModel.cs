using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    class ObjectWrapperByModel<TModel, TViewModel> : IWrapper
        where TModel : class, new()
        where TViewModel : ViewModelProxy<TModel>, new()
    {
        Lazy<TViewModel> vm;
        public Func<TModel> SourceGetterFunction { get; private set; }
        public object Value { get { return vm.Value; } }

        public ObjectWrapperByModel(Func<TModel> sourceGetterFunction)
        {
            if (sourceGetterFunction == null) throw new ArgumentNullException("sourceGetterFunction");
            SourceGetterFunction = sourceGetterFunction;
            Reset();
        }

        public void Reset()
        {
            vm = new Lazy<TViewModel>(() =>
            {
                var model = SourceGetterFunction();
                if (model == null) return null;
                return new TViewModel { Model = model };
            });
        }
    }
}
