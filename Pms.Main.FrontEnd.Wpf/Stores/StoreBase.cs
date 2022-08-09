using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class StoreBase
    {
        protected Lazy<Task> _initializeLoadLazy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task Load()
        {
            throw new NotImplementedException();
        }

        protected Task InitializeLoad()
        {
            throw new NotImplementedException();
        }
    }
}
