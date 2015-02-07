using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shiva.Sample.ViewModels
{
    class PersonDialog : Shiva.ViewModelProxy<Models.Person>
    {
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string FullName { get { return Model.FirstName + " " + Model.LastName; } }

        public PersonDialog()
        {
            Configuration.Property(() => Model.Age)
                         .Enforce(x => x > 0, "Age should be greater than zero");

            Configuration.Property(() => FullName)
                         .DependsOn(() => Model.FirstName)
                         .DependsOn(() => Model.LastName);

            OkCommand = new RelayCommand(x => EndEdit());
            CancelCommand = new RelayCommand(x => CancelEdit());
        }
    }
}
