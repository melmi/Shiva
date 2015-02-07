using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.ViewModels
{
    interface IViewFactoryService
    {
        void RunPersonDialog(object viewModel);
        void RunEmployeesToCompany(object viewModel);
        void RunCompanyToEmployees(object viewModel);
        void ShowMessage(string message);
    }
}
