using Microsoft.Toolkit.Mvvm.Input;
using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class BillingExportCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly BillingStore _store;
        private readonly MainStore _mainStore;
        private readonly BillingViewModel _viewModel;
        private readonly BillingModel _model;

        private bool _canExecute { get; set; } = true;

        public BillingExportCommand(BillingViewModel viewModel, BillingModel model, BillingStore store, MainStore mainStore)
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
                    string payrollCode= _mainStore.PayrollCode;
                    string adjustmentName= _viewModel.AdjustmentName;
                    IEnumerable<Billing> billings = _viewModel.Billings;

                    _model.Export(billings, cutoffId, $"{cutoffId}_{payrollCode}_{adjustmentName}.xls");
                    _viewModel.SetAsFinishProgress();
                });
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage= ex.Message;
            }
            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
