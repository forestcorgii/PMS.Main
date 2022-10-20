using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class LoadSummary : IRelayCommand
    {
        private TimesheetListingVm ViewModel;
        private Models.Timesheets Timesheets;

        public LoadSummary(TimesheetListingVm viewModel, Models.Timesheets timesheets)
        {
            ViewModel = viewModel;
            Timesheets = timesheets;
        }

        public event EventHandler? CanExecuteChanged;

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            string site = ViewModel.Site.ToString();
            string payrollCode = ViewModel.PayrollCode.Name;
            Cutoff cutoff = ViewModel.Cutoff;
            cutoff.SetSite(site);

            var summary = await Timesheets.DownloadContentSummary(cutoff, payrollCode, site);
            IEnumerable<Timesheet> timesheets = Timesheets.MapEmployeeView(summary.UnconfirmedTimesheet);

            ViewModel.Timesheets = new ObservableCollection<Timesheet>(timesheets);
            ViewModel.NotConfirmed = ViewModel.Timesheets.Count;

            ViewModel.Confirmed = int.Parse(summary.TotalConfirmed);
            ViewModel.TotalTimesheets = int.Parse(summary.TotalCount);

            ViewModel.SetAsFinishProgress();

            executable = true;
            NotifyCanExecuteChanged();
        }


        public async Task StartDownload(int[] pages, Cutoff cutoff, string payrollCode, string site)
        {
            ViewModel.SetProgress("Downloading Timesheets", pages.Length);

            foreach (int page in pages)
            {
                await DownloadPage(page, cutoff, payrollCode, site);
                ViewModel.ProgressValue++;
            }


            ViewModel.SetAsFinishProgress();
        }

        public async Task DownloadPage(int page, Cutoff cutoff, string payrollCode, string site)
        {
            while (true)
            {
                try
                {
                    IEnumerable<Timesheet> timesheets = await Timesheets.DownloadContent(cutoff, payrollCode, ViewModel.PayrollCode.PayrollCodeId, site, page);
                    foreach (Timesheet timesheet in timesheets)
                    {
                        EmployeeView ee = Timesheets.FindEmployeeView(timesheet.EEId);
                        timesheet.EE = ee;
                        timesheet.PayrollCode = ViewModel.PayrollCode.PayrollCodeId;
                        timesheet.CutoffId = cutoff.CutoffId;
                        ViewModel.Timesheets.Add(timesheet);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (!MessageBoxes.Inquire($"{ex.Message}... Do You want to Retry?"))
                        break;
                }
            }
        }


        public void NotifyCanExecuteChanged() =>
         CanExecuteChanged?.Invoke(this, new());
    }
}
