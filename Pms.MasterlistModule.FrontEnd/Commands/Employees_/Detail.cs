using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using Pms.MasterlistModule.FrontEnd.Views;
using Pms.Masterlists.Domain.Entities.Employees;
using Pms.Masterlists.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
{
    public class Detail : IRelayCommand
    {
        private readonly Employees Employees;
        private EmployeeListingVm ListingVm;

        public Detail(EmployeeListingVm listingVm, Employees employees)
        {
            Employees = employees;
            ListingVm = listingVm;
            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged;
        }

        public void Execute(object? parameter)
        {
            EmployeeDetailVm detailVm;
            if (parameter is Employee employee)
                detailVm = new(employee, Employees);
            else detailVm = new(new(), Employees);

            EmployeeDetailView detailView = new() { DataContext = detailVm };

            detailVm.OnRequestClose += (s, e) => detailView.Close();
            detailView.ShowDialog();
        }


        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => ListingVm.Executable;
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();
        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
