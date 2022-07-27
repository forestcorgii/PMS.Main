using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Main.FrontEnd.Wpf.Controller;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Pms.Main.FrontEnd.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TimesheetWindow : Window
    {

        private readonly CollectionViewSource CutoffViewSource;
        private readonly CollectionViewSource PayrollCodeViewSource;

        private TimesheetController TimesheetController;
        private EmployeeController EmployeeController;


        public TimesheetWindow()
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

            List<string> payrollCodes = EmployeeController.ListPayrollCodes();
            PayrollCodeViewSource.Source = payrollCodes;

            // cbPayrollCode.SelectedItem = Shared.DefaultPayrollCode;
            btnGenerateDBF.IsChecked = true;

            cbPayrollCode.SelectedItem = Shared.DefaultPayrollCode;
            cbCutoffDate.SelectedItem = Shared.DefaultCutoff;
        }

        private void cbPayrollCode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems is not null && e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
            {
                Shared.DefaultPayrollCode = (string)e.AddedItems[0];
                frmMain.Refresh();
            }
        }

        private void cbPayrollDate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems is not null && e.AddedItems[0] is not null)
            {
                Shared.DefaultCutoff = new Cutoff((string)e.AddedItems[0]);
                frmMain.Refresh();
            }
        }


        private TimeDownloaderPage? timesheetDownloaderPage;
        private void btnTimesheetDownloader_Checked(object sender, RoutedEventArgs e)
        {
            if (timesheetDownloaderPage is null)
                timesheetDownloaderPage = new();

            frmMain.Navigate(timesheetDownloaderPage);
        }

        private TimesheetPage timesheetPage;
        private void btnTimesheetPage_Checked(object sender, RoutedEventArgs e)
        {
            if (timesheetPage is null)
                timesheetPage = new();
            frmMain.Navigate(timesheetPage);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

            //CutoffViewSource.Source = CutoffService.GetCutoffs();
        }


        //private void btnCompare_Checked(object sender, RoutedEventArgs e)
        //{
        //    frmTimesheet.Navigate(new PayrollTimeComparer());
        //}

        //private void btnDownload_Checked(object sender, RoutedEventArgs e)
        //{
        //    frmTimesheet.Navigate(TimeDownloaderPage);
        //}

        //private void btnGenerateDBF_Checked(object sender, RoutedEventArgs e)
        //{
        //    frmTimesheet.Navigate(new GenerateDBFPage());
        //}
    }
}
