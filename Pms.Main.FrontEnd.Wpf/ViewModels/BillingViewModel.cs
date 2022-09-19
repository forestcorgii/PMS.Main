using Pms.Adjustments.Domain;
using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Enums;
using Pms.Timesheets.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Main.FrontEnd.Wpf.Commands.Billings;
using System.ComponentModel;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class BillingViewModel : ViewModelBase
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

        private BillingModel _model;

        private string _adjustmentName;

        public string AdjustmentName
        {
            get => _adjustmentName; set
            {
                SetProperty(ref _adjustmentName, value);
            }
        }
        private IEnumerable<string> _adjustmentNames;
        public IEnumerable<string> AdjustmentNames { get => _adjustmentNames; set => SetProperty(ref _adjustmentNames, value); }



        public BillingViewModel(BillingModel model, MasterlistModel employeeModel)
        {
            _billings = new ObservableCollection<Billing>();
            Billings = new ObservableCollection<Billing>();

            _model = model;

            ExportBillings = new Export(this, _model);
            GenerateBillings = new Generate(this, _model, employeeModel);
            ListBillings = new Commands.Billings.Listing(this, _model);
            ListBillings.Execute(null);

            IsActive = true;
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
            Messenger.Register<BillingViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
            Messenger.Register<BillingViewModel, SelectedCutoffChangedMessage>(this, (r, m) => r.CutoffId = m.Value.CutoffId);
        }
    }
}
