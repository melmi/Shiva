using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.ViewModels
{
    class EmployeesAndCompanies
    {
        public ObservableCollection<Person> Employees { get; set; }
        public ObservableCollection<Company> Companies { get; set; }
        public Company CurrentCompany { get; set; }
    }
}
