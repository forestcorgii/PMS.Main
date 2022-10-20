using Microsoft.Toolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Adjustments.Domain.Enums;

namespace Pms.AdjustmentModule.FrontEnd.Commands
{
    public class Export : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly BillingListingVm _viewModel;
        private readonly Models.Billings Billings;

        private bool executable = true;

        public Export(BillingListingVm viewModel, Models.Billings model)
        {
            _viewModel = viewModel;
            Billings = model;
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
                    _viewModel.SetProgress("Exporting Payrolls for Land Bank.", 1);
                    string cutoffId = _viewModel.CutoffId;
                    string payrollCode = _viewModel.PayrollCodeId;
                    AdjustmentTypes adjustmentType = _viewModel.AdjustmentName;
                    IEnumerable<Billing> billingItems = _viewModel.Billings;

                    Billings.Export(billingItems, adjustmentType, $"{cutoffId}_{payrollCode}_{adjustmentType}.xls");
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
