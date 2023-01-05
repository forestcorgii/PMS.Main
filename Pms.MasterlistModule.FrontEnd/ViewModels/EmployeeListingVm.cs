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
using Pms.MasterlistModule.FrontEnd.Commands.Employees_;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Masterlists.Domain.Entities.Employees;
using CommunityToolkit.Mvvm.Input;

namespace Pms.MasterlistModule.FrontEnd.ViewModels
{
    public class EmployeeListingVm : ViewModelBase
    {
        private int activeEECount;
        public int ActiveEECount { get => activeEECount; set => SetProperty(ref activeEECount, value); }

        private int nonActiveEECount;
        public int NonActiveEECount { get => nonActiveEECount; set => SetProperty(ref nonActiveEECount, value); }


        private string searchInput = string.Empty;
        public string SearchInput
        {
            get => searchInput;
            set => SetProperty(ref searchInput, value);
        }

        private bool hideArchived;
        public bool HideArchived
        {
            get => hideArchived;
            set => SetProperty(ref hideArchived, value);
        }

        private IEnumerable<Employee> _employees;
        public IEnumerable<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }


        public ICommand LoadEmployees { get; }

        public ICommand SyncAll { get; }
        public ICommand SyncNewlyHired { get; }
        public ICommand SyncResigned { get; }

        public ICommand BankImport { get; }
        public ICommand EEDataImport { get; }
        public ICommand MasterFileImport { get; }
        public ICommand AllEEExport { get; }
        public ICommand NoTinEEExport { get; }

        public ICommand OpenPayrollCodeView { get; }

        public ICommand CheckDetail { get; }

        public EmployeeListingVm(Employees employees, PayrollCodes payrollCodes, Companies companies)
        {
            BankImport = new BankImport(this, employees);
            EEDataImport = new EEDataImport(this, employees);
            MasterFileImport = new MasterFileImport(this, employees);
            AllEEExport = new MasterlistExport(this, employees);
            NoTinEEExport = new UnknownTin(this, employees);

            CheckDetail = new Detail(this, employees);

            SyncNewlyHired = new SyncNewlyHired(this, employees);
            SyncResigned = new SyncResigned(this, employees);
            SyncAll  = new SyncAll(this, employees);


            OpenPayrollCodeView = new Commands.Payroll_Codes.OpenView(this, payrollCodes, companies);

            LoadEmployees = new Listing(this, employees);



            site = WeakReferenceMessenger.Default.Send<CurrentSiteRequestMessage>();
            companyId = WeakReferenceMessenger.Default.Send<CurrentCompanyRequestMessage>().Response.CompanyId;
            payrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;
            payrollCode = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response;

            IsActive = true;
            LoadEmployees.Execute(null);
        }


        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(Site), nameof(CompanyId), nameof(PayrollCodeId), nameof(HideArchived), nameof(SearchInput) }).Any(p => p == e.PropertyName))
                LoadEmployees.Execute(null);

            base.OnPropertyChanged(e);
        }


        private SiteChoices site = SiteChoices.MANILA;
        public SiteChoices Site { get => site; set => SetProperty(ref site, value); }

        private string companyId = string.Empty;
        public string CompanyId { get => companyId; set => SetProperty(ref companyId, value); }

        private string payrollCodeId = string.Empty;
        public string PayrollCodeId { get => payrollCodeId; set => SetProperty(ref payrollCodeId, value); }

        private PayrollCode payrollCode;
        public PayrollCode PayrollCode { get => payrollCode; set => SetProperty(ref payrollCode, value); }

        protected override void OnActivated()
        {
            Messenger.Register<EmployeeListingVm, SelectedSiteChangedMessage>(this, (r, m) => r.Site = m.Value);
            Messenger.Register<EmployeeListingVm, SelectedCompanyChangedMessage>(this, (r, m) => r.CompanyId = m.Value.CompanyId);
            Messenger.Register<EmployeeListingVm, SelectedPayrollCodeChangedMessage>(this, (r, m) =>
            {
                r.PayrollCode = m.Value;
                r.PayrollCodeId = m.Value.PayrollCodeId;
            });
        }
    }

}
