using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedCutoffIdChangedMessage : ValueChangedMessage<string>
    {
        public SelectedCutoffIdChangedMessage(string value) : base(value) { }
    }

    public sealed class CurrentCutoffIdRequestMessage : RequestMessage<string> { }
}
