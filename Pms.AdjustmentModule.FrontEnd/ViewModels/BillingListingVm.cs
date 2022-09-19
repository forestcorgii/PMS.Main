using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using Pms.AdjustmentModule.FrontEnd.Models;
using Pms.AdjustmentModule.FrontEnd.Commands;

namespace Pms.AdjustmentModule.FrontEnd.ViewModels
{
    public class BillingListingVm : ViewModelBase
    {

        private IEnumerable<Billing> _billings;
        public IEnumerable<Billing> Billings
        {
            get => _billings;
            set => SetProperty(ref _billings, value);
        }

        public ICommand GenerateBillings { get; }
        public ICommand ListBillings { get; }
        public ICommand ExportBillings { get; }


        private string _adjustmentName = string.Empty;
        public string AdjustmentName
        {
            get => _adjustmentName; set
            {
                SetProperty(ref _adjustmentName, value);
            }
        }
        private IEnumerable<string> _adjustmentNames=new List<string>();
        public IEnumerable<string> AdjustmentNames { get => _adjustmentNames; set => SetProperty(ref _adjustmentNames, value); }



        public BillingListingVm(Billings model)
        {
            _billings = new ObservableCollection<Billing>();
            Billings = new ObservableCollection<Billing>();

            ExportBillings = new Export(this, model);
            GenerateBillings = new Generate(this, model);
            ListBillings = new Listing(this, model);

            IsActive = true;

            CutoffId = WeakReferenceMessenger.Default.Send<CurrentCutoffIdRequestMessage>();
            PayrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;

            ListBillings.Execute(null);
        }



        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(PayrollCodeId), nameof(CutoffId) }).Any(p => p == e.PropertyName))
                ListBillings.Execute(null);

            base.OnPropertyChanged(e);
        }




        private string payrollCodeId = string.Empty;
        public string PayrollCodeId { get => payrollCodeId; set => SetProperty(ref payrollCodeId, value); }

        private string cutoffId = string.Empty;
        public string CutoffId { get => cutoffId; set => SetProperty(ref cutoffId, value); }

        protected override void OnActivated()
        {
            Messenger.Register<BillingListingVm, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
            Messenger.Register<BillingListingVm, SelectedCutoffIdChangedMessage>(this, (r, m) => r.CutoffId = m.Value);
        }
    }
}
