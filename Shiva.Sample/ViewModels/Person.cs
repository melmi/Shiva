using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.ViewModels
{
    class Person : ViewModelProxy<Models.Person>
    {
        public string FullName { get { return Model.FirstName + " " + Model.LastName; } }

        public Person()
        {
            Configuration.Property(() => Model.Age)
                         .Enforce(x => x > 0, "Age should be greater than zero");

            Configuration.Property(() => FullName)
                         .DependsOn(() => Model.FirstName)
                         .DependsOn(() => Model.LastName);

            Configuration.WrapObject<Models.Company, Company>(() => Model.Company);
        }
    }
}
