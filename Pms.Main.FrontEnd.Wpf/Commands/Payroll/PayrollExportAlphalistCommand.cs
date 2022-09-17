using Microsoft.Toolkit.Mvvm.Input;
using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using Pms.Payrolls.ServiceLayer.Files.Exports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollExportAlphalistCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollStore _store;
        private readonly MainStore _mainStore;
        private readonly PayrollViewModel _viewModel;
        private readonly PayrollModel _model;

        private bool _canExecute { get; set; } = true;

        public PayrollExportAlphalistCommand(PayrollViewModel viewModel, PayrollModel model, PayrollStore store, MainStore mainStore)
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
                    _viewModel.SetProgress("Exporting Alphalist.", 1);

                    Cutoff cutoff = new(_mainStore.Cutoff.CutoffId);
                    Company? company = _mainStore.Companies.Where(c => c.CompanyId == _mainStore.PayrollCode.CompanyId).FirstOrDefault();
                    if (company is not null)
                    {
                        IEnumerable<Payroll> payrolls = _model.Get(cutoff.YearCovered, _mainStore.PayrollCode.CompanyId);
                        var employeePayrolls = payrolls.GroupBy(py => py.EEId).Select(py => py.ToList()).ToList();

                        List<AlphalistDetail> alphalists = new();
                        foreach (var employeePayroll in employeePayrolls)
                            alphalists.Add(new AutomatedAlphalistDetail(employeePayroll, company.MinimumRate, cutoff.YearCovered).CreateAlphalistDetail());

                        _model.ExportAlphalist(alphalists, cutoff.YearCovered, company);
                        _model.ExportAlphalistVerifier(employeePayrolls, cutoff.YearCovered, company);
                        _viewModel.SetAsFinishProgress();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Alphalist Export Error",
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
