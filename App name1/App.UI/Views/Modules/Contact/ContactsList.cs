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

    partial class ContactsList
    {
        public List<Contact> Items;

        private CachedValue<bool> _ShowPhotoColumn;

        public bool ShowPhotoColumn
        {
            get
            {
                if (_ShowPhotoColumn == null)
                {
                    _ShowPhotoColumn = Items.Any(x => x.Photo.HasValue());
                }

                return _ShowPhotoColumn.Value;
            }
        }

        public override async Task OnInitializing()
        {
            Items = GetSource().ToList();

            await base.OnInitializing();
            await InitializeComponents();
        }

        IEnumerable<Contact> GetSource() => Database.GetList<Contact>();

        public async Task ReloadButtonTapped() => await Reload();

        public async Task Reload() => await List.UpdateSource(Items = GetSource().ToList());

        partial class Row
        {
            public ContactsList Module => FindParent<ContactsList>();

            public override async Task OnInitializing()
            {
                await base.OnInitializing();
                await InitializeComponents();
            }

            public async Task ViewButtonTapped() => await Nav.Forward<Pages.Page1Enter>(new { Item = Item });

            public async Task DeleteButtonTapped()
            {
                if (!(await Alert.Confirm("Are you sure you want to delete this contact?"))) return;

                try
                {
                    Database.Delete(Item);
                }
                catch (Exception ex)
                {
                    await Alert.Show(ex.Message);
                    return;
                }

                await Module.Reload();
            }
        }
    }
}