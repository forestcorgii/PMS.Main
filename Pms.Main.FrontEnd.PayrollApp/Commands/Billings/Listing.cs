using CommunityToolkit.Mvvm.Input;
using Pms.Adjustments.Domain;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands.Billings
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        BillingViewModel _viewModel;
        BillingModel _model;
        private bool executable;

        public Listing(BillingViewModel viewModel, BillingModel model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;

            try
            {
                IEnumerable<Billing> billings = new List<Billing>();
                await Task.Run(() =>
                {
                    billings = _model.GetBillings(_viewModel.CutoffId.Substring(0, 4));
                });

                _viewModel.AdjustmentNames = billings.ExtractAdjustmentNames();
                billings = billings
                    .FilterPayrollCode(_viewModel.PayrollCodeId)
                    .FilterAdjustmentName(_viewModel.PayrollCodeId);

                _viewModel.Billings = billings;
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
