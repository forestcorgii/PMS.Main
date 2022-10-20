using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.ServiceLayer.HRMS.Exceptions;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.MasterlistModule.FrontEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Utils;
using Pms.Masterlists.Domain.Entities.Employees;

namespace Pms.MasterlistModule.FrontEnd.Commands.Employees_
{
    public class Sync : IAsyncRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeDetailVm _viewModel;


        public Sync(EmployeeDetailVm viewModel, Employees model)
        {
            _model = model;
            _viewModel = viewModel;
        }

        public Task? ExecutionTask { get; }

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool executable = true;
        public bool CanExecute(object? parameter) =>
        executable;


        public async void Execute(object? parameter) =>
            await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            executable = false;
            NotifyCanExecuteChanged();

            try
            {
                try
                {
                    IHRMSInformation employeeFoundOnServer = await _model.SyncOneAsync(_viewModel.Employee.EEId, _viewModel.Employee.Site);
                    if (employeeFoundOnServer is not null)
                    {
                        _viewModel.Employee.LastName = employeeFoundOnServer.LastName;
                        _viewModel.Employee.FirstName = employeeFoundOnServer.FirstName;
                        _viewModel.Employee.MiddleName = employeeFoundOnServer.MiddleName;
                        _viewModel.Employee.JobCode = employeeFoundOnServer.JobCode;
                        _viewModel.Employee.Location = employeeFoundOnServer.Location;

                        _viewModel.RefreshProperties();
                    }
                }
                catch (Exception ex) { MessageBoxes.Error(ex.Message, "Employee Sync Error"); }
            }
            catch (HttpRequestException) { MessageBoxes.Error("HTTP Request failed, please check Your HRMS Configuration."); }

            executable = true;
            NotifyCanExecuteChanged();
        }



        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        public void Cancel() { }
    }
}
