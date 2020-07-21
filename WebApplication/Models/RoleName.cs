using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CW.Soloist.WebApplication.Models
{
    /// <summary>
    /// Role name for managing authorizations & restricted customized access.
    /// </summary>
    public static class RoleName
    {
        /// <summary> Administrator role with maximum privileges. </summary>
        public const string Admin = "Admin";

        /// <summary> Regular user role with restricted privileges. </summary>
        public const string ApplicationUser = "ApplicationUser";
    }
}