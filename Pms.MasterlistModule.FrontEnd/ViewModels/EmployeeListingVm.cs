using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Main.FrontEnd.Common;
using Pms.MasterlistModule.FrontEnd;
using Pms.MasterlistModule.FrontEnd.Commands;
using Pms.MasterlistModule.FrontEnd.Models;
using Pms.Masterlists.Domain.Enums;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.MasterlistModule.FrontEnd.Commands.Masterlists;
using CommunityToolkit.Mvvm.Messaging;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class EmployeeListingVm : ViewModelBase
    {
        private string searchInput = string.Empty;
        public string SearchInput
        {
            get => searchInput;
            set => SetProperty(ref searchInput, value);
        }

        private bool includeArchived;
        public bool IncludeArchived
        {
            get => includeArchived;
            set => SetProperty(ref includeArchived, value);
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        private IEnumerable<Employee> _employees;
        public IEnumerable<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<BankChoices> BankTypes => new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());

        public ICommand LoadEmployeesCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand BankImportCommand { get; }
        public ICommand EEDataImportCommand { get; }
        public ICommand SaveCommand { get; }

        public EmployeeListingVm(Employees model)
        {
            DownloadCommand = new Download(this, model);
            BankImportCommand = new BankImport(this, model);
            EEDataImportCommand = new EEDataImport(this, model);
            SaveCommand = new Save(this, model);

            LoadEmployeesCommand = new Listing(this, model);
            LoadEmployeesCommand.Execute(null);

            _selectedEmployee = new();


            Site = WeakReferenceMessenger.Default.Send<CurrentSiteRequestMessage>();
            CompanyId = WeakReferenceMessenger.Default.Send<CurrentCompanyRequestMessage>().Response.CompanyId;
            PayrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;

            IsActive = true;
        }


        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(Site), nameof(CompanyId), nameof(PayrollCodeId), nameof(IncludeArchived), nameof(SearchInput) }).Any(p => p == e.PropertyName))
                LoadEmployeesCommand.Execute(null);

            base.OnPropertyChanged(e);
        }


        private SiteChoices site = SiteChoices.MANILA;
        public SiteChoices Site { get => site; set => SetProperty(ref site, value); }

        private string companyId = string.Empty;
        public string CompanyId { get => companyId; set => SetProperty(ref companyId, value); }

        private string payrollCodeId = string.Empty;
        public string PayrollCodeId { get => payrollCodeId; set => SetProperty(ref payrollCodeId, value); }

        protected override void OnActivated()
        {
            Messenger.Register<EmployeeListingVm, SelectedSiteChangedMessage>(this, (r, m) => r.Site = m.Value);
            Messenger.Register<EmployeeListingVm, SelectedCompanyChangedMessage>(this, (r, m) => r.CompanyId = m.Value.CompanyId);
            Messenger.Register<EmployeeListingVm, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
        }
    }

}
