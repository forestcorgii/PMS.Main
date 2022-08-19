using Microsoft.Toolkit.Mvvm.Input;
using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class BillingGenerationCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private ViewModelBase _viewModel;
        private BillingModel _model;
        private EmployeeModel _employeeModel;
        private BillingStore _store;
        private MainStore _mainStore;


        public BillingGenerationCommand(ViewModelBase viewModel, BillingModel model, BillingStore store, MainStore mainStore, EmployeeModel employeeModel)
        {
            _model = model;
            _viewModel = viewModel;
            _store = store;
            _mainStore = mainStore;
            _employeeModel = employeeModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await Task.Run(() =>
            {
                string[] eeIds = _employeeModel.FilterEmployees("", _mainStore.PayrollCode).Select(ee => ee.EEId).ToArray();
                List<Billing> billings = new();

                _viewModel.SetProgress("Billings Generation on going.", eeIds.Length);
                foreach (string eeId in eeIds)
                {
                    billings.AddRange(_model.GenerateBillings(_mainStore.Cutoff.CutoffId, eeId));
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetProgress("Saving Generated billings.", billings.Count);
                foreach (Billing billing in billings)
                {
                    billing.PayrollCode = _mainStore.PayrollCode;
                    _model.AddBilling(billing);
                    _viewModel.ProgressValue++;
                }

                _viewModel.SetAsFinishProgress();
            });
        }

        public void NotifyCanExecuteChanged() { }
    }
}
