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

    partial class ContactForm
    {
        public Contact Item;

        public ContactForm()
        {
            Item = Nav.Param<Contact>("Item")?.Clone() ?? new Contact();
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

        public async Task SaveButtonTapped()
        {
            try
            {
                if (!await (SaveInDatabase())) return;
            }
            catch (Exception ex)
            {
                await Alert.Show(ex.Message);
                return;
            }

            await Nav.Back();
        }

        internal void LoadData()
        {
            Name.Value = Item.Name;
            Email.Value = Item.Email;
            Number.Value = Item.Number;
            Tel.Value = Item.Tel;
            Notes.Value = Item.Notes;
            DateOfBirth.Value = Item.DateOfBirth;
            TimeOfBirth.Value = Item.TimeOfBirth;
            Type.Value = Item.Type;
            Types.Value = Item.Types.Select(x => x.ContactType);
            Photo.Value = Item.Photo;
            IsNice.Value = Item.IsNice;
        }

        internal void PopulateModel()
        {
            Item.Name = Name.GetValue<string>();
            Item.Email = Email.GetValue<string>();
            Item.Number = Number.GetValue<int>();
            Item.Tel = Tel.GetValue<string>();
            Item.Notes = Notes.GetValue<string>();
            Item.DateOfBirth = DateOfBirth.GetValue<DateTime?>();
            Item.TimeOfBirth = TimeOfBirth.GetValue<TimeSpan?>();
            Item.Type = Type.GetValue<Domain.ContactType>();
            Item.Photo = Photo.GetValue<Document>();
            Item.IsNice = IsNice.GetValue<bool?>();
        }

        async internal Task<bool> SaveInDatabase()
        {
            PopulateModel();

            using (var scope = Database.CreateTransactionScope())
            {
                try
                {
                    Item = Database.Save(Item).Clone();

                    var types = Types.GetValue<IEnumerable<ContactType>>().ToList();
                    // Remove unselected Types
                    Database.Delete(Item.Types.Where(x => x.ContactType.IsNoneOf(types)).ToList());

                    // Save newly selected ones
                    types.Except(Item.Types.Select(x => x.ContactType)).ToList()
                    .Do(x => Database.Save(new ContactContactType { Contact = Item, ContactType = x }));

                    scope.Complete();
                    return true;
                }
                catch (ValidationException ex)
                {
                    await Alert.Show("Invalid input", ex.ToFullMessage());
                }

                return false;
            }
        }
    }
}