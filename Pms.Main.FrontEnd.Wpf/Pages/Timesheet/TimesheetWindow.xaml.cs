using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Payroll.Timesheets.Domain.SupportTypes;
using System;
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
        //private readonly CutoffService CutoffService;

        public TimesheetWindow()
        {
            InitializeComponent();
            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));

            //CutoffService = new();
            //CutoffViewSource.Source = CutoffService.GetCutoffs();
        }

        public TimesheetWindow(Cutoff cutoff, IConfigurationRoot? conf)
        {
            InitializeComponent();
            Shared.Configuration = conf;
            CutoffViewSource = (CollectionViewSource)FindResource(nameof(CutoffViewSource));

            //CutoffService = new();
            //CutoffViewSource.Source = CutoffService.GetCutoffs();

            //Shared.DefaultCutoff = cutoff;
            cbPayrollDate.SelectedItem = cutoff;
            cbPayrollDate.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Shared.DefaultPayRegister = null;
            cbPayrollCode.SelectedItem = null;
            btnGenerateDBF.IsChecked = true;
        }

        private void cbPayrollCode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems is not null && e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
            {
                Shared.DefaultPayrollCode= (string)e.AddedItems[0];
                frmMain.Refresh();
            }
        }
        private void cbPayrollDate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is not null)
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
                timesheetPage = new(Shared.Configuration);
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
