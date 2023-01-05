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
    public class SyncAll : IAsyncRelayCommand
    {
        private readonly Employees Model;
        private readonly EmployeeListingVm ListingVm;


        public SyncAll(EmployeeListingVm listingVm, Employees model)
        {
            Model = model;
            ListingVm = listingVm;

            ListingVm.CanExecuteChanged += ListingVm_CanExecuteChanged; ;
        }

        public Task? ExecutionTask { get; }

        public bool CanBeCanceled => false;

        public bool IsCancellationRequested => false;

        public bool IsRunning => false;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public bool CanExecute(object? parameter) => ListingVm.Executable;


        public async void Execute(object? parameter) => await ExecuteAsync(parameter);


        public async Task ExecuteAsync(object? parameter)
        {
            string[] eeIds;
            if (parameter is not null && parameter is string[])
                eeIds = (string[])parameter;
            else
                eeIds = ListingVm.Employees.Select(ee => ee.EEId).ToArray();

            ListingVm.SetProgress("Syncing Unknown Employees", eeIds.Length);

            List<Exception> exceptions = new();
            try
            {
                foreach (string eeId in eeIds)
                {
                    try
                    {
                        Employee employee;
                        Employee employeeFoundOnServer = await Model.SyncOneAsync(eeId, ListingVm.Site.ToString());
                        Employee employeeFoundLocally = Model.FindEmployee(eeId);

                        if (employeeFoundOnServer is null && employeeFoundLocally is null)
                        {
                            employee = new Employee() { EEId = eeId, Active = false };
                            Model.Save(employee);
                        }
                        else if (employeeFoundOnServer is null && employeeFoundLocally is not null)
                        {
                            employeeFoundLocally.Active = false;
                            Model.Save(employeeFoundLocally);
                        }
                        else if (employeeFoundOnServer is not null)
                        {
                            employeeFoundOnServer.Active = true;
                            Model.Save(employeeFoundOnServer);
                        }
                    }
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        //if (!MessageBoxes.Inquire($"{ex.Message}. Do you want to proceed?", "Employee Sync Error"))
                        //    break;
                    }

                    if (!ListingVm.IncrementProgress())
                        break;
                }

                Model.ReportExceptions(exceptions, ListingVm.PayrollCode, $"{ListingVm.Site}-REGULAR");
            }
            catch (HttpRequestException) { MessageBoxes.Error("HTTP Request failed, please check Your HRMS Configuration."); }
            catch (Exception ex) { MessageBoxes.Error(ex.Message); }

            ListingVm.SetAsFinishProgress($"{exceptions.Count} error/s found.");

            ListingVm.SyncNewlyHired.Execute(DateTime.Now.AddDays(-20));
        }


        public void Cancel() { }

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
        private void ListingVm_CanExecuteChanged(object? sender, bool e) => NotifyCanExecuteChanged();

    }
}
