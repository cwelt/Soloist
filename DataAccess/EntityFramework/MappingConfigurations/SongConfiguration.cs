using CW.Soloist.DataAccess.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.DataAccess.EntityFramework.MappingConfigurations
{
    internal class SongConfiguration : EntityTypeConfiguration<Song>
    {
        public SongConfiguration()
        {
            ToTable("Songs");

            HasKey(s => s.Id);

            Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(50);

            Property(s => s.Artist)
                .HasMaxLength(50);

            Property(s => s.ChordsFileName)
                .IsRequired()
                .HasMaxLength(100);

            Property(s => s.MidiFileName)
                .IsRequired()
                .HasMaxLength(100);

            Property(s => s.MidiPlaybackFileName)
                .IsRequired()
                .HasMaxLength(100);
            

            Property(s => s.Created)
                .HasColumnName(nameof(Song.Created));

            Property(s => s.Modified)
                .HasColumnName(nameof(Song.Modified));

            Property(s => s.MelodyTrackIndex);

            Property(s => s.IsPublic)
                .HasColumnName(nameof(Song.IsPublic));

            Ignore(s => s.Chords);

            Ignore(s => s.Midi);

            HasRequired(s => s.User)
                .WithMany(u => u.Songs)
                .HasForeignKey(s => s.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
