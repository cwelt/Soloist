using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CW.Soloist.DataAccess.DomainModels
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public IList<Song> Songs { get; set; }
    }


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


    /// <summary>
    /// Enumeration that represents the various possible user activities 
    /// on the application resources that might require an authorization check, for 
    /// example activity of displaying a resource or updating a resource.
    /// </summary>
    public enum AuthorizationActivity
    {
        Create = 1,
        Update = 2,
        Display = 3,
        Cancel = 4,
        Delete = 5
    }
}