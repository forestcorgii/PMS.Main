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

namespace Pms.MasterlistModule.FrontEnd.Commands.Masterlists
{
    public class Download : IAsyncRelayCommand
    {
        private readonly Employees _model;
        private readonly EmployeeListingVm _viewModel;


        public Download(EmployeeListingVm viewModel, Employees model)
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
            string[] eeIds;
            if (parameter is not null && parameter is string[])
                eeIds = (string[])parameter;
            else
                eeIds = _viewModel.Employees.Select(ee => ee.EEId).ToArray();

            _viewModel.SetProgress("Syncing Unknown Employees", eeIds.Length);

            try
            {
                foreach (string eeId in eeIds)
                {
                    try
                    {
                        IActive employee;
                        IActive employeeFoundOnServer = await _model.FindEmployeeAsync(eeId, _viewModel.Site.ToString());
                        IActive employeeFoundLocally = _model.FindEmployee(eeId);

                        if (employeeFoundOnServer is null && employeeFoundLocally is null)
                        {
                            employee = new Employee() { EEId = eeId, Active = false };
                            _model.Save(employee);
                        }
                        else if (employeeFoundOnServer is null && employeeFoundLocally is not null)
                        {
                            employeeFoundLocally.Active = false;
                            _model.Save(employeeFoundLocally);
                        }
                        else if (employeeFoundOnServer is not null)
                        {
                            employeeFoundOnServer.Active = true;
                            _model.Save(employeeFoundOnServer);
                        }
                    }
                    catch (Exception ex) { MessageBoxes.Error(ex.Message, "Employee Sync Error"); }

                    _viewModel.ProgressValue++;
                }
            }
            catch (HttpRequestException) { MessageBoxes.Error("HTTP Request failed, please check Your HRMS Configuration."); }
            _viewModel.SetAsFinishProgress();

            executable = true;
        }




        public void NotifyCanExecuteChanged() { }
        public void Cancel() { }
    }
}
