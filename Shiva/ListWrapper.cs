using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    class ListWrapper<TModel, TViewModel> : IWrapper
        where TModel : class, new()
        where TViewModel : ViewModelProxy<TModel>, new()
    {
        IList<TModel> source;
        ObservableCollection<TViewModel> vm;

        public Func<IList<TModel>> SourceGetterFunction { get; private set; }
        public object Value
        {
            get { return vm; }
        }

        public ListWrapper(Func<IList<TModel>> sourceGetterFunction)
        {
            if (sourceGetterFunction == null) throw new ArgumentNullException("sourceGetterFunction");
            SourceGetterFunction = sourceGetterFunction;
            Reset();
        }

        public void Reset()
        {
            source = SourceGetterFunction();
            vm.CollectionChanged -= vm_CollectionChanged;
            vm = new ObservableCollection<TViewModel>(source.Select(s => new TViewModel { Model = s }));
            vm.CollectionChanged += vm_CollectionChanged;
        }

        void vm_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // based on: http://stackoverflow.com/questions/1256793/mvvm-sync-collections
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        source.Insert(e.NewStartingIndex + i, ((TViewModel)e.NewItems[i]).Model);
                    break;

                case NotifyCollectionChangedAction.Move:
                    var items = source.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                    for (int i = 0; i < e.OldItems.Count; i++)
                        source.RemoveAt(e.OldStartingIndex);

                    for (int i = 0; i < items.Count; i++)
                        source.Insert(e.NewStartingIndex + i, items[i]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                        source.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // remove
                    for (int i = 0; i < e.OldItems.Count; i++)
                        source.RemoveAt(e.OldStartingIndex);

                    // add
                    goto case NotifyCollectionChangedAction.Add;

                case NotifyCollectionChangedAction.Reset:
                    source.Clear();
                    for (int i = 0; i < e.NewItems.Count; i++)
                        source.Add(((TViewModel)e.NewItems[i]).Model);
                    break;

                default:
                    break;
            }
        }
    }
}
