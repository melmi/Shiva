using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shiva.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var m = new Models.Person();
            m.FirstName = "John";
            m.LastName = "Smith";
            m.Age = 30;

            var vm = new ViewModels.PersonDialogViewModel();
            vm.Model = m;

            var v = new Views.PersonDialog();
            v.DataContext = vm;
            v.ShowDialog();

            MessageBox.Show(string.Format("{0} {1} {2}", m.FirstName, m.LastName, m.Age));
        }
    }
}
