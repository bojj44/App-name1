using System.Linq;
namespace Domain
{
    using System;
    using System.Collections.Generic;
    using Zebble.Services;

    partial class User : IUser
    {
        /// <summary>
        /// Gets the roles of this user.
        /// </summary>
        public virtual IEnumerable<string> GetRoles() { yield return "User"; }

        public static object FindByEmail(object text)
        {
            throw new NotImplementedException();
        }
    }
}