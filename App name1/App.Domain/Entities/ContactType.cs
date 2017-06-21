namespace Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public partial class ContactType : GuidEntity
    {
        /* -------------------------- Constructor -----------------------*/

        public ContactType()
        {
            this.Deleting += this.Cascade_Deleting;
        }

        /* -------------------------- Properties -------------------------*/

        #region Name Property

        public string Name { get; set; }

        #endregion

        /* -------------------------- Methods ----------------------------*/

        public static ContactType FindByName(string name)
        {
            return Database.Find<ContactType>(c => c.Name == name);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public new ContactType Clone()
        {
            return (ContactType)base.Clone();
        }

        protected override void ValidateProperties(ValidationResult result)
        {
            // Validate Name property:

            if (this.Name.LacksValue())
            {
                result.Add(nameof(Name), "Name cannot be empty.");
            }

            if (this.Name != null && this.Name.Length > 100)
            {
                result.Add(nameof(Name), "The provided Name is too long. A maximum of 100 characters is acceptable.");
            }

            // Ensure uniqueness of Name.

            if (Database.Any<ContactType>(c => c.Name == this.Name && c != this))
            {
                result.Add(nameof(Name), "Name must be unique. There is an existing Contact type record with the provided Name.");
            }
        }

        private void Cascade_Deleting(object source, System.ComponentModel.CancelEventArgs e)
        {
            // Cascade delete on the dependant Contact contact types:
            Database.DeleteAll<ContactContactType>(c => c.ContactType == this);

            var dependantContacts = Database.GetList<Contact>(c => c.Type == this);

            if (dependantContacts.Any())
            {
                throw new ValidationException("This Contact type cannot be deleted because of {0} dependent Contact record(s).", dependantContacts.Count());
            }
        }
    }
}