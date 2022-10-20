using CommunityToolkit.Mvvm.Input;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.Adjustments.Domain;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.AdjustmentModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pms.Adjustments.Domain.Models;
using Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records;

namespace Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        BillingRecordListingVm _viewModel;
        BillingRecords Records;
        private bool executable;

        public Listing(BillingRecordListingVm viewModel, BillingRecords billings)
        {
            _viewModel = viewModel;
            Records = billings;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            try
            {
                IEnumerable<BillingRecord> billingRecordItems = new List<BillingRecord>();
                await Task.Run(() =>
                {
                    billingRecordItems = Records.GetByPayrollCode(_viewModel.PayrollCodeId);
                });

                
                _viewModel.BillingRecords = billingRecordItems;
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }

    static class PayrollFilterExtension
    {
        //public static IEnumerable<Billing> FilterPayrollCode(this IEnumerable<BillingRecord> payrolls, string payrollCode)
        //{
        //    if (payrollCode != string.Empty)
        //        return payrolls.Where(p => p.EE.PayrollCode == payrollCode);
        //    return payrolls;
        //}

    }
}
