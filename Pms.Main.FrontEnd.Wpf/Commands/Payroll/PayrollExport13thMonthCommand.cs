using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using Pms.Payrolls.ServiceLayer.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class PayrollExport13thMonthCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly PayrollStore _store;
        private readonly MainStore _mainStore;
        private readonly PayrollViewModel _viewModel;
        private readonly PayrollModel _model;

        private bool _canExecute { get; set; } = true;

        public PayrollExport13thMonthCommand(PayrollViewModel viewModel, PayrollModel model, PayrollStore store, MainStore mainStore)
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
                    Cutoff cutoff = new(_mainStore.Cutoff.CutoffId);
                    int yearCovered = cutoff.YearCovered;

                    _viewModel.SetProgress("Exporting 13th Month Report.", 1);
                    IEnumerable<Payroll> payrolls = _model.Get(yearCovered, BankType.LBP);
                    List<string> eeIds = payrolls.ExtractEEIds();

                    List<ThirteenthMonth> thirteenthMonths = new();
                    foreach (string eeId in eeIds)
                    {
                        IEnumerable<Payroll> eePayrolls = payrolls.Where(p => p.EEId == eeId);

                        double totalRegPay = eePayrolls.Sum(p => p.AdjustedRegPay());
                        double computed13Month = totalRegPay / 12;

                        thirteenthMonths.Add(new ThirteenthMonth()
                        {
                            EE = eePayrolls.First().EE,
                            EEId = eeId,
                            TotalRegPay = totalRegPay,
                            Amount = computed13Month
                        });
                    }

                    _model.Export13thMonth(thirteenthMonths, yearCovered, BankType.LBP);
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
