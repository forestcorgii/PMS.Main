using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.PayrollModule.FrontEnd.Models;
using Pms.PayrollModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.PayrollModule.FrontEnd.Commands
{
    public class ExportBankReport : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollViewModel _viewModel;
        private readonly Models.Payrolls _model;

        private bool _canExecute { get; set; } = true;

        public ExportBankReport(PayrollViewModel viewModel, Models.Payrolls model)
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
                    _viewModel.SetProgress("Exporting Payrolls.", 1);

                    string cutoffId = _viewModel.Cutoff.CutoffId;
                    string payrollCode = _viewModel.PayrollCode.PayrollCodeId;

                    IEnumerable<Payroll> payrolls = _model.Get(cutoffId, payrollCode);

                    _model.ExportBankReport(payrolls, cutoffId, payrollCode);
                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex.Message,
                    "Bank Report Export Error");
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
