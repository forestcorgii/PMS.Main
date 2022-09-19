using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Common.Messages;
using Pms.PayrollModule.FrontEnd.Commands;
using Pms.PayrollModule.FrontEnd.Models;
using Pms.Masterlists.Domain;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Payrolls.Domain;

namespace Pms.PayrollModule.FrontEnd.ViewModels
{
    public class AlphalistViewModel : ViewModelBase
    {
        public string BirDbfDirectory { get; set; } = "";
        public string CompanyId { get; set; } = "";

        public ObservableCollection<string> CompanyIds { get; set; }

        public ICommand SaveToBirProgram { get; set; }

        public AlphalistViewModel(PayrollModel model)
        {
            SaveToBirProgram = new ImportAlphalist(this, model);

            IsActive = true;
        }




        private Company company = new Company();
        public Company Company { get => company; set => SetProperty(ref company, value); }

        private string payrollCodeId = string.Empty;
        public string PayrollCodeId { get => payrollCodeId; set => SetProperty(ref payrollCodeId, value); }

        private Cutoff cutoff = new Cutoff();
        public Cutoff Cutoff { get => cutoff; set => SetProperty(ref cutoff, value); }

        protected override void OnActivated()
        {
            Messenger.Register<AlphalistViewModel, SelectedCompanyChangedMessage>(this, (r, m) => r.Company = m.Value);
            Messenger.Register<AlphalistViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
            Messenger.Register<AlphalistViewModel, SelectedCutoffIdChangedMessage>(this, (r, m) => r.Cutoff = new Cutoff(m.Value));
        }

    }
}
