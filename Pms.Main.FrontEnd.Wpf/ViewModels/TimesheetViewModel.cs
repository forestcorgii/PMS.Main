using Pms.Masterlists.Domain;
using Pms.Main.FrontEnd.Wpf.Commands;
using Pms.Main.FrontEnd.Wpf.Models;
using Pms.Timesheets.BizLogic.Concrete;
using Pms.Timesheets.Domain;
using Pms.Timesheets.Domain.SupportTypes;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.Outputs;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Pms.Timesheets.ServiceLayer.TimeSystem.Services.Enums;
using Pms.Main.FrontEnd.Common.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Pms.Masterlists.Domain.Enums;
using Pms.Main.FrontEnd.Wpf.Commands.Timesheets;
using Pms.Main.FrontEnd.Common;

namespace Pms.Main.FrontEnd.Wpf.ViewModels
{
    public class TimesheetViewModel : ViewModelBase
    {
        #region Properties
        private DownloadOptions options = DownloadOptions.All;
        public DownloadOptions Options
        {
            get => options;
            set => SetProperty(ref options, value);
        }


        private IEnumerable<Timesheet> _timesheets;
        public IEnumerable<Timesheet> Timesheets
        {
            get => _timesheets;
            set => SetProperty(ref _timesheets, value);
        }
        #endregion

        public ICommand DownloadCommand { get; }
        public ICommand EvaluateCommand { get; }
        public ICommand ExportCommand { get; }


        public ICommand LoadTimesheets { get; }

        public TimesheetViewModel(TimesheetModel model)
        {
            LoadTimesheets = new Commands.Timesheets.Listing(this, model);

            DownloadCommand = new Download(this, model);
            EvaluateCommand = new Evaluation(this, model);
            ExportCommand = new Export(this, model);

            _timesheets = new ObservableCollection<Timesheet>();
            Timesheets = new ObservableCollection<Timesheet>();


            IsActive = true;
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((new string[] { nameof(Site), nameof(PayrollCode), nameof(Cutoff) }).Any(p => p == e.PropertyName))
                LoadTimesheets.Execute(null);

            base.OnPropertyChanged(e);
        }



        private SiteChoices site = SiteChoices.MANILA;
        public SiteChoices Site { get => site; set => SetProperty(ref site, value); }

        private PayrollCode payrollCode = new PayrollCode();
        public PayrollCode PayrollCode { get => payrollCode; set => SetProperty(ref payrollCode, value); }

        private Cutoff cutoff = new Cutoff();
        public Cutoff Cutoff { get => cutoff; set => SetProperty(ref cutoff, value); }

        protected override void OnActivated()
        {
            Messenger.Register<TimesheetViewModel, SelectedSiteChangedMessage>(this, (r, m) => r.Site = m.Value);
            Messenger.Register<TimesheetViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCode = m.Value);
            Messenger.Register<TimesheetViewModel, SelectedCutoffChangedMessage>(this, (r, m) => r.Cutoff = m.Value);
        }
    }
}

