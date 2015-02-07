using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Shiva.Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var vm = new ViewModels.MainWindow(Views.ViewFactoryService.Instance);
            var m = new Views.MainWindow();
            m.DataContext = vm;
            m.Show();
        }
    }
}
