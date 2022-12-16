using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Pms.Masterlists.Domain.Entities.Employees;
using Pms.MasterlistModule.FrontEnd.Views;

namespace Pms.MasterlistModule.FrontEnd.Commands.Payroll_Codes
{
    public class OpenView : IRelayCommand
    {
        private readonly PayrollCodes PayrollCodes;
        private readonly Companies Companies;

        private EmployeeListingVm ListingVm;

        public OpenView(EmployeeListingVm listingVm, PayrollCodes model, Companies companies)
        {
            PayrollCodes = model;
            Companies = companies;
            ListingVm = listingVm;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public void Execute(object? parameter)
        {
            ListingVm.SetProgress("", 1);
            try
            {
                PayrollCodeDetailVm vm = new(PayrollCodes, Companies);
                PayrollCodeDetailView v = new() { DataContext = vm };

                v.ShowDialog();
            }
            catch (Exception ex) { MessageBoxes.Error(ex.Message, ""); }

            ListingVm.SetAsFinishProgress();
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
