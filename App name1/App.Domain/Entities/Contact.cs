namespace Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public partial class Contact : GuidEntity
    {
        /* -------------------------- Fields -------------------------*/

        private Document photo;

        private CachedReference<ContactType> cachedType = new CachedReference<ContactType>();

        internal List<ContactContactType> cachedTypes;

        /* -------------------------- Constructor -----------------------*/

        public Contact()
        {
            this.Deleting += this.Cascade_Deleting;
        }

        /* -------------------------- Properties -------------------------*/

        #region Date of birth Property

        public DateTime? DateOfBirth { get; set; }

        #endregion

        #region Email Property

        public string Email { get; set; }

        #endregion

        #region Is nice Property

        public bool? IsNice { get; set; }

        #endregion

        #region Name Property

        public string Name { get; set; }

        #endregion

        #region Notes Property

        public string Notes { get; set; }

        #endregion

        #region Number Property

        public int Number { get; set; }

        #endregion

        #region Photo Property

        [Newtonsoft.Json.JsonIgnore]
        public Document Photo
        {
            get
            {
                if (ReferenceEquals(this.photo, null))
                {
                    this.photo = Document.Empty().Attach(this, "Photo");
                }

                return this.photo;
            }

            set
            {
                if (!ReferenceEquals(this.photo, null))
                {
                    // Detach the previous file, so it doesn't get updated or deleted with this Contact instance.
                    this.photo.Detach();
                }

                if (ReferenceEquals(value, null))
                {
                    value = Document.Empty();
                }

                this.photo = value.Attach(this, "Photo");
            }
        }

        #endregion

        #region Tel Property

        public string Tel { get; set; }

        #endregion

        #region Time of birth Property

        public TimeSpan? TimeOfBirth { get; set; }

        #endregion

        #region Contact types Association

        [Calculated]
        [Newtonsoft.Json.JsonIgnore]
        public IEnumerable<ContactType> ContactTypes
        {
            get
            {
                return Types.Select(x => x.ContactType);
            }
        }

        #endregion

        #region Type Association

        public Guid? TypeId { get; set; }

        public ContactType Type
        {
            get
            {
                return cachedType.Get(this.TypeId);
            }

            set
            {
                this.TypeId = value.Get(c => c.ID);
            }
        }

        #endregion

        #region Types Association

        [Calculated]
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public IEnumerable<ContactContactType> Types
        {
            get
            {
                Func<IEnumerable<ContactContactType>> fetch = () => Database.GetList<ContactContactType>(c => c.Contact == this);

                if (Database.AnyOpenTransaction()) return fetch();
                else return cachedTypes ?? (cachedTypes = fetch().ToList());
            }
        }

        #endregion

        /* -------------------------- Methods ----------------------------*/

        public override string ToString()
        {
            return this.Name;
        }

        public new Contact Clone()
        {
            var result = (Contact)base.Clone();

            result.Photo = this.Photo.Clone();
            return result;
        }

        protected override void ValidateProperties(ValidationResult result)
        {
            // Validate Email property:

            if (this.Email != null && this.Email.Length > 200)
            {
                result.Add(nameof(Email), "The provided Email is too long. A maximum of 200 characters is acceptable.");
            }

            // Ensure Email matches Email address pattern:

            if (this.Email.HasValue() && !System.Text.RegularExpressions.Regex.IsMatch(this.Email, "\\s*\\w+([-+.'\\w])*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*\\s*"))
            {
                result.Add(nameof(Email), "The provided Email is not a valid Email address.");
            }

            // Validate Name property:

            if (this.Name.LacksValue())
            {
                result.Add(nameof(Name), "Name cannot be empty.");
            }

            if (this.Name != null && this.Name.Length > 200)
            {
                result.Add(nameof(Name), "The provided Name is too long. A maximum of 200 characters is acceptable.");
            }

            // Validate Number property:

            if (this.Number < 0)
            {
                result.Add(nameof(Number), "The value of Number must be 0 or more.");
            }

            if (this.Number > 20)
            {
                result.Add(nameof(Number), "The value of Number must be 20 or less.");
            }

            // Validate Tel property:

            if (this.Tel != null && this.Tel.Length > 200)
            {
                result.Add(nameof(Tel), "The provided Tel is too long. A maximum of 200 characters is acceptable.");
            }
        }

        private void Cascade_Deleting(object source, System.ComponentModel.CancelEventArgs e)
        {
            // Cascade delete the dependant Types:
            // Cascade delete on the Types of this Contact:
            Database.Delete(this.Types);
        }
    }
}