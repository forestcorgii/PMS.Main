using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Main.FrontEnd.Common
{
    public class ViewModelBase : ObservableRecipient
    {
        protected double progressValue = 0;
        public double ProgressValue
        {
            get => progressValue;
            set => SetProperty(ref progressValue, value);
        }

        protected double progressMaximum = 1;
        public double ProgressMaximum
        {
            get => progressMaximum;
            set => SetProperty(ref progressMaximum, value);
        }

        protected string statusMessage = "";
        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public ViewModelBase() => Cancel = new RelayCommand(DoCancel);


        private bool PendingCancel { get; set; }
        public ICommand Cancel { get; }
        public void DoCancel()
        {
            if (MessageBoxes.Inquire("Are you sure you want to cancel?"))
            {
                PendingCancel = true;
                StatusMessage = "Cancellation Pending...";
            }
        }
        public bool IncrementProgress()
        {
            ProgressValue++;
            return !PendingCancel;
        }


        public void SetProgress(string progressDescription, double maximum)
        {
            StatusMessage = progressDescription;
            ProgressMaximum = maximum;
            ProgressValue = 0;
            NotifyCanExecuteChanged(false);
        }

        public void SetAsFinishProgress(string progressDescription = "DONE")
        {
            StatusMessage = progressDescription;
            ProgressMaximum = 1;
            ProgressValue = 0;
            NotifyCanExecuteChanged(true);
        }







        public event EventHandler<bool>? CanExecuteChanged;
        public bool Executable { get; set; } = true;
        public void NotifyCanExecuteChanged(bool executable)
        {
            PendingCancel = false;
            Executable = executable;
            CanExecuteChanged?.Invoke(this, Executable);
        }




        public virtual void Dispose() { }

    }
}
