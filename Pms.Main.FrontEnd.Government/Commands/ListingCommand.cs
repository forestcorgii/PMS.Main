using Microsoft.Toolkit.Mvvm.Input;
using Pms.Main.FrontEnd.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Government.Commands
{
    public class ListingCommand : IRelayCommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly IStore _store;

        private bool _canExecute;

        public ListingCommand(IStore store)
        {
            _store = store;
        }


        public bool CanExecute(object? parameter) => _canExecute;


        public async void Execute(object? parameter)
        {
            _canExecute = false;
            bool? doReload = (bool?)parameter;
            try
            {
                if (doReload.HasValue && doReload.Value == true)
                    await _store.Reload();
                else
                    await _store.Load();
            }
            catch
            {

            }

            _canExecute = true;
            NotifyCanExecuteChanged();
        }

        public void NotifyCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
