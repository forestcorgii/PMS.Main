using Pms.Main.FrontEnd.Wpf.ViewModel;
using Pms.Timesheets.Domain.SupportTypes;
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

        private readonly FilterTimesheetsViewModel TimesheetController;
        private readonly FilterEmployeesViewModel EmployeeController;


        public MainWindow()
        {
            InitializeComponent();

            TimesheetController = new(Shared.DefaultCutoff, Shared.DefaultPayrollCode);
            EmployeeController = new(Shared.DefaultPayrollCode);

            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));
            PayrollCodeViewSource = (CollectionViewSource)FindResource(nameof(PayrollCodeViewSource));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> cutoffs = TimesheetController.GetCutoffs();
            CutoffViewSource.Source = cutoffs;

            List<string> payrollCodes = EmployeeController.ListPayrollCodes();
            PayrollCodeViewSource.Source = payrollCodes;
        }

        private void CbPayrollDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Shared.DefaultCutoff = new Cutoff(cbPayrollDate.Text);
                frmMain.Refresh();
            }
        }
        private void CbPayrollDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object? currentValue = e.AddedItems[0];
            if (currentValue is not null)
            {
                Shared.DefaultCutoff = new Cutoff((string)currentValue);
                frmMain.Refresh();
            }
        }


        private void cbPayrollCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Shared.DefaultPayrollCode = cbPayrollCode.Text;
                frmMain.Refresh();
            }
        }
        private void cbPayrollCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object? currentValue = e.AddedItems[0];
            if (currentValue is not null)
            {
                Shared.DefaultPayrollCode = (string)currentValue;
                frmMain.Refresh();
            }
        }


        private TimesheetPage? TimesheetPage;
        private void BtnTimesheet_Checked(object sender, RoutedEventArgs e)
        {
            if (TimesheetPage is null)
                TimesheetPage = new();
            frmMain.Navigate(TimesheetPage);
        }

        private void BtnEmployee_Checked(object sender, RoutedEventArgs e)
        {
            if (Shared.DefaultCutoff is not null)
            {
                EmployeeUi employeeUI = new();
                frmMain.Navigate(employeeUI);
            }
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

    }
}
