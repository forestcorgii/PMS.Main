using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Exceptions;
using Pms.Masterlists.FrontEnd.Models;
using Pms.Masterlists.FrontEnd.Stores;
using Pms.Masterlists.FrontEnd.ViewModels;
using Pms.Masterlists.ServiceLayer.HRMS.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Masterlists.FrontEnd.Commands
{
    public class Download : IAsyncRelayCommand
    {
        private EmployeeListingVm _viewModel;
        private Models.Employees _model;
        private MasterlistStore _store;

        public Task? ExecutionTask { get; }

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Download(EmployeeListingVm viewModel, MasterlistStore store, Models.Employees model)
        {
            _viewModel = viewModel;
            _store = store;
            _model = model;

        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter) =>
            await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            string[] eeIds;
            if (parameter is not null && parameter is string[])
                eeIds = (string[])parameter;
            else
                eeIds = _store.Employees.Select(ee => ee.EEId).ToArray();

            _viewModel.SetProgress("Syncing Unknown Employees", eeIds.Length);

            try
            {
                foreach (string eeId in eeIds)
                {
                    try
                    {
                        IPersonalInformation employee;
                        IPersonalInformation employeeFoundOnServer = await _model.FindEmployeeAsync(eeId, _store.Site.ToString());
                        IPersonalInformation employeeFoundLocally = _model.FindEmployee(eeId);

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
                    catch (Exception ex) { MessageBoxes.ShowError(ex.Message, "Employee Sync Error"); }

                    _viewModel.ProgressValue++;
                }
            }
            catch (HttpRequestException)
            {
                _viewModel.StatusMessage = "HTTP Request failed, please check Your HRMS Configuration.";
            }
            _viewModel.SetAsFinishProgress();
            await _store.Reload();
        }




        public void NotifyCanExecuteChanged() { }
        public void Cancel() { }
    }
}
