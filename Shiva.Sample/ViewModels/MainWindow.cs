using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.ViewModels
{
    class MainWindow
    {
        public IViewFactoryService ViewFactoryService { get; private set; }
        public Shiva.RelayCommand FirePersonDialog { get; private set; }
        public Shiva.RelayCommand FireEmployeeToCompany { get; private set; }
        public Shiva.RelayCommand FireCompanyToEmployee { get; private set; }
        public Shiva.RelayCommand FireCompany { get; private set; }

        public MainWindow(IViewFactoryService viewFactoryService)
        {
            if (viewFactoryService == null) throw new ArgumentNullException("viewFactoryService");
            ViewFactoryService = viewFactoryService;
            FirePersonDialog = new RelayCommand(x => runPersonDialogExample());
            FireEmployeeToCompany = new RelayCommand(x => ViewFactoryService.RunEmployeesToCompany(getEmployeesAndCompanies()));
            FireCompanyToEmployee = new RelayCommand(x => ViewFactoryService.RunCompanyToEmployees(getEmployeesAndCompanies()));
            FireCompany = new RelayCommand(x => ViewFactoryService.RunCompany(getEmployeesAndCompanies().Companies[0]));
        }

        void runPersonDialogExample()
        {
            var m = new Models.Person();
            m.FirstName = "John";
            m.LastName = "Smith";
            m.Age = 30;

            var vm = new ViewModels.PersonDialog();
            vm.Model = m;
            vm.BeginEdit();

            ViewFactoryService.RunPersonDialog(vm);

            ViewFactoryService.ShowMessage(string.Format("{0} {1} {2}", m.FirstName, m.LastName, m.Age));
        }

        EmployeesAndCompanies getEmployeesAndCompanies()
        {
            var p1 = new Models.Person { FirstName = "P1 F", LastName = "P1 L", Age = 1 };
            var p2 = new Models.Person { FirstName = "P2 F", LastName = "P2 L", Age = 2 };
            var p3 = new Models.Person { FirstName = "P3 F", LastName = "P3 L", Age = 3 };
            var p4 = new Models.Person { FirstName = "P4 F", LastName = "P4 L", Age = 4 };
            var p5 = new Models.Person { FirstName = "P5 F", LastName = "P5 L", Age = 5 };
            var employees = new Models.Person[] { p1, p2, p3, p4, p5 };

            var c1 = new Models.Company { Name = "C1", Address = "C1 Address", Phone = "111" };
            var c2 = new Models.Company { Name = "C2", Address = "C2 Address", Phone = "222" };
            var companies = new Models.Company[] { c1, c2 };

            p1.Company = p2.Company = p3.Company = c1;
            p4.Company = p5.Company = c2;

            c1.Employees.AddRange(new Models.Person[] { p1, p2, p3 });
            c2.Employees.AddRange(new Models.Person[] { p4, p5 });

            var result = new EmployeesAndCompanies();
            result.Employees = new ObservableCollection<Person>(employees.Select(p => new Person() { Model = p }));
            result.Companies = new ObservableCollection<Company>(companies.Select(p => new Company() { Model = p }));
            return result;
        }
    }
}
