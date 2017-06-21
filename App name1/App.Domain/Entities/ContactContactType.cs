namespace Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public partial class ContactContactType : GuidEntity
    {
        /* -------------------------- Fields -------------------------*/

        private CachedReference<Contact> cachedContact = new CachedReference<Contact>();
        private Guid? previousContactId;

        private CachedReference<ContactType> cachedContactType = new CachedReference<ContactType>();

        /* -------------------------- Constructor -----------------------*/

        public ContactContactType()
        {
            this.Loaded += (s, e) => previousContactId = ContactId;
        }

        /* -------------------------- Properties -------------------------*/

        #region Contact Association

        public Guid? ContactId { get; set; }

        public Contact Contact
        {
            get
            {
                return cachedContact.Get(this.ContactId);
            }

            set
            {
                this.ContactId = value.Get(c => c.ID);
            }
        }

        #endregion

        #region Contact type Association

        public Guid? ContactTypeId { get; set; }

        public ContactType ContactType
        {
            get
            {
                return cachedContactType.Get(this.ContactTypeId);
            }

            set
            {
                this.ContactTypeId = value.Get(c => c.ID);
            }
        }

        #endregion

        /* -------------------------- Methods ----------------------------*/

        public override string ToString()
        {
            return $"Contact contact type ({this.ID})";
        }

        public new ContactContactType Clone()
        {
            return (ContactContactType)base.Clone();
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void InvalidateCachedReferences()
        {
            base.InvalidateCachedReferences();

            new [] { ContactId, previousContactId }.ExceptNull().Distinct()
            .Select(id => Cache.Current.Get<Contact>(id)).ExceptNull().Do(x => x.cachedTypes = null);
        }
    }
}