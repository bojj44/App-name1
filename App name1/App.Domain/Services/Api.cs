namespace Domain
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Zebble.Framework;
    using Zebble;
    using System.Threading.Tasks;

    /// <summary>
    /// Acts as a gateway to the server API. You can customise its behaviour if needed.
    /// </summary>
    public class Api : BaseApi
    {
        public static class User
        {
            public static async Task Login(string email, string password)
            {
                var token = await Post<string>("v1/login", new { Email = email, Password = password });

                if (token.HasValue())
                    Device.IO.File("SessionToken.txt").WriteAllText(token);

                // Runtime.User = await HttpGet<User>("v1/login/user-details").ResultOrCacheOrDefault();
            }
        }
    }
}