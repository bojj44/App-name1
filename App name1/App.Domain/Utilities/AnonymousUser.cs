namespace Domain
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using Zebble.Framework;

    /// <summary>
    /// Represents a user who has not logged in.
    /// </summary>
    [TransientEntity]
    public class AnonymousUser : User
    {
        /// <summary>
        /// Gets the fix text of Anonymous.
        /// </summary>
        public override string ToString() => "Anonymous";

        /// <summary>
        /// Gets the roles of this user.
        /// </summary>
        public override IEnumerable<string> GetRoles()
        {
            yield return "Anonymous";
        }

        /// <summary>
        /// Gets a unique GUID per user IP address to satisfy AntiForgeryToken.
        /// </summary>
        public override Guid ID
        {
            get { return Guid.Empty; }
            set { }
        }
    }
}