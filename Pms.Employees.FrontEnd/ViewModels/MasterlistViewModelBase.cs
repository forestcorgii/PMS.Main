using Microsoft.Toolkit.Mvvm.ComponentModel;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.FrontEnd.Stores;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.Commands;

namespace Pms.Masterlists.FrontEnd.ViewModels
{
    public class MasterlistViewModelBase : ViewModelBase
    {
        private MasterlistStore _store { get; set; }


        //public ICommand DownloadCommand { get; }
        //public ICommand BankImportCommand { get; }
        //public ICommand EEDataImportCommand { get; }

        public MasterlistViewModelBase(MasterlistStore employeeStore, Models.Employees employeeModel)
        {
            _store = employeeStore;
            _store.Reloaded += _cutoffStore_EmployeesReloaded;

        }

        public override void Dispose()
        {
            _store.Reloaded -= _cutoffStore_EmployeesReloaded;
            base.Dispose();
        }

        private void _cutoffStore_EmployeesReloaded()
        {
        }


    }
}
