using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;
using Pms.Main.FrontEnd.Wpf.ViewModel;
using Pms.Timesheets.Domain;
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;

namespace Pms.Main.FrontEnd.Wpf.Views
{

    public partial class TimesheetPage
    {
 
        public TimesheetPage()
        {
            InitializeComponent();
        }
 


        private void GenerateDBFPage_Loaded(object sender, RoutedEventArgs e)
        {
 
        }

       










        #region TIMESHEET DOWNLOAD
        //private void btnDownloadTS_Click(object sender, RoutedEventArgs e)
        //{
        //    SelectDownloadOption downloadOptionsDialog = new();
        //    bool? dialogResult = downloadOptionsDialog.ShowDialog();
        //    if (dialogResult is not null and true)
        //        _ = DownloadTimesheetsVM.StartDownload(downloadOptionsDialog.DownloadOptions);
        //}
        //private void DownloadController_DownloadStarted(object sender, int TotalPages)
        //{
        //    PbDownloadProgress.Maximum = TotalPages + 1;
        //    PbDownloadProgress.Value = 0;

        //    LbStatusMessage.Text = "Downloading Timesheets";
        //    LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
        //    LbPbValue.Text = "0";
        //}
        //private void DownloadController_DownloadEnded(object? sender, EventArgs e)
        //{
        //    PbDownloadProgress.Maximum = 0;
        //    PbDownloadProgress.Value = 0;

        //    LbPbMaximum.Text = "0";
        //    LbPbValue.Text = "0";

        //    EvaluateTimesheetsVM.EvaluateTimesheets();
        //}

        //private void DownloadController_PageDownloadSucceeded(object sender, int Page)
        //{
        //    PbDownloadProgress.Value++;
        //    LbPbValue.Text = PbDownloadProgress.Value.ToString();
        //}
        //private void DownloadController_PageDownloadFailed(object sender, string errorMessage)
        //{
        //    MessageBox.Show(errorMessage, "DownloadController_PageDownloadFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        //}
        #endregion




















        #region TIMESHEET EVALUTATION
        //private void btnEvaluateTS_Click(object sender, RoutedEventArgs e)
        //{
        //    EvaluateTimesheetsVM.EvaluateTimesheets();
        //}
        //private void DownloadController_EvaluationStarted(object? sender, EventArgs e)
        //{
        //}
        //private async void DownloadController_EvaluationSucceeded(object sender, EvaluationResultArgs e)
        //{
        //    if (e.MissingPages is not null && e.MissingPages.Count > 0)
        //    {
        //        _ = DownloadTimesheetsVM.StartDownload(e.MissingPages.ToArray());
        //    }
        //    else
        //    {
        //        if (e.NoEETimesheets is not null)
        //            await EEDownloadController.FindEmployeeAsync(e.NoEETimesheets.ToArray());

        //        await FilterTimesheetsVM.FillEmployeeDetail();

        //        if (e.UnconfirmedTimesheetsWithAttendance is not null)
        //        {
        //            if (e.Timesheets is not null && e.UnconfirmedTimesheetsWithAttendance is not null && e.UnconfirmedTimesheetsWithoutAttendance is not null)
        //                ExportTimesheetsVM.ExportEfile("", e.Timesheets, e.UnconfirmedTimesheetsWithAttendance, e.UnconfirmedTimesheetsWithoutAttendance);
        //        }
        //    }
        //}
        //private void DownloadController_EvaluationFailed(object sender, string errorMessage)
        //{
        //    MessageBox.Show(errorMessage, "DownloadController_EvaluationFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        //}
        #endregion






















        #region EMPLOYEE DOWNLOAD
        private void EEDownloadController_EmployeeDownloadStarted(object sender, int totalEmployees)
        {
            //LbStatusMessage.Text = "Downloading Employees";
            //PbDownloadProgress.Maximum = totalEmployees + 1;
            //PbDownloadProgress.Value = 0;

            //LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
            //LbPbValue.Text = "0";
        }
        private void EEDownloadController_EmployeeDownloadError(object sender, string eeId, string errorMessage)
        {
            //MessageBox.Show(errorMessage, "EEDownloadController_EmployeeDownloadError", MessageBoxButton.OK, MessageBoxImage.Error);
            //PbDownloadProgress.Value = 0;
        }
        private void EEDownloadController_EmployeeDownloadSucceed(object sender, string eeId)
        {
            //PbDownloadProgress.Value++;
            //LbPbValue.Text = PbDownloadProgress.Value.ToString();
        }
        #endregion




















        #region FILL EMPLOYEE DETAIL
        private void TimesheetController_TimesheetFillStarted(object sender, int maximum)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    LbStatusMessage.Text = "Filling Timesheets with Employee Detail";
            //    PbDownloadProgress.Maximum = maximum;
            //    PbDownloadProgress.Value = 0;

            //    LbPbMaximum.Text = PbDownloadProgress.Maximum.ToString();
            //    LbPbValue.Text = "0";
            //});
        }
        private void TimesheetController_TimesheetFilled(object? sender, EventArgs e)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    PbDownloadProgress.Value++;
            //    LbPbValue.Text = PbDownloadProgress.Value.ToString();
            //});
        }
        private void TimesheetController_TimesheetFillFailed(object sender, string errorMessage)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    MessageBox.Show(errorMessage, "TimesheetController_TimesheetFillFailed", MessageBoxButton.OK, MessageBoxImage.Error);
            //    PbDownloadProgress.Value = 0;
            //});
        }
        #endregion



















        #region TIMESHEET EXPORT
        private void btnExportDBF_Click(object sender, RoutedEventArgs e)
        {
            //ExportTimesheetsVM.Export();
        }

        private void TimesheetOutputController_ExportStarted(object? sender, EventArgs e)
        {
            //PbDownloadProgress.Maximum = 1;
        }
        private void TimesheetOutputController_ExportFailed(object sender, string failedReason)
        {
            //MessageBox.Show(failedReason, "TimesheetOutputController_ExportFailed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void TimesheetOutputController_ExportEnded(object? sender, EventArgs e)
        {
            //LbStatusMessage.Text = "DONE";
            //PbDownloadProgress.Value = 1;
            //ReloadList();
        }























        #endregion

        private void btnDownloadTS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEvaluateTS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExportDBF_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}