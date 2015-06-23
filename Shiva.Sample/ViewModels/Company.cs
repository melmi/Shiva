using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.ViewModels
{
    class Company : ViewModelProxy<Models.Company>
    {
        public Company()
        {
            Configuration.WrapList<Models.Person, Person>(() => Model.Employees);
        }
    }
}
