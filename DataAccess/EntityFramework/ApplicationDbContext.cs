using System.Data.Entity;
using CW.Soloist.DataAccess.DomainModels;
using Microsoft.AspNet.Identity.EntityFramework;
using CW.Soloist.DataAccess.EntityFramework.MappingConfigurations;

namespace CW.Soloist.DataAccess.EntityFramework
{
    /// <summary>
    /// The DbContext that is used to manage all the persisted entites 
    /// managed in this web application via the Entity Framework.
    /// <para> This context is used both for cross-platform custom application 
    /// entities, such as Song, as well as general entities for managing the AAA security 
    /// prinicple, such as the User and Role entities. </para>
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor 
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }


        // custom application tracked persisted entites 
        public virtual DbSet<Song> Songs { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // register the ORM configurations 
            modelBuilder.Configurations.Add(new SongConfiguration());
        }
    }
}
