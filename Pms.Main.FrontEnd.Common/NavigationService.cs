﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Pms.Main.FrontEnd.Common.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Common.Services
{
    public class NavigationService<TViewModel> where TViewModel : ObservableObject
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}