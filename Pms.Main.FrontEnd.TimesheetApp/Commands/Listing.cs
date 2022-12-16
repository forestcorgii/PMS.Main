using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Main.FrontEnd.TimesheetApp.ViewModels;
using Pms.Masterlists.Domain;
using Pms.MasterlistModule.FrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.TimesheetApp.Commands
{
    public class Listing : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private MainViewModel _viewModel;
        private TimesheetModule.FrontEnd.Models.Timesheets _timesheetModel;
        private Companies _companies;
        private PayrollCodes _payrollCodes;

        public Listing(MainViewModel viewModel, TimesheetModule.FrontEnd.Models.Timesheets timesheetModel, PayrollCodes payrollCodes, Companies companies)
        {
            _viewModel = viewModel;

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
                List<PayrollCode> payrollCodes = new();
                IEnumerable<Company> companies = new List<Company>();
                await Task.Run(() =>
                {
                    cutoffIds = _timesheetModel
                        .ListCutoffIds()
                        .OrderByDescending(c => c)
                        .ToArray();

                    payrollCodes = _payrollCodes.ListPayrollCodes().ToList();
                    companies = _companies.ListCompanies();
                });

                _viewModel.Companies = companies;

                payrollCodes.Add(new PayrollCode());
                _viewModel.PayrollCodes = payrollCodes;

                _viewModel.CutoffIds = cutoffIds;
                _viewModel.CutoffId = cutoffIds.First();

            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            executable = true;
        }

        public void NotifyCanExecuteChanged() { }
    }
}
