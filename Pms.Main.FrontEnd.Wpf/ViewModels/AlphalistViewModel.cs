using Pms.Main.FrontEnd.Common;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Commands.Payrolls;
using Pms.Main.FrontEnd.Wpf.Messages;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Payrolls.Domain;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
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
            Messenger.Register<AlphalistViewModel, SelectedCutoffChangedMessage>(this, (r, m) => r.Cutoff = new Cutoff(m.Value.CutoffId));
        }

    }
}
