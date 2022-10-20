using CommunityToolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records;
using Pms.AdjustmentModule.FrontEnd.Views.Billing_Records;
using Pms.Adjustments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records
{
    public class Detail : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private Models.BillingRecords Records;

        private Models.Employees Employees;

        public Detail(Models.BillingRecords model, Models.Employees employees)
        {
            Records = model;
            Employees = employees;
        }

        private bool executable = true;
        public bool CanExecute(object? parameter) => executable;

        public void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            BillingRecordDetailVm detailVm = new(new() { EffectivityDate = DateTime.Now }, Records, Employees);
            if (parameter is BillingRecord record)
                detailVm = new(record, Records, Employees);

            BillingRecordDetailView detailView = new() { DataContext = detailVm };
            detailVm.OnRequestClose += (s, e) => detailView.Close();
            detailView.ShowDialog();

            executable = true;
            NotifyCanExecuteChanged();
        }
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
