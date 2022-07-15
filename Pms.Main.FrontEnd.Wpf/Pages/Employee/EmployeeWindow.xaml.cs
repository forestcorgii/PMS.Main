using Microsoft.Extensions.Configuration;
using Payroll.Timesheets.Domain.SupportTypes;
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

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {

        private readonly CollectionViewSource CutoffViewSource;
        
        public EmployeeWindow()
        {
            InitializeComponent();
            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));
         }

        public EmployeeWindow(Cutoff cutoff, IConfigurationRoot? conf)
        {
            InitializeComponent();
            Shared.Configuration = conf;
            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));

            cbPayrollDate.SelectedItem = cutoff;
            cbPayrollDate.IsEnabled = false;
             
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnEmployee.IsChecked = true;
        }

        private void BtnEmployee_Checked(object sender, RoutedEventArgs e)
        {
            frmMain.Navigate(new EmployeeUi());
        }

        private void cbPayrollCode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems is not null && e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
            {
                //Shared.DefaultPayRegister = (PayRegister)e.AddedItems[0];
                frmMain.Refresh();
            }
        }
        private void cbPayrollDate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
            {
                //Shared.DefaultCutoff = (Cutoff)e.AddedItems[0];
                frmMain.Refresh();
            }
        }
    }
}
