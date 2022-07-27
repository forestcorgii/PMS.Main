using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;
using Pms.Main.FrontEnd.Wpf.Controller;
using Pms.Timesheets.Domain;

namespace Pms.Main.FrontEnd.Wpf
{

    public partial class TimesheetPage
    {
        private readonly CollectionViewSource TimesheetViewSource;

        private readonly TimesheetController TimesheetController;
        private readonly TimesheetOutputController TimesheetOutputController;
        private readonly TimesheetDownloadController TSDownloadController;
        private readonly EmployeeController EmployeeController;
        private readonly EmployeeDownloadController EEDownloadController;



        public TimesheetPage()
        {
            InitializeComponent();

            TimesheetViewSource = (CollectionViewSource)FindResource(nameof(TimesheetViewSource));

            EmployeeController = new();
            EEDownloadController = new();

            TimesheetController = new();
            TSDownloadController = new();
            TimesheetOutputController = new();

            InitializeEvents();
        }


        private void GenerateDBFPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadList();
        }

        private void ReloadList()
        {
            if (Shared.DefaultCutoff is not null && Shared.DefaultPayrollCode is not null)
            {
                var payrollCode = Shared.DefaultPayrollCode;
                var cutoff = Shared.DefaultCutoff;

                IEnumerable<Timesheet> timesheets = TimesheetController.GetTimesheets(cutoff.CutoffId, payrollCode);
                TimesheetViewSource.Source = timesheets;

                LbTotalEE.Text = $"Total:   {timesheets.Count()}";
                LbUnconfirmedEE.Text = $"Unconfirmed:   {timesheets.Count(ts => !ts.IsConfirmed)}";
                LbUnconfirmedEEWithAttendance.Text = $"Unconfirmed With Attendance:   {timesheets.Count(ts => !ts.IsConfirmed && ts.TotalHours > 0)}";
            }
        }


        private void InitializeEvents()
        {
            TSDownloadController.DownloadStarted += DownloadController_DownloadStarted;
            TSDownloadController.DownloadCancelled += DownloadController_DownloadEnded;
            TSDownloadController.DownloadEnded += DownloadController_DownloadEnded;

            TSDownloadController.PageDownloadSucceeded += DownloadController_PageDownloadSucceeded;
            TSDownloadController.PageDownloadFailed += DownloadController_PageDownloadFailed;

            TSDownloadController.EvaluationStarted += DownloadController_EvaluationStarted;
            TSDownloadController.EvaluationSucceeded += DownloadController_EvaluationSucceeded;
            TSDownloadController.EvaluationFailed += DownloadController_EvaluationFailed;


            EEDownloadController.EmployeeDownloadStarted += EEDownloadController_EmployeeDownloadStarted;
            EEDownloadController.EmployeeDownloadSucceed += EEDownloadController_EmployeeDownloadSucceed;
            EEDownloadController.EmployeeDownloadError += EEDownloadController_EmployeeDownloadError;

            TimesheetOutputController.ExportStarted += TimesheetOutputController_ExportStarted;
            TimesheetOutputController.ExportFailed += TimesheetOutputController_ExportFailed;
            TimesheetOutputController.ExportEnded += TimesheetOutputController_ExportEnded;

            TimesheetController.TimesheetFillStarted += TimesheetController_TimesheetFillStarted;
            TimesheetController.TimesheetFilled += TimesheetController_TimesheetFilled;
            TimesheetController.TimesheetFillFailed += TimesheetController_TimesheetFillFailed;
        }


























        #region TIMESHEET DOWNLOAD
        private void btnDownloadTS_Click(object sender, RoutedEventArgs e)
        {
            _ = TSDownloadController.StartDownload(Shared.DefaultCutoff, Shared.DefaultPayrollCode);
        }
        private void DownloadController_DownloadStarted(object sender, int TotalPages)
        {
            PbDownloadProgress.Maximum = TotalPages + 1;
            PbDownloadProgress.Value = 0;

            LbStatusMessage.Text = "Downloading Timesheets";
            LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
            LbPbValue.Text = "0";
        }
        private void DownloadController_DownloadEnded(object? sender, EventArgs e)
        {
            PbDownloadProgress.Maximum = 0;
            PbDownloadProgress.Value = 0;

            LbPbMaximum.Text = "0";
            LbPbValue.Text = "0";
        }

