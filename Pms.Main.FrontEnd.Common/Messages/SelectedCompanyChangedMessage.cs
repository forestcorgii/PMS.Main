using CommunityToolkit.Mvvm.Messaging.Messages;
using Pms.Masterlists.Domain;

namespace Pms.Main.FrontEnd.Common.Messages
{
    public sealed class SelectedCompanyChangedMessage : ValueChangedMessage<Company>
    {
        public SelectedCompanyChangedMessage(Company value) : base(value) { }
    }

    public sealed class CurrentCompanyRequestMessage : RequestMessage<Company> { }
}
