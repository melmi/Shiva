using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva
{
    public interface IOnNotifyPropertyChanged : INotifyPropertyChanged
    {
        void OnNotifyPropertyChanged(string property);
    }
}
