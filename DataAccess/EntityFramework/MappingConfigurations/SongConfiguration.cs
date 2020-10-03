using CW.Soloist.DataAccess.DomainModels;
using System.Data.Entity.ModelConfiguration;

namespace CW.Soloist.DataAccess.EntityFramework.MappingConfigurations
{
    /// <summary>
    /// Configuration mapping class between the <see cref="Song"/> model class, 
    /// and the underlying database table "Songs". 
    /// </summary>
    internal class SongConfiguration : EntityTypeConfiguration<Song>
    {
        public SongConfiguration()
        {
            // Define name of the mapped database table 
            ToTable("Songs");

            // Define the property that represent the unique identifier that should be used as a primary key 
            HasKey(s => s.Id);

            // Map class properties to table columns 
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

            // define properties that should not be mapped to the database such as navigation properties 
            Ignore(s => s.Chords);

            Ignore(s => s.Midi);

            // map mutliplicty relationship to Users database table 
            HasRequired(s => s.User)
                .WithMany(u => u.Songs)
                .HasForeignKey(s => s.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
