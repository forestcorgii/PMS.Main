using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public abstract class StoreBase
    {
        protected abstract Lazy<Task> _initializeLoadLazy { get; set; }

        public abstract Task Load();

        protected abstract Task InitializeLoad();
    }
}
