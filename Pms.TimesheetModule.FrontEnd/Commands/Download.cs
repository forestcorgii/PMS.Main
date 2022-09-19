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
        private TimesheetListingVm _viewModel;
        private Models.Timesheets _cutoffTimesheet;

        public Download(TimesheetListingVm viewModel, Models.Timesheets cutoffTimesheet)
        {
            _viewModel = viewModel;
            _cutoffTimesheet = cutoffTimesheet;
        }

        public event EventHandler? CanExecuteChanged;

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;

            string site = _viewModel.Site.ToString();
            string payrollCode = _viewModel.PayrollCode.Name;
            Cutoff cutoff = _viewModel.Cutoff;
            cutoff.SetSite(site);

            if (parameter is null)
            {
                _viewModel.SetProgress("Retrieving Download content summary", 1);
                int[] pages;
                pages = await _cutoffTimesheet.DownloadContentSummary(cutoff, payrollCode, site);

                await StartDownload(pages, cutoff, payrollCode, site);
            }
            else
                await StartDownload((int[])parameter, cutoff, payrollCode, site);
         
            executable = true;
        }

        public async Task StartDownload(int[] pages, Cutoff cutoff, string payrollCode, string site)
        {
            _viewModel.SetProgress("Downloading Timesheets", pages.Length);

            foreach (int page in pages)
            {
                while (true)
                {
                    try
                    {
                        IEnumerable<Timesheet> timesheets = await _cutoffTimesheet.DownloadContent(cutoff, payrollCode, site, page);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!MessageBoxes.Inquire(
                            $"{ex.Message}... Do You want to Retry?",
                            "Timesheet Download Error..."
                        ))
                            break;
                    }
                }
                _viewModel.ProgressValue++;
            }
            _viewModel.SetAsFinishProgress();
        }

        public void NotifyCanExecuteChanged() { }
    }
}
