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
    public class ExportMacro : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollViewModel _viewModel;
        private readonly Models.Payrolls _model;

        private bool executable { get; set; } = true;

        public ExportMacro(PayrollViewModel viewModel, Models.Payrolls model)
        {
            _viewModel = viewModel;
            _model = model;
        }


        public bool CanExecute(object? parameter) => executable;


        public async void Execute(object? parameter)
        {
            if (_viewModel.Cutoff.CutoffDate.Day == 15)
            {
                MessageBoxes.Prompt("Can only export Macros on 30th Cutoff.", "");
                return; 
            }
            executable = false;
            NotifyCanExecuteChanged();

            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Exporting Alphalist.", 1);

                    Cutoff cutoff = new(_viewModel.Cutoff.CutoffId);
                    Company company = _viewModel.Company;
                    if (company is not null)
                    {
                        IEnumerable<MonthlyPayroll> payrolls = _model.GetMonthlyPayrolls(cutoff.CutoffDate.Month, _viewModel.Company.CompanyId);

                        _model.ExportMacro(payrolls, cutoff, company.CompanyId);
                        _model.ExportMacroB(payrolls, cutoff, company.CompanyId);

                        _viewModel.SetAsFinishProgress();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex.Message,
                    "Alphalist Export Error");
            }
            executable = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
