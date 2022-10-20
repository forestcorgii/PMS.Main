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

        public Detail(Employees employees)
        {
            Employees = employees;
        }

        public void Execute(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            EmployeeDetailVm detailVm;
            if (parameter is Employee employee)
                detailVm = new(employee, Employees);
            else detailVm = new(new(), Employees);

            EmployeeDetailView detailView = new() { DataContext = detailVm };

            detailVm.OnRequestClose += (s, e) => detailView.Close();
            detailView.ShowDialog();

            executable = true;
            NotifyCanExecuteChanged();
        }

        protected bool executable = true;


        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => executable;

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
