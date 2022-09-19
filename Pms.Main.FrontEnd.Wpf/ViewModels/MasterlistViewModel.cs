using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pms.Masterlists.Domain.Enums;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Wpf.Commands.Masterlists;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class MasterlistViewModel : ViewModelBase
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

        public MasterlistViewModel(MasterlistModel model)
        {
            DownloadCommand = new Download(this, model);
            BankImportCommand = new BankImport(this, model);
            EEDataImportCommand = new EEDataImport(this, model);
            SaveCommand = new Save(this, model);
             
            LoadEmployeesCommand = new Commands.Masterlists.Listing(this, model);
            LoadEmployeesCommand.Execute(null);

            _selectedEmployee = new();

            IsActive = true;
        }

        public override void Dispose()
        {
            IsActive = false;

            Messenger.Unregister<SelectedSiteChangedMessage>(this);
            Messenger.Unregister<SelectedCompanyChangedMessage>(this);
            Messenger.Unregister<SelectedPayrollCodeChangedMessage>(this);

            base.Dispose();
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
            Messenger.Register<MasterlistViewModel, SelectedSiteChangedMessage>(this, (r, m) => r.Site = m.Value);
            Messenger.Register<MasterlistViewModel, SelectedCompanyChangedMessage>(this, (r, m) => r.CompanyId = m.Value.CompanyId);
            Messenger.Register<MasterlistViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
        }
    }
}
