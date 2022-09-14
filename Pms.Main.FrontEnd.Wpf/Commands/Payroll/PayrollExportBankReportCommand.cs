using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollExportBankReportCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollStore _store;
        private readonly MainStore _mainStore;
        private readonly PayrollViewModel _viewModel;
        private readonly PayrollModel _model;

        private bool _canExecute { get; set; } = true;

        public PayrollExportBankReportCommand(PayrollViewModel viewModel, PayrollModel model, PayrollStore store, MainStore mainStore)
        {
            _store = store;
            _viewModel = viewModel;
            _model = model;
            _mainStore = mainStore;
        }

        public bool CanExecute(object? parameter) => _canExecute;

        public async void Execute(object? parameter)
        {
            _canExecute = false;
            try
            {
                await Task.Run(() =>
                {
                    _viewModel.SetProgress("Exporting Payrolls.", 1);

                    string cutoffId = _mainStore.Cutoff.CutoffId;
                    string payrollCode = _mainStore.PayrollCode.PayrollCodeId;

                    IEnumerable<Payroll> payrolls = _model.Get(cutoffId, payrollCode);

                    _model.ExportBankReport(payrolls, cutoffId, payrollCode);
                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source,
                    "Bank Report Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
