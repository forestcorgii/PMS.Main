using Microsoft.Toolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.AdjustmentModule.FrontEnd.Commands
{
    public class Export : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly BillingListingVm _viewModel;
        private readonly Billings Billings;

        private bool executable = true;

        public Export(BillingListingVm viewModel, Billings model)
        {
            _viewModel = viewModel;
            Billings = model;
        }


        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Exporting Payrolls for Land Bank.", 1);
                    string cutoffId = _viewModel.CutoffId;
                    string payrollCode = _viewModel.PayrollCodeId;
                    string adjustmentName = _viewModel.AdjustmentName;
                    IEnumerable<Billing> billingItems = _viewModel.Billings;

                    Billings.Export(billingItems, cutoffId, $"{cutoffId}_{payrollCode}_{adjustmentName}.xls");
                    _viewModel.SetAsFinishProgress();
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
