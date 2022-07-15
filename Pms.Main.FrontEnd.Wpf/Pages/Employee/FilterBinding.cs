using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf
{
    public class FilterBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _filter { get; set; }
        public string Filter {
            get => _filter;
            set {
                _filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
            }
        }
    }
}
