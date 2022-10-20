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
using Pms.Adjustments.Domain.Enums;

namespace Pms.AdjustmentModule.FrontEnd.Commands
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        BillingListingVm _viewModel;
        Models.Billings Billings;
        private bool executable;

        public Listing(BillingListingVm viewModel, Models.Billings billings)
        {
            _viewModel = viewModel;
            Billings = billings;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            try
            {
                IEnumerable<Billing> billingItems = new List<Billing>();
                await Task.Run(() =>
                {
                    billingItems = Billings.GetBillings(_viewModel.CutoffId);
                });

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
                return payrolls.Where(p => p.EE.PayrollCode == payrollCode);
            return payrolls;
        }
        public static IEnumerable<Billing> FilterAdjustmentName(this IEnumerable<Billing> payrolls, AdjustmentTypes adjustmentType)
        {
            return payrolls.Where(p => p.AdjustmentType == adjustmentType);
        }

    }
}
