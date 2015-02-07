using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shiva.Sample.Views
{
    class ViewFactoryService : ViewModels.IViewFactoryService
    {
        public void RunPersonDialog(object viewModel)
        {
            var v = new PersonDialog { DataContext = viewModel };
            v.ShowDialog();
        }

        public void RunEmployeesToCompany(object viewModel)
        {
            var v = new EmployeeToCompany { DataContext = viewModel };
            v.ShowDialog();
        }

        public void RunCompanyToEmployees(object viewModel)
        {
            var v = new CompanyToEmployees { DataContext = viewModel };
            v.ShowDialog();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        ViewFactoryService() { }
        static Lazy<ViewFactoryService> instance = new Lazy<ViewFactoryService>(() => new ViewFactoryService());
        static public ViewFactoryService Instance { get { return instance.Value; } }
    }
}
