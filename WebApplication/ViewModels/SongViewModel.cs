using System;
using System.Web;
using System.ComponentModel;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.CompositionService.Enums;
using CW.Soloist.WebApplication.Validations;
using System.ComponentModel.DataAnnotations;


namespace CW.Soloist.WebApplication.ViewModels
{
    public class SongViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [DisplayName("Song Name")]
        public string Title { get; set; }

        [Required]
        [StringLength(70)]
        public string Artist { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [FileUploadValidation]
        [DisplayName("MIDI File")]
        public HttpPostedFileBase MidiFileHandler { get; set; }

        [DisplayName("MIDI File Name")]
        public string MidiFileName { get; set; }

        [DisplayName("MIDI Playback File")]
        public string MidiPlaybackFileName { get; set; }

        public IMidiFile MidiData { get; set; }

        [Required]
        [DisplayName("Melody Track Number")]
        public MelodyTrackIndex? MelodyTrackIndex { get; set; }
        public string MelodyTrackIndexDescription
        {
            get => MelodyTrackIndex?.GetDisplayName() ??
                CW.Soloist.CompositionService.Midi.MelodyTrackIndex.NoMelodyTrackInFile
                .GetDisplayName();
        }

        [Required]
        [FileUploadValidation]
        [DisplayName("Chords File")]
        public HttpPostedFileBase ChordsFileHandler { get; set; }

        [DisplayName("Chords File Name")]
        public string ChordsFileName { get; set; }

        [DisplayName("Chord Progression")]
        public String ChordProgression { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [DisplayName("Is Song Public To All Users")]
        public bool IsPublic { get; set; }

        public bool IsUserAuthorizedToEdit { get; set; }
        public bool IsUserAuthorizedToDelete { get; set; }
        public bool IsAdminUser { get; set; }

        public string StatusMessage { get; set; }

        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            Id = song.Id;
            Artist = song.Artist;
            Title = song.Title;
            MidiFileName = song.MidiFileName;
            MidiPlaybackFileName = song.MidiPlaybackFileName;
            ChordsFileName = song.ChordsFileName;
            IsPublic = song.IsPublic;
            Created = song.Created;
            Modified = song.Modified;
            MelodyTrackIndex = song.MelodyTrackIndex;
        }
    }
}