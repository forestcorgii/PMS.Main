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
        public ObservableCollection<Payroll> Payrolls { get => _store.Payrolls; set => SetProperty(ref _store.Payrolls, value); }

        private int chkCount;
        public int ChkCount { get => chkCount; set => SetProperty(ref chkCount, value); }

        private int lbpCount;
        public int LbpCount { get => lbpCount; set => SetProperty(ref lbpCount, value); }

        private int cbcCount;
        public int CbcCount { get => cbcCount; set => SetProperty(ref cbcCount, value); }

        private int mtacCount;
        public int MtacCount { get => mtacCount; set => SetProperty(ref mtacCount, value); }

        private int mpaloCount;
        public int MpaloCount { get => mpaloCount; set => SetProperty(ref mpaloCount, value); }

        private int unknownEECount;
        public int UnknownEECount { get => unknownEECount; set => SetProperty(ref unknownEECount, value); }


 
        #region Commands
        public ICommand PayrollListing { get; }
        public ICommand PayrollImport { get; }
        public ICommand PayrollBankReportExport { get; }
        public ICommand PayrollAlphalistExport { get; }
        public IAsyncRelayCommand EmployeeDownloadCommand { get; }
        #endregion

        private PayrollStore _store;

        public PayrollViewModel(PayrollStore store, MainStore mainStore, PayrollModel model, MasterlistStore employeeStore, MasterlistModel employeeModel)
        {
            _store = store;
            _store.Reloaded += PayrollsReloaded;

            PayrollListing = new ListingCommand(store);
            PayrollListing.Execute(null);

            PayrollImport = new PayrollImportCommand(this, model, mainStore);
            PayrollBankReportExport = new PayrollExportBankReportCommand(this, model, store, mainStore);
            PayrollAlphalistExport = new PayrollExportAlphalistCommand(this, model, store, mainStore);

            EmployeeDownloadCommand = new Download(this, mainStore, employeeStore, employeeModel);

            Payrolls = new ObservableCollection<Payroll>(_store.Payrolls);
        }

        public override void Dispose()
        {
            _store.Reloaded -= PayrollsReloaded;
            base.Dispose();
        }

        private void PayrollsReloaded()
        {
            Payrolls = new ObservableCollection<Payroll>(_store.Payrolls);
            ChkCount = Payrolls.Count(p => p.EE.Bank == BankChoices.CHK);
            LbpCount = Payrolls.Count(p => p.EE.Bank == BankChoices.LBP);
            CbcCount = Payrolls.Count(p => p.EE.Bank == BankChoices.CBC);
            MtacCount = Payrolls.Count(p => p.EE.Bank == BankChoices.MTAC);
            MpaloCount = Payrolls.Count(p => p.EE.Bank == BankChoices.MPALO);
            UnknownEECount = Payrolls.Count(p => p.EE is null || p.EE.FirstName == string.Empty);
        }
    }
}
