using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain;
using Pms.Masterlists.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Main.FrontEnd.Wpf.Messages
{
    public sealed class SelectedSiteChangedMessage : ValueChangedMessage<SiteChoices>
    {
        public SelectedSiteChangedMessage(SiteChoices value) : base(value) { }
    }

    public sealed class CurrentSiteRequestMessage : RequestMessage<SiteChoices> { }
}
