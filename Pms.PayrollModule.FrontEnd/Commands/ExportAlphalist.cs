using Microsoft.Toolkit.Mvvm.Input;
using Pms.Masterlists.Domain;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pms.Main.FrontEnd.Common.Utils;

namespace Pms.PayrollModule.FrontEnd.Commands
{
    public class ExportAlphalist : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollViewModel _viewModel;
        private readonly Models.Payrolls _model;

        private bool _canExecute { get; set; } = true;

        public ExportAlphalist(PayrollViewModel viewModel, Models.Payrolls model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => _canExecute;


        public async void Execute(object? parameter)
        {
            _canExecute = false;
            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Exporting Alphalist.", 1);

                    Cutoff cutoff = new(_viewModel.Cutoff.CutoffId);
                    Company company = _viewModel.Company;
                    if (company is not null)
                    {
                        IEnumerable<IEnumerable<Payroll>> employeePayrolls =
                            _model.GetYearlyPayrollsByEmployee(cutoff.YearCovered, _viewModel.Company.CompanyId);

                        List<AlphalistDetail> alphalists = new();
                        foreach (var employeePayroll in employeePayrolls)
                        {
                            AutomatedAlphalistDetail alphaDetailFactory = new(employeePayroll, company.MinimumRate, cutoff.YearCovered);
                            alphalists.Add(alphaDetailFactory.CreateAlphalistDetail());
                        }

                        _model.ExportAlphalist(alphalists, cutoff.YearCovered, company);
                        _model.ExportAlphalistVerifier(employeePayrolls, cutoff.YearCovered, company);

                        _viewModel.SetAsFinishProgress();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex.Message,
                    "Alphalist Export Error");
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
