using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Common.States
{
    public class StoreBase : IStore
    {
        public Action Reloaded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task Load()
        {
            throw new NotImplementedException();
        }

        public Task Reload()
        {
            throw new NotImplementedException();
        }
    }
}
