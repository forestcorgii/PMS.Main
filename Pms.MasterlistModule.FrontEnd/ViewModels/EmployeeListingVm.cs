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
using Pms.Masterlists.Domain.Entities.Employees;

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

        private IEnumerable<Employee> _employees;
        public IEnumerable<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }


        public ICommand LoadEmployees { get; }
        public ICommand Download { get; }

        public ICommand BankImport { get; }
        public ICommand EEDataImport { get; }
        public ICommand MasterFileImport { get; }
        
        public ICommand CheckDetail { get; }

        public EmployeeListingVm(Employees model)
        {
            Download = new Download(this, model);
            BankImport = new BankImport(this, model);
            EEDataImport = new EEDataImport(this, model);
            MasterFileImport = new MasterFileImport(this, model);
            CheckDetail = new ViewEmployeeDetail(model);

            LoadEmployees = new Listing(this, model);
            LoadEmployees.Execute(null);


            Site = WeakReferenceMessenger.Default.Send<CurrentSiteRequestMessage>();
            CompanyId = WeakReferenceMessenger.Default.Send<CurrentCompanyRequestMessage>().Response.CompanyId;
            PayrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;

            IsActive = true;
        }


        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(Site), nameof(CompanyId), nameof(PayrollCodeId), nameof(IncludeArchived), nameof(SearchInput) }).Any(p => p == e.PropertyName))
                LoadEmployees.Execute(null);

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
