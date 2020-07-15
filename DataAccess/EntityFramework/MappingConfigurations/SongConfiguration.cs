using CW.Soloist.DataAccess.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.EntityFramework.MappingConfigurations
{
    internal class SongConfiguration : EntityTypeConfiguration<Song>
    {
        public SongConfiguration()
        {
            Property(s => s.ChordsFileName)
                .IsRequired()
                .HasMaxLength(2000);

            Property(s => s.MidiFileName)
                .IsRequired()
                .HasMaxLength(2000);

            Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(50);

            Ignore(s => s.Chords);

            Ignore(s => s.Midi);
        }
    }
}
