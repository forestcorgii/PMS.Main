using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf
{
    public partial class TimeDownloaderPage
    {
        //private readonly TimesheetDownloaderService TimesheetDownloaderService;
        //private readonly EmployeeService EmployeeService;
        //DownloadProgress? progress;

        private BackgroundWorker bgProcessor;


        public TimeDownloaderPage()
        {
            bgProcessor = new BackgroundWorker();
            bgProcessor.DoWork += BgProcessor_DoWork;
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.

            //TimesheetDownloaderService = new TimesheetDownloaderService(Shared.Configuration);
            //EmployeeService = new EmployeeService(Shared.Configuration);
        }

        private void TimeDownloader_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSummary();
        }

        private async void BgProcessor_DoWork(object? sender, DoWorkEventArgs e)
        {
            // GET CUT OFF RANGE
            if (e.Argument is null) return;
            //DownloadProgress progress = default;

            string payrollCode, payRegisterId;
            int totalPage = 0;
            List<int> pages;
            DateTime payrollDate;


            string message = "";

            try
            {
                //progress = (DownloadProgress)e.Argument;
                //if (progress is not null && progress.Pages is not null)
                //{
                //    payrollDate = progress.PayrollDate;
                //    payrollCode = progress.PayrollCode;
                //    payRegisterId = progress.PayRegisterId;
                //    pages = progress.Pages;
                //    totalPage = progress.TotalPage;
                //}
                //else return;

                //if (progress is not null && progress.Pages is not null)
                //{
                //    foreach (int page in progress.Pages)
                //    {
                //        await TimesheetDownloaderService.ProcessDownloadPageContent(
                //            payrollCode,
                //            payRegisterId,
                //            payrollDate,
                //            page
                //        );

                //        {
                //            Dispatcher.Invoke(() => { pb.Value = page + 1; lbPage.Text = $"{page}/{progress.Pages.Last()}"; });
                //        }
                //    }
                //    message = "Download Finished!";
                //}
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            {
                Dispatcher.Invoke(() =>
                {
                    pb.Value = totalPage;
                    lbStatus.Text = message;

                    btnDownload.IsEnabled = true;
                    cbDownloadType.IsEnabled = true;
                    btnRefresh.IsEnabled = true;
                });
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            //string payRegisterId;
            //int totalPage, page;
            //if (progress is not null)
            //{
            //    payRegisterId = progress.PayRegisterId;
            //    page = progress.Page;
            //    totalPage = progress.TotalPage;
            //}
            //else return;

            //if (!bgProcessor.IsBusy && progress is not null)
            //{
            //    switch (cbDownloadType.SelectedIndex)
            //    {
            //        case 0://RESUME DOWNLOAD
            //            if (totalPage == page)
            //                progress.Pages = Enumerable.Range(0, totalPage + 1).ToList();
            //            else
            //                progress.Pages = Enumerable.Range(page, (totalPage + 1) - page).ToList();
            //            break;
            //        case 1://DOWNLOAD UNCONFIRMED
            //            progress.Pages = TimesheetDownloaderService.GetPageWithUnconfirmedTS(payRegisterId);
            //            break;
            //        case 2://RE DOWNLOAD
            //            progress.Page = 0;
            //            progress.Pages = Enumerable.Range(0, totalPage + 1).ToList();
            //            break;
            //    }

            //    if (progress.Pages is not null && progress.Pages.Count > 0)
            //    {
            //        lbStatus.Text = "Downloading...";
            //        pb.Value = progress.Pages[0];
            //        lbPage.Text = $"{progress.Pages[0] + 1}/{progress.Pages.Last()}";

            //        btnDownload.IsEnabled = false;
            //        cbDownloadType.IsEnabled = false;
            //        btnRefresh.IsEnabled = false;
            //        bgProcessor.RunWorkerAsync(progress);
            //    }
            //    else
            //    {
            //        lbStatus.Text = "No listed for Download.";
            //        pb.Value = 0;
            //        lbPage.Text = "0/0";
            //    }
            //}
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ClearSummaryDisplay();
            LoadSummary();
        }

        private async void LoadSummary()
        {
            //if (Shared.DefaultCutoff is not null && Shared.DefaultPayRegister is not null && TimesheetDownloaderService is not null)
            //{
            //    //TODO: GET A DOWNLOAD SUMMARY FROM THE TIME SYSTEM
            //    DateTime[] cutoffRange = CutoffService.GetCutoffRange(Shared.DefaultCutoff.PayrollDate);

            //    // ctrlLoader.Visibility = Visibility.Visible;
            //    DownloadSummary<Timesheet>? summary = await TimesheetDownloaderService.GetTimesheetSummary(cutoffRange, Shared.DefaultPayRegister.PayrollCode);
            //    // ctrlLoader.Visibility = Visibility.Collapsed;

            //    if (summary is not null && summary.UnconfirmedTimesheet is not null)
            //    {
            //        // CREATE A DOWNLOAD PROGRESS INSTANCE BASED ON THE DOWNLOAD SUMMARY
            //        progress = new(
            //            Shared.DefaultPayRegister,
            //            Int32.Parse(summary.TotalPage),
            //            TimesheetDownloaderService.GetLastPage(Shared.DefaultPayRegister.PayRegisterId)
            //        );


            //        List<Employee> employees = await EmployeeService.CompleteEmployeeDetails(summary.UnconfirmedTimesheet);
            //        lstUnconfirmedEmployees.ItemsSource = employees;

            //        FillSummaryDisplay(summary, progress);
            //    }
            //}
        }

        private void ClearSummaryDisplay()
        {
            lbEmployeeCount.Text = "Employee Count: ???";
            lbPageCount.Text = "Page Count: ???";
            lbUnconfirmedEmployeeCount.Text = "Unconfirmed: ???";
            lbConfirmedEmployeeCount.Text = "Confirmed: ???";
            lbPage.Text = "0/0";
            pb.Maximum = 0;
            pb.Value = 0;
        }

        //private void FillSummaryDisplay(DownloadSummary<Timesheet> summary, DownloadProgress _progress)
        //{
        //    lbEmployeeCount.Text = $"Employee Count: {summary.TotalCount}";
        //    lbPageCount.Text = $"Page Count: {summary.TotalPage}";
        //    lbUnconfirmedEmployeeCount.Text = $"Unconfirmed: {summary.UnconfirmedTimesheet.Length}";
        //    lbConfirmedEmployeeCount.Text = $"Confirmed: {summary.TotalConfirmed}";
        //    lbPage.Text = $"{_progress.Page}/{_progress.TotalPage}";
        //    pb.Maximum = _progress.TotalPage + 1;
        //    pb.Value = _progress.Page;
        //}

    }
}