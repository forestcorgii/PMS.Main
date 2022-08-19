using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class PayrollViewModel : ViewModelBase
    {
        private IEnumerable<Payroll> _payrolls;
        public IEnumerable<Payroll> Payrolls { get => _payrolls; set => SetProperty(ref _payrolls, value); }

        private PayrollStore _store;

        public ICommand PayrollListing { get; }
        public ICommand PayrollImport { get; }
        public ICommand PayrollExport { get; }


        public PayrollViewModel(PayrollStore store, MainStore mainStore, PayrollModel model)
        {
            _payrolls = new List<Payroll>();
            Payrolls = new List<Payroll>();

            _store = store;
            _store.Reloaded += PayrollsReloaded;

            PayrollListing = new ListingCommand(store);
            PayrollImport = new PayrollImportCommand(this, model, mainStore);
            PayrollExport = new PayrollExportCommand();

        }

        private void PayrollsReloaded()
        {
            Payrolls = _store.Payrolls;
        }
    }
}
