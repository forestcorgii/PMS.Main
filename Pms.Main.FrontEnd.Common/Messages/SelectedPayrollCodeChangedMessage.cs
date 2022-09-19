using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedPayrollCodeChangedMessage : ValueChangedMessage<PayrollCode>
    {
        public SelectedPayrollCodeChangedMessage(PayrollCode value) : base(value) { }
    }

    public sealed class CurrentPayrollCodeRequestMessage : RequestMessage<PayrollCode> { }
}
