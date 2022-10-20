using Pms.AdjustmentModule.FrontEnd.Commands.Billing_Records;
using Pms.Adjustments.Domain.Enums;
using Pms.Adjustments.Domain.Models;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Pms.AdjustmentModule.FrontEnd.ViewModels.Billing_Records
{
    public class BillingRecordListingVm : ViewModelBase
    {

        private IEnumerable<BillingRecord> _billingRecords;
        public IEnumerable<BillingRecord> BillingRecords
        {
            get => _billingRecords;
            set => SetProperty(ref _billingRecords, value);
        }

        public ICommand ListBillings { get; }
        public ICommand Detail { get; }
        public ICommand Import { get; }

        private AdjustmentTypes _adjustmentName;
        public AdjustmentTypes AdjustmentName
        {
            get => _adjustmentName;
            set
            {
                SetProperty(ref _adjustmentName, value);
            }
        }

        public BillingRecordListingVm(Models.BillingRecords billingRecords, Models.Employees employees)
        {
            payrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;
            string cutoffId = WeakReferenceMessenger.Default.Send<CurrentCutoffIdRequestMessage>();
            if (!string.IsNullOrEmpty(cutoffId))
                cutoff = new Cutoff(cutoffId);

            IsActive = true;


            Detail = new Detail(billingRecords, employees);
            Import = new Import(this, billingRecords);

            ListBillings = new Listing(this, billingRecords);
            ListBillings.Execute(null);
        }


        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(PayrollCodeId), nameof(Cutoff) }).Any(p => p == e.PropertyName))
                ListBillings.Execute(null);

            base.OnPropertyChanged(e);
        }

        private string payrollCodeId = string.Empty;
        public string PayrollCodeId { get => payrollCodeId; set => SetProperty(ref payrollCodeId, value); }

        private Cutoff cutoff = new Cutoff();
        public Cutoff Cutoff { get => cutoff; set => SetProperty(ref cutoff, value); }

        protected override void OnActivated()
        {
            Messenger.Register<BillingRecordListingVm, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
            Messenger.Register<BillingRecordListingVm, SelectedCutoffIdChangedMessage>(this, (r, m) => r.Cutoff = new Cutoff(m.Value));
        }
    }
}
