using Pms.Main.FrontEnd.Wpf.Controller;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MainWindow : Window
    {
        private readonly CollectionViewSource CutoffViewSource;
        private readonly CollectionViewSource PayrollCodeViewSource;

        private TimesheetController TimesheetController;
        private EmployeeController EmployeeController;


        public MainWindow()
        {
            InitializeComponent();

            TimesheetController = new();
            EmployeeController = new();

            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));
            PayrollCodeViewSource = (CollectionViewSource)FindResource(nameof(PayrollCodeViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> cutoffs = TimesheetController.GetCutoffs();
            CutoffViewSource.Source = cutoffs;

            List<string> payrollCodes= EmployeeController.ListPayrollCodes();
            PayrollCodeViewSource.Source = payrollCodes;
        }

        private void CbPayrollDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
            {
                //Shared.DefaultCutoff = (Cutoff)e.AddedItems[0];
                frmMain.Refresh();
            }
        }

        private void BtnTimesheet_Checked(object sender, RoutedEventArgs e)
        {
                TimesheetWindow timeheetUI = new();
                _ = timeheetUI.ShowDialog();
        }

        private void BtnEmployee_Checked(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultCutoff is not null)
            //{
            //    EmployeeWindow employeeUI = new(Shared.DefaultCutoff, Shared.Configuration);
            //    _ = employeeUI.ShowDialog();
            //}

        }

        private void BtnAdjustment_Click(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultCutoff is not null)
            //{
            //    AdjustmentWindow adjustUI = new(Shared.DefaultCutoff, Shared.Configuration);
            //    _ = adjustUI.ShowDialog();
            //}
        }

        private void BtnPayroll_Click(object sender, RoutedEventArgs e)
        {
            //if (Shared.DefaultCutoff is not null)
            //{
            //    PayrollWindow payrollUI = new(Shared.DefaultCutoff, Shared.Configuration);
            //    _ = payrollUI.ShowDialog();
            //}
        }

        private void cbPayrollCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
