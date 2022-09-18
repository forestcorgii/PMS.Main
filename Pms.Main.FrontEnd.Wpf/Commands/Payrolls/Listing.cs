using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Commands.Payrolls
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        PayrollViewModel _viewModel;
        PayrollModel _model;
        private bool executable;

        public Listing(PayrollViewModel viewModel, PayrollModel model)
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
                IEnumerable<Payroll> payrolls = new List<Payroll>();
                await Task.Run(() =>
                {
                    payrolls = _model.Get(_viewModel.Cutoff.CutoffId)
                    .SetCompanyId(_viewModel.Company.CompanyId)
                    .SetPayrollCode(_viewModel.PayrollCode.PayrollCodeId);
                });

                _viewModel.Payrolls = payrolls;

                _viewModel.ChkCount = payrolls.Count(p => p.EE.Bank == BankChoices.CHK);
                _viewModel.LbpCount = payrolls.Count(p => p.EE.Bank == BankChoices.LBP);
                _viewModel.CbcCount = payrolls.Count(p => p.EE.Bank == BankChoices.CBC);
                _viewModel.MtacCount = payrolls.Count(p => p.EE.Bank == BankChoices.MTAC);
                _viewModel.MpaloCount = payrolls.Count(p => p.EE.Bank == BankChoices.MPALO);
                _viewModel.UnknownEECount = payrolls.Count(p => p.EE is null || p.EE.FirstName == string.Empty);
            
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

        public static IEnumerable<Payroll> SetCompanyId(this IEnumerable<Payroll> payrolls, string companyId)
        {
            if (companyId != string.Empty)
                return payrolls.Where(p => p.CompanyId == companyId);
            return payrolls;
        }
        public static IEnumerable<Payroll> SetPayrollCode(this IEnumerable<Payroll> payrolls, string payrollCode)
        {
            if (payrollCode != string.Empty)
                return payrolls.Where(p => p.PayrollCode == payrollCode);
            return payrolls;
        }
    }
}
