using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.TimesheetModule.FrontEnd.ViewModels;
using Pms.Timesheets.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.TimesheetModule.FrontEnd.Commands
{
    public class EvaluateAll : IRelayCommand
    {
        private TimesheetListingVm ListingVm;
        private Models.Timesheets Timesheets;

        public event EventHandler? CanExecuteChanged;

        public EvaluateAll(TimesheetListingVm listingVm, Models.Timesheets timesheets)
        {
            ListingVm = listingVm;
            Timesheets = timesheets;
        }

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            string cutoffId = ListingVm.Cutoff.CutoffId;
            string payrollCode = ListingVm.PayrollCode.PayrollCodeId;

            int[] missingPages = Timesheets.GetMissingPages(cutoffId, payrollCode);
            if (missingPages is not null && missingPages.Length == 0)
            {
                try
                {
                    IEnumerable<string> noEETimesheets = Timesheets.ListTimesheetNoEETimesheet(cutoffId);

                    await FillEmployeeDetail();
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message, "Timesheet Evaluation Error"); }

            }
            else
                ListingVm.DownloadCommand.Execute(missingPages);
            ListingVm.LoadTimesheets.Execute(null);
        }


        public Task FillEmployeeDetail()
        {
            return Task.Run(() =>
           {
               if (ListingVm.Cutoff is not null)
               {
                   try
                   {
                       List<Timesheet> timesheetItems = Timesheets
                           .GetTimesheets(ListingVm.Cutoff.CutoffId)
                           .Where(ts => ts.EE.PayrollCode == ListingVm.PayrollCode.PayrollCodeId)
                           .ToList();

                       ListingVm.SetProgress("Filling Employee detail to Timesheets", timesheetItems.Count());

                       foreach (Timesheet timesheet in timesheetItems)
                       {
                           Timesheets.SaveEmployeeData(timesheet);
                           ListingVm.ProgressValue++;
                       }
                   }
                   catch (Exception ex) { MessageBoxes.Error(ex.Message, "Timesheet Evaluation Error"); }

                   ListingVm.SetAsFinishProgress();
               }
           });
        }

        public void NotifyCanExecuteChanged()
        {

        }
    }
}
