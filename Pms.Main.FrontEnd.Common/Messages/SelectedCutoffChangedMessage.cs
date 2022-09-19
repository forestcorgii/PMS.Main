using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Timesheets.Domain.SupportTypes;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedCutoffChangedMessage : ValueChangedMessage<Cutoff>
    {
        public SelectedCutoffChangedMessage(Cutoff value) : base(value) { }
    }

    public sealed class CurrentCutoffRequestMessage : RequestMessage<Cutoff> { }
}
