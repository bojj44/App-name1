namespace Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using Zebble.Framework;
    using Zebble.Framework.Data;

    public partial class User : GuidEntity, IComparable<User>
    {
        /* -------------------------- Properties -------------------------*/

        #region Email Property

        public string Email { get; set; }

        #endregion

        #region First name Property

        public string FirstName { get; set; }

        #endregion

        #region Is deactivated Property

        public bool IsDeactivated { get; set; }

        #endregion

        #region Last name Property

        public string LastName { get; set; }

        #endregion

        #region Name Property

        [Calculated]
        [Newtonsoft.Json.JsonIgnore]
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        #endregion

        #region Password Property

        public string Password { get; set; }

        #endregion

        /* -------------------------- Methods ----------------------------*/

        public static User FindByEmail(string email)
        {
            return Database.Find<User>(u => u.Email == email);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public new User Clone()
        {
            return (User)base.Clone();
        }

        public int CompareTo(User other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            else
            {
                return this.Name.CompareTo(other.Name);
            }
        }

        public override int CompareTo(object other)
        {
            if (other is User) return CompareTo(other as User);
            else return base.CompareTo(other);
        }

        protected override void ValidateProperties(ValidationResult result)
        {
            // Validate Email property:

            if (this.Email.LacksValue())
            {
                result.Add(nameof(Email), "Email cannot be empty.");
            }

            if (this.Email != null && this.Email.Length > 100)
            {
                result.Add(nameof(Email), "The provided Email is too long. A maximum of 100 characters is acceptable.");
            }

            // Ensure Email matches Email address pattern:

            if (this.Email.HasValue() && !System.Text.RegularExpressions.Regex.IsMatch(this.Email, "\\s*\\w+([-+.'\\w])*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*\\s*"))
            {
                result.Add(nameof(Email), "The provided Email is not a valid Email address.");
            }

            // Ensure uniqueness of Email.

            if (Database.Any<User>(u => u.Email == this.Email && u != this))
            {
                result.Add(nameof(Email), "Email must be unique. There is an existing User record with the provided Email.");
            }

            // Validate FirstName property:

            if (this.FirstName.LacksValue())
            {
                result.Add(nameof(FirstName), "First name cannot be empty.");
            }

            if (this.FirstName != null && this.FirstName.Length > 200)
            {
                result.Add(nameof(FirstName), "The provided First name is too long. A maximum of 200 characters is acceptable.");
            }

            // Validate LastName property:

            if (this.LastName.LacksValue())
            {
                result.Add(nameof(LastName), "Last name cannot be empty.");
            }

            if (this.LastName != null && this.LastName.Length > 200)
            {
                result.Add(nameof(LastName), "The provided Last name is too long. A maximum of 200 characters is acceptable.");
            }

            // Validate Password property:

            if (this.Password != null && this.Password.Length > 100)
            {
                result.Add(nameof(Password), "The provided Password is too long. A maximum of 100 characters is acceptable.");
            }
        }
    }
}