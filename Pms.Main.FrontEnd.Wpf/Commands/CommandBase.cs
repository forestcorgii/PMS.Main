using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Wpf.Stores;
using Pms.Main.FrontEnd.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Commands
{
    public class CommandBase : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;
         
        protected bool _canExecute;


        public bool CanExecute(object? parameter) => _canExecute;


        public void Execute(object? parameter) { }


        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());

        
    }
}
