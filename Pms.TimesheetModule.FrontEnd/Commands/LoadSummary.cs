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
        private Models.Timesheets Timesheets;
        private TimesheetListingVm ListingVm;

        public LoadSummary(TimesheetListingVm viewModel, Models.Timesheets timesheets)
        {
            Timesheets = timesheets;
            ListingVm = viewModel;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public event EventHandler? CanExecuteChanged;
         
        public async void Execute(object? parameter)
        { 
            string site = ListingVm.Site.ToString();
            string payrollCode = ListingVm.PayrollCode.Name;
            Cutoff cutoff = ListingVm.Cutoff;
            cutoff.SetSite(site);

            var summary = await Timesheets.DownloadContentSummary(cutoff, payrollCode, site);
            IEnumerable<Timesheet> timesheets = Timesheets.MapEmployeeView(summary.UnconfirmedTimesheet);

            ListingVm.Timesheets = new ObservableCollection<Timesheet>(timesheets);
            ListingVm.NotConfirmed = ListingVm.Timesheets.Count;

            ListingVm.Confirmed = int.Parse(summary.TotalConfirmed);
            ListingVm.TotalTimesheets = int.Parse(summary.TotalCount);

            ListingVm.SetAsFinishProgress();
        }


        public async Task StartDownload(int[] pages, Cutoff cutoff, string payrollCode, string site)
        {
            ListingVm.SetProgress("Downloading Timesheets", pages.Length);

            foreach (int page in pages)
            {
                await DownloadPage(page, cutoff, payrollCode, site);
                ListingVm.ProgressValue++;
            }


            ListingVm.SetAsFinishProgress();
        }

        public async Task DownloadPage(int page, Cutoff cutoff, string payrollCode, string site)
        {
            while (true)
            {
                try
                {
                    IEnumerable<Timesheet> timesheets = await Timesheets.DownloadContent(cutoff, payrollCode,  site, page);
                    foreach (Timesheet timesheet in timesheets)
                    {
                        EmployeeView ee = Timesheets.FindEmployeeView(timesheet.EEId);
                        timesheet.EE = ee;
                        timesheet.CutoffId = cutoff.CutoffId;
                        ListingVm.Timesheets.Add(timesheet);
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


        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
