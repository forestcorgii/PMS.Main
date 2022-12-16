using Microsoft.Toolkit.Mvvm.Input;
using Pms.Masterlists.Domain;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;
using System.Linq;

namespace Pms.PayrollModule.FrontEnd.Commands
{
    public class Export13thMonth : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollViewModel _viewModel;
        private readonly Models.Payrolls _model;

        private bool _canExecute { get; set; } = true;

        public Export13thMonth(PayrollViewModel viewModel, Models.Payrolls model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => _canExecute;


        public async void Execute(object? parameter)
        {
            _canExecute = false;
            NotifyCanExecuteChanged();
            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Exporting 13th Month.", 1);

                    Cutoff cutoff = new(_viewModel.Cutoff.CutoffId);
                    Company company = _viewModel.Company;
                    if (company is not null)
                    {
                        IEnumerable<IEnumerable<Payroll>> employeePayrolls =
                            _model.GetYearlyPayrollsByEmployee(cutoff.YearCovered, _viewModel.PayrollCode.PayrollCodeId, _viewModel.Company.CompanyId);

                        List<Payroll> payrolls = new();

                        foreach (var employeePayroll in employeePayrolls)
                        {
                            Payroll payroll = employeePayroll.First();
                            Payroll _13thMonthPayroll = new()
                            {
                                EE = payroll.EE,
                                EEId = payroll.EEId,
                                CutoffId = $"{DateTime.Now:yy}{12}-13",
                                PayrollCode = payroll.PayrollCode,
                                YearCovered = payroll.YearCovered,
                                NetPay = employeePayroll.Sum(p =>
                                {
                                    if (p.RegHours > 96)
                                        return p.RegularPay / p.RegHours * 96;
                                    return p.RegularPay;
                                }) / 12,
                            };
                            payrolls.Add(_13thMonthPayroll);
                        }

                        _model.ExportBankReport(payrolls, $"{cutoff.CutoffDate:yy}{12}-13", _viewModel.PayrollCode.PayrollCodeId);

                        _viewModel.SetAsFinishProgress();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex.Message,
                    "13th Month Export Error");
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
