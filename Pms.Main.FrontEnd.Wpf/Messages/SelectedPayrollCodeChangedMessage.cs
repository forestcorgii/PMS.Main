using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Messages
{
    public sealed class SelectedPayrollCodeChangedMessage : ValueChangedMessage<PayrollCode>
    {
        public SelectedPayrollCodeChangedMessage(PayrollCode value) : base(value) { }
    }

    public sealed class CurrentPayrollCodeRequestMessage : RequestMessage<PayrollCode> { }
}
