using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain.Enums;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedSiteChangedMessage : ValueChangedMessage<SiteChoices>
    {
        public SelectedSiteChangedMessage(SiteChoices value) : base(value) { }
    }

    public sealed class CurrentSiteRequestMessage : RequestMessage<SiteChoices> { }
}
