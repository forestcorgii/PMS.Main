using Microsoft.Toolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Common.Messages;

namespace Pms.AdjustmentModule.FrontEnd
{
    public class Generate : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private BillingListingVm _viewModel;
        private Billings billings;


        public Generate(BillingListingVm viewModel, Billings model)
        {
            billings = model;
            _viewModel = viewModel;
        }

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            await Task.Run(() =>
            {
                string[] eeIds = billings.GetEmployeesWithPcv(_viewModel.PayrollCodeId, _viewModel.CutoffId)
                    .Union(billings.GetEmployeesWithBillingRecord(_viewModel.PayrollCodeId, _viewModel.CutoffId))
                    .ToArray();

                List<Billing> billingItems = new();

                _viewModel.SetProgress("Billings Generation on going.", eeIds.Length);
                foreach (string eeId in eeIds)
                {
                    billings.ResetBillings(_viewModel.CutoffId, eeId);
                    billingItems.AddRange(billings.GenerateBillingFromBillingRecord(_viewModel.CutoffId, eeId));
                    billingItems.AddRange(billings.GenerateBillingFromTimesheetView(_viewModel.CutoffId, eeId));
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetProgress("Saving Generated billings from PCVs and Allowances.", billingItems.Count);
                foreach (Billing billing in billingItems)
                {
                    billings.AddBilling(billing);
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetAsFinishProgress();
                _viewModel.ListBillings.Execute(true);
            });

            executable = true;
            NotifyCanExecuteChanged();
        }
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
