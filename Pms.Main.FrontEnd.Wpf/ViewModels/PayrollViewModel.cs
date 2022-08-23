using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class PayrollViewModel : ViewModelBase
    {
        private ObservableCollection<Payroll> _payrolls;
        public ObservableCollection<Payroll> Payrolls { get => _payrolls; set => SetProperty(ref _payrolls, value); }

        private BankType _bankType = BankType.LBP;
        public BankType BankType { get => _bankType; set => SetProperty(ref _bankType, value); }
        public ObservableCollection<BankType> BankTypes => new ObservableCollection<BankType>(Enum.GetValues(typeof(BankType)).Cast<BankType>());

        private PayrollStore _store;

        public ICommand PayrollListing { get; }
        public ICommand PayrollImport { get; }
        public ICommand PayrollLBPExport { get; }
        public ICommand Payroll13thMonthExport { get; }
        public IAsyncRelayCommand EmployeeDownloadCommand { get; }


        public PayrollViewModel(PayrollStore store, MainStore mainStore, PayrollModel model,EmployeeStore employeeStore,EmployeeModel employeeModel)
        {
            _store = store;
            _store.Reloaded += PayrollsReloaded;

            _payrolls = new ObservableCollection<Payroll>();
            Payrolls = new ObservableCollection<Payroll>(_store.Payrolls);

            PayrollListing = new ListingCommand(store);
            PayrollImport = new PayrollImportCommand(this, model, mainStore);
            PayrollLBPExport = new PayrollExportLBPCommand(this, model, store, mainStore);
            Payroll13thMonthExport = new PayrollExport13thMonthCommand(this, model, store, mainStore);
           
            EmployeeDownloadCommand = new EmployeeDownloadCommand(this, mainStore, employeeStore, employeeModel);

            PayrollListing.Execute(null);
        }

        public override void Dispose()
        {
            _store.Reloaded -= PayrollsReloaded;
            base.Dispose();
        }

        private void PayrollsReloaded()
        {
            Payrolls = new ObservableCollection<Payroll>(_store.Payrolls);
        }
    }
}