        private void DownloadController_PageDownloadSucceeded(object sender, int Page)
        {
            PbDownloadProgress.Value++;
            LbPbValue.Text = PbDownloadProgress.Value.ToString();
        }
        private void DownloadController_PageDownloadFailed(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "DownloadController_PageDownloadFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion





























        #region TIMESHEET EVALUTATION
        private void btnEvaluateTS_Click(object sender, RoutedEventArgs e)
        {
            if (Shared.DefaultPayrollCode is not null)
            {
                var payrollCode = Shared.DefaultPayrollCode;
                var cutoff = Shared.DefaultCutoff;
                TSDownloadController.EvaluateTimesheets(cutoff.CutoffDate, payrollCode);
            }
        }
        private void DownloadController_EvaluationStarted(object? sender, EventArgs e)
        {
        }
        private async void DownloadController_EvaluationSucceeded(object sender, EvaluationResultArgs e)
        {
            if (e.MissingPages is not null && e.MissingPages.Count > 0)
            {
                _ = TSDownloadController.StartDownload(Shared.DefaultCutoff, Shared.DefaultPayrollCode);
            }
            else
            {
                if (e.NoEETimesheets is not null)
                    await EEDownloadController.FindEmployeeAsync(e.NoEETimesheets.ToArray());

                await TimesheetController.FillEmployeeDetail(Shared.DefaultCutoff.CutoffId, Shared.DefaultPayrollCode);

                if (e.UnconfirmedTimesheetsWithAttendance is not null)
                {
                    if (e.Timesheets is not null && e.UnconfirmedTimesheetsWithAttendance is not null && e.UnconfirmedTimesheetsWithoutAttendance is not null)
                        TimesheetOutputController.ExportEfile(Shared.DefaultCutoff, Shared.DefaultPayrollCode, "", e.Timesheets, e.UnconfirmedTimesheetsWithAttendance, e.UnconfirmedTimesheetsWithoutAttendance);
                }
            }
        }
        private void DownloadController_EvaluationFailed(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "DownloadController_EvaluationFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion





























        #region EMPLOYEE DOWNLOAD
        private void EEDownloadController_EmployeeDownloadStarted(object sender, int totalEmployees)
        {
            LbStatusMessage.Text = "Downloading Employees";
            PbDownloadProgress.Maximum = totalEmployees + 1;
            PbDownloadProgress.Value = 0;

            LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
            LbPbValue.Text = "0";
        }
        private void EEDownloadController_EmployeeDownloadError(object sender, string eeId, string errorMessage)
        {
            MessageBox.Show(errorMessage, "EEDownloadController_EmployeeDownloadError", MessageBoxButton.OK, MessageBoxImage.Error);
            PbDownloadProgress.Value = 0;
        }
        private void EEDownloadController_EmployeeDownloadSucceed(object sender, string eeId)
        {
            PbDownloadProgress.Value++;
            LbPbValue.Text = PbDownloadProgress.Value.ToString();
        }
        #endregion




























        #region FILL EMPLOYEE DETAIL
        private void TimesheetController_TimesheetFillStarted(object sender, int maximum)
        {
            Dispatcher.Invoke(() =>
            {
                LbStatusMessage.Text = "Filling Timesheets with Employee Detail";
                PbDownloadProgress.Maximum = maximum;
                PbDownloadProgress.Value = 0;

                LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
                LbPbValue.Text = "0";
            });
        }
        private void TimesheetController_TimesheetFilled(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                PbDownloadProgress.Value++;
                LbPbValue.Text = PbDownloadProgress.Value.ToString();
            });
        }
        private void TimesheetController_TimesheetFillFailed(object sender, string errorMessage)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(errorMessage, "TimesheetController_TimesheetFillFailed", MessageBoxButton.OK, MessageBoxImage.Error);
                PbDownloadProgress.Value = 0;
            });
        }
        #endregion

         



        #region TIMESHEET EXPORT
        private void TimesheetOutputController_ExportStarted(object? sender, EventArgs e)
        {
            PbDownloadProgress.Maximum = 1;
        }
        private void TimesheetOutputController_ExportFailed(object sender, string failedReason)
        {
            MessageBox.Show(failedReason, "TimesheetOutputController_ExportFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void TimesheetOutputController_ExportEnded(object? sender, EventArgs e)
        {
            LbStatusMessage.Text = "DONE";
            PbDownloadProgress.Value = 1;
            ReloadList();
        }
        #endregion



























        private void btnExportDBF_Click(object sender, RoutedEventArgs e)
        {
            if (Shared.DefaultPayrollCode is not null)
            {
                var PayrollCode = Shared.DefaultPayrollCode;
                var cutoff = Shared.DefaultCutoff;

                string dbfPath = $@"{AppDomain.CurrentDomain.BaseDirectory}/DBF/{cutoff.CutoffDate:yyyyMMdd}";
                Directory.CreateDirectory(dbfPath);

                IEnumerable<string> bankCategories = EmployeeController.ListBankCategories(PayrollCode);
                foreach (string bankCategory in bankCategories)
                {
                    //TimesheetOutputController.SavePayrollTimeToDBF(PayrollDate, PayrollCode, PayRegisterId, bankCategory, dbfPath);
                }
            }
        }

        private void btnExportEFile_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}