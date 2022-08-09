using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Stores
{
    public class TimesheetStore : StoreBase
    {
        protected override Lazy<Task> _initializeLoadLazy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override Task Load()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeLoad()
        {
            throw new NotImplementedException();
        }
    }
}
