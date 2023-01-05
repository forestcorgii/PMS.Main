using Microsoft.Toolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Adjustments.Domain.Enums;
using Pms.TimesheetModule.FrontEnd.Models;
using Pms.Timesheets.Domain;
using System.Linq;

namespace Pms.AdjustmentModule.FrontEnd.Commands
{
    public class AddAdjustment : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly BillingListingVm Vm;
        private readonly Models.Billings Billings;
        private readonly Pms.TimesheetModule.FrontEnd.Models.Timesheets Timesheets;

        private bool executable = true;

        public AddAdjustment(BillingListingVm viewModel, Models.Billings model, TimesheetModule.FrontEnd.Models.Timesheets timesheets)
        {
            Vm = viewModel;
            Billings = model;
            Timesheets = timesheets;
        }


        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            try
            {
                await Task.Run(() =>
                {
                    Vm.SetProgress("Exporting Adjustments.", Vm.Billings.Count());
                    var timesheets = Timesheets.GetTimesheets(Vm.CutoffId);
                    foreach (Billing billing in Vm.Billings)
                    {
                        AdjustmentOptions adjustOption = AdjustmentOptions.ADJUST1;
                        if (parameter is string adjustTypeString)
                            adjustOption = (AdjustmentOptions)int.Parse(adjustTypeString);
                        else
                            break;
                        Timesheet timesheet = timesheets.Where(ts => ts.CutoffId == billing.CutoffId && ts.EEId == billing.EEId).First();
                        if (!billing.Applied)
                        {
                            if (adjustOption == AdjustmentOptions.ADJUST1)
                                timesheet.Adjust1 += billing.Amount;
                            else
                                timesheet.Adjust2 += billing.Amount;
                        }
                        else if (billing.Applied && billing.AdjustmentOption != adjustOption)
                        {
                            if (adjustOption == AdjustmentOptions.ADJUST1)
                            {
                                timesheet.Adjust2 -= billing.Amount;
                                timesheet.Adjust1 += billing.Amount;
                            }
                            else
                            {
                                timesheet.Adjust1 -= billing.Amount;
                                timesheet.Adjust2 += billing.Amount;
                            }
                        }

                        Timesheets.SaveTimesheet(timesheet);

                        billing.AdjustmentOption = adjustOption;
                        billing.Applied = true;
                        Billings.AddBilling(billing);

                        Vm.IncrementProgress();
                    }

                    Vm.SetAsFinishProgress();
                });
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
