using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Main.FrontEnd.PayrollApp.ViewModels;
using Pms.Masterlists.Domain;
using Pms.MasterlistModule.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.PayrollApp.Commands
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private MainViewModel _viewModel;
        private TimesheetModule.FrontEnd.Models.Timesheets _timesheetModel;
        private PayrollModule.FrontEnd.Models.Payrolls _payrollModel;
        private Companies _companies;
        private PayrollCodes _payrollCodes;

        public Listing(MainViewModel viewModel, PayrollModule.FrontEnd.Models.Payrolls payrollModel, TimesheetModule.FrontEnd.Models.Timesheets timesheetModel, PayrollCodes payrollCodes, Companies companies)
        {
            _viewModel = viewModel;

            _payrollModel = payrollModel;
            _timesheetModel = timesheetModel;

            _companies = companies;
            _payrollCodes = payrollCodes;
        }

        private bool executable;
        public bool CanExecute(object? parameter) => executable;

        public async void Execute(object? parameter)
        {
            executable = false;
            try
            {
                string[] cutoffIds = new string[] { };
                IEnumerable<PayrollCode> payrollCodes = new List<PayrollCode>();
                IEnumerable<Company> companies = new List<Company>();
                await Task.Run(() =>
                {
                    cutoffIds = _timesheetModel
                        .ListCutoffIds()
                        .Union(_payrollModel.ListCutoffIds())
                        .OrderByDescending(c => c)
                        .ToArray();

                    payrollCodes = _payrollCodes.ListPayrollCodes();
                    companies = _companies.ListCompanies();
                });

                _viewModel.Companies = companies;
                _viewModel.PayrollCodes = payrollCodes;

                _viewModel.CutoffIds = cutoffIds;
                _viewModel.CutoffId = cutoffIds.First();
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
        }

        public void NotifyCanExecuteChanged()
        {
            throw new NotImplementedException();
        }
    }
}
