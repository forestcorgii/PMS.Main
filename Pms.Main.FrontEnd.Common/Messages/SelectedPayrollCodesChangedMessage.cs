using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedPayrollCodesChangedMessage : ValueChangedMessage<string[]>
    {
        public SelectedPayrollCodesChangedMessage(string[] value) : base(value) { }
    }

    public sealed class CurrentPayrollCodesRequestMessage : RequestMessage<string[]> { }
}
