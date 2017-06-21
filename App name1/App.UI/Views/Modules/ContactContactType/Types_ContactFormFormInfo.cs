namespace UI.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Zebble;
    using Zebble.Framework;

    partial class Types_ContactFormFormInfo
    {
        public ContactContactType Item;

        public Types_ContactFormFormInfo()
        {
            Item = Nav.Param<ContactContactType>("Item")?.Clone() ?? new ContactContactType();
        }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            await InitializeComponents();
        }

        public override async Task OnInitialized()
        {
            await base.OnInitialized();

            LoadData();
        }

        internal void LoadData()
        {
            ContactType.Value = Item.ContactType;
        }

        internal void PopulateModel()
        {
            Item.ContactType = ContactType.GetValue<Domain.ContactType>();
        }
    }
}