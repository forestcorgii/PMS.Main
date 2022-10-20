using CommunityToolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records;
using Pms.Adjustments.Domain.Models;
using Pms.Main.FrontEnd.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records
{
    public class Save : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private BillingRecordDetailVm ViewModel;

        private Models.BillingRecords Records;

        public Save(BillingRecordDetailVm viewModel, Models.BillingRecords model)
        {
            Records = model;
            ViewModel = viewModel;
        }

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            ViewModel.Record.EE = null;
            ViewModel.Record.RecordId = BillingRecord.GenerateId(ViewModel.Record);
            Records.SaveRecord(ViewModel.Record);

            MessageBoxes.Prompt("Changes has been successfully saved.", "");

            ViewModel.Close();

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
