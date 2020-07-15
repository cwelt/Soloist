using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework.MappingConfigurations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.EntityFramework
{
    public class SoloistContext : DbContext
    {
        public SoloistContext()
    : base("name=SoloistContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public virtual DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SongConfiguration());
        }
    }
}
