using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollExportLBPCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollStore _store;
        private readonly MainStore _mainStore;
        private readonly PayrollViewModel _viewModel;
        private readonly PayrollModel _model;

        private bool _canExecute { get; set; } = true;

        public PayrollExportLBPCommand(PayrollViewModel viewModel, PayrollModel model, PayrollStore store, MainStore mainStore)
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
                    _viewModel.SetProgress("Exporting Payrolls for Land Bank.",1);
                    string cutoffId = _mainStore.Cutoff.CutoffId;
                    IEnumerable<Payrolls.Domain.Payroll> payrolls = _model.Get(cutoffId, BankType.LBP);
                    _model.ExportLBP(payrolls, cutoffId, "LBP");
                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
