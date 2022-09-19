using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Commands.Payrolls;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Masterlists.Domain;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Pms.Payrolls.Domain.Enums;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class PayrollViewModel : ViewModelBase
    {
        private IEnumerable<Payroll> payrolls;
        public IEnumerable<Payroll> Payrolls { get => payrolls; set => SetProperty(ref payrolls, value); }

        private int chkCount;
        public int ChkCount { get => chkCount; set => SetProperty(ref chkCount, value); }

        private int lbpCount;
        public int LbpCount { get => lbpCount; set => SetProperty(ref lbpCount, value); }

        private int cbcCount;
        public int CbcCount { get => cbcCount; set => SetProperty(ref cbcCount, value); }

        private int mtacCount;
        public int MtacCount { get => mtacCount; set => SetProperty(ref mtacCount, value); }

        private int mpaloCount;
        public int MpaloCount { get => mpaloCount; set => SetProperty(ref mpaloCount, value); }

        private int unknownEECount;
        public int UnknownEECount { get => unknownEECount; set => SetProperty(ref unknownEECount, value); }



        #region Commands
        public ICommand PayrollListing { get; }
        public ICommand PayrollImport { get; }
        public ICommand PayrollBankReportExport { get; }
        public ICommand PayrollAlphalistExport { get; }
        public IAsyncRelayCommand EmployeeDownloadCommand { get; }
        #endregion


        public PayrollViewModel(PayrollModel model)
        {
            PayrollListing = new Commands.Payrolls.Listing(this, model);
            PayrollListing.Execute(null);

            PayrollImport = new ImportPayrollRegister(this, model);
            PayrollBankReportExport = new ExportBankReport(this, model);
            PayrollAlphalistExport = new ExportAlphalist(this, model);

            IsActive = true;
        }



        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(Company), nameof(PayrollCode), nameof(Cutoff) }).Any(p => p == e.PropertyName))
                PayrollListing.Execute(null);

            base.OnPropertyChanged(e);
        }




        private Company company = new Company();
        public Company Company { get => company; set => SetProperty(ref company, value); }

        private PayrollCode payrollCode = new PayrollCode();
        public PayrollCode PayrollCode { get => payrollCode; set => SetProperty(ref payrollCode, value); }

        private Cutoff cutoff = new Cutoff();
        public Cutoff Cutoff { get => cutoff; set => SetProperty(ref cutoff, value); }

        protected override void OnActivated()
        {
            Messenger.Register<PayrollViewModel, SelectedCompanyChangedMessage>(this, (r, m) => r.Company = m.Value);
            Messenger.Register<PayrollViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCode = m.Value);
            Messenger.Register<PayrollViewModel, SelectedCutoffIdChangedMessage>(this, (r, m) => r.Cutoff = new Cutoff(m.Value));
        }
    }
}
