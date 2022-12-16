﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Common;
using Pms.PayrollModule.FrontEnd.Commands;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.PayrollModule.FrontEnd.Models;
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

namespace Pms.PayrollModule.FrontEnd.ViewModels
{
    public class PayrollViewModel : ViewModelBase
    {
        private IEnumerable<Payroll> payrolls;
        public IEnumerable<Payroll> Payrolls { get => payrolls; set => SetProperty(ref payrolls, value); }

        private int chkCount;
        public int ChkCount { get => chkCount; set => SetProperty(ref chkCount, value); }
        private double chkTotal;
        public double ChkTotal { get => chkTotal; set => SetProperty(ref chkTotal, value); }

        private int lbpCount;
        public int LbpCount { get => lbpCount; set => SetProperty(ref lbpCount, value); }
        private double lbpTotal;
        public double LbpTotal { get => lbpTotal; set => SetProperty(ref lbpTotal, value); }

        private int cbcCount;
        public int CbcCount { get => cbcCount; set => SetProperty(ref cbcCount, value); }
        private double cbcTotal;
        public double CbcTotal { get => cbcTotal; set => SetProperty(ref cbcTotal, value); }

        private int mtacCount;
        public int MtacCount { get => mtacCount; set => SetProperty(ref mtacCount, value); }
        private double mtacTotal;
        public double MtacTotal { get => mtacTotal; set => SetProperty(ref mtacTotal, value); }

        private int mpaloCount;
        public int MpaloCount { get => mpaloCount; set => SetProperty(ref mpaloCount, value); }
        private double mpaloTotal;
        public double MpaloTotal { get => mpaloTotal; set => SetProperty(ref mpaloTotal, value); }


        private int unknownEECount;
        public int UnknownEECount { get => unknownEECount; set => SetProperty(ref unknownEECount, value); }
        private double unknownEETotal;
        public double UnknownEETotal { get => unknownEETotal; set => SetProperty(ref unknownEETotal, value); }

        private int grandCount;
        public int GrandCount { get => grandCount; set => SetProperty(ref grandCount, value); }
        private double grandTotal;
        public double GrandTotal { get => grandTotal; set => SetProperty(ref grandTotal, value); }


        #region Commands
        public ICommand PayrollListing { get; }
        public ICommand PayrollImport { get; }
        public ICommand PayrollBankReportExport { get; }
        public ICommand PayrollAlphalistExport { get; }
        public ICommand PayrollMacroExport { get; }
        public ICommand Payroll13thMonthExport { get; }
        public IAsyncRelayCommand EmployeeDownloadCommand { get; }
        #endregion


        public PayrollViewModel(Models.Payrolls model)
        {
            PayrollListing = new Listing(this, model);

            PayrollImport = new ImportPayrollRegister(this, model);
            PayrollBankReportExport = new ExportBankReport(this, model);
            PayrollAlphalistExport = new ExportAlphalist(this, model);
            PayrollMacroExport = new ExportMacro(this, model);
            Payroll13thMonthExport = new Export13thMonth(this, model);


            company = WeakReferenceMessenger.Default.Send<CurrentCompanyRequestMessage>();
            payrollCode = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>();

            string cutoffId = WeakReferenceMessenger.Default.Send<CurrentCutoffIdRequestMessage>();
            cutoff = new Cutoff(cutoffId);

            IsActive = true;
            
            PayrollListing.Execute(null);
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
