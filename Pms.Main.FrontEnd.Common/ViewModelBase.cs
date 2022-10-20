using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected double progressMaximum = 100;
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


        public void SetProgress(string processDescription, double maximum)
        {
            StatusMessage = processDescription;
            ProgressMaximum = maximum;
            ProgressValue = 0;
        }

        public void SetAsFinishProgress()
        {
            StatusMessage = "DONE";
            ProgressMaximum = 1;
            ProgressValue = 0;
        }


        public virtual void Dispose() { }

    }
}
