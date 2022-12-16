using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class Download : IRelayCommand
    {
        private TimesheetListingVm ListingVm;
        private Models.Timesheets Timesheets;

        public Download(TimesheetListingVm viewModel, Models.Timesheets timesheets)
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
            try
            {
                if (parameter is int page)
                {
                    ListingVm.SetProgress($"Downloading Timesheet Page {page}..", 1);
                    await DownloadPage(page, cutoff, payrollCode, site);
                }
                else if (parameter is int[] pages)
                {
                    await StartDownload(pages, cutoff, payrollCode, site);
                }
                else
                {
                    ListingVm.SetProgress("Retrieving Download content summary", 1);
                    DownloadSummary<Timesheet> summary = await Timesheets.DownloadContentSummary(cutoff, payrollCode, site);
                    pages = Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray();

                    await StartDownload(pages, cutoff, payrollCode, site);
                }
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }
            ListingVm.SetAsFinishProgress();
        }


        public async Task StartDownload(int[] pages, Cutoff cutoff, string payrollCode, string site)
        {
            ListingVm.SetProgress($"Downloading {payrollCode} Timesheets", pages.Length);

            foreach (int page in pages)
            {
                await DownloadPage(page, cutoff, payrollCode, site);
                if (!ListingVm.IncrementProgress())
                    break;
            }


            ListingVm.SetAsFinishProgress();
        }

        public async Task DownloadPage(int page, Cutoff cutoff, string payrollCode, string site)
        {
            while (true)
            {
                try
                {
                    IEnumerable<Timesheet> timesheets = await Timesheets.DownloadContent(cutoff, payrollCode, site, page);
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
