using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Commands.Payroll_Codes
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;
        private bool executable = true;

        private readonly PayrollCodes PayrollCodes;
        private readonly Companies Companies;
        private readonly PayrollCodeDetailVm _viewModel;


        public Listing(PayrollCodeDetailVm viewModel, PayrollCodes model, Companies companies)
        {
            PayrollCodes = model;
            _viewModel = viewModel;
            Companies = companies;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();
            try
            {
                IEnumerable<PayrollCode> payrollCodes = new List<PayrollCode>();
                IEnumerable<Company> companies = new List<Company>();
                await Task.Run(() =>
                {
                    payrollCodes = PayrollCodes.ListPayrollCodes();
                    companies = Companies.ListCompanies();
                });

                _viewModel.PayrollCodes = new(payrollCodes);
                _viewModel.CompanyIds = new(companies.Select(c => c.CompanyId));
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }

}
