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

namespace Pms.AdjustmentModule.FrontEnd.Commands
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        BillingListingVm _viewModel;
        Billings Billings;
        private bool executable;

        public Listing(BillingListingVm viewModel, Billings billings)
        {
            _viewModel = viewModel;
            Billings = billings;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;

            try
            {
                IEnumerable<Billing> billingItems = new List<Billing>();
                await Task.Run(() =>
                {
                    billingItems = Billings.GetBillings(_viewModel.CutoffId.Substring(0, 4));
                });

                _viewModel.AdjustmentNames = billingItems.ExtractAdjustmentNames();
                billingItems = billingItems
                    .FilterPayrollCode(_viewModel.PayrollCodeId)
                    .FilterAdjustmentName(_viewModel.AdjustmentName);

                _viewModel.Billings = billingItems;
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
        public static IEnumerable<Billing> FilterPayrollCode(this IEnumerable<Billing> payrolls, string payrollCode)
        {
            if (payrollCode != string.Empty)
                return payrolls.Where(p => p.PayrollCode == payrollCode);
            return payrolls;
        }
        public static IEnumerable<Billing> FilterAdjustmentName(this IEnumerable<Billing> payrolls, string adjustmentName)
        {
            if (adjustmentName != string.Empty)
                return payrolls.Where(p => p.AdjustmentName == adjustmentName);
            return payrolls;
        }

    }
}
