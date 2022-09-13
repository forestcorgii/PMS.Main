using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Main.FrontEnd.Wpf.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class AlphalistViewModel : ViewModelBase
    {
        public string BirDbfDirectory { get; set; }
        public string CompanyId { get; set; }
        
        public ObservableCollection<string> CompanyIds { get; set; }

        public ICommand SaveToBirProgram { get; set; }

        private PayrollStore _store;

        public AlphalistViewModel(PayrollModel model, PayrollStore store, MainStore mainStore)
        {
            _store = store;
            _store.Reloaded += PayrollsReloaded;

            SaveToBirProgram = new PayrollImportAlphalistCommand(this, model, store, mainStore);
            //CompanyIds = new ObservableCollection<string>(store.CompanyIds);
        }

        public override void Dispose()
        {
            _store.Reloaded -= PayrollsReloaded;
            base.Dispose();
        }

        private void PayrollsReloaded()
        {
            //CompanyIds = new ObservableCollection<string>(_store.CompanyIds);
        }
    }
}
