using CsQuery.StringScanner.Implementation;
using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.WebApplication.ViewModels;
using EllipticCurve.Utils;
using SendGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace CW.Soloist.WebApplication.Validations
{
    /// <summary>
    /// Validation attribiute class for validating uploaded chord progression & MIDI files.
    /// </summary>
    internal class FileUploadValidation : ValidationAttribute
    {
        // initialization 
        private const int FileMaxLength = 1048576; // 1 MB 
        private string errorMessage = string.Empty;
        IMidiFile midi = null;
        IList<IBar> bars = null;
        SongFileType songFileType;
        SongViewModel songViewModel = null;
        HttpPostedFileBase fileHandler;
        HttpPostedFileBase midiFileHandler;
        HttpPostedFileBase chordsFileHandler;
        MelodyTrackIndex? melodyTrackIndex;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            songViewModel = validationContext.ObjectInstance as SongViewModel;

            midiFileHandler = songViewModel?.MidiFileHandler;
            chordsFileHandler = songViewModel?.ChordsFileHandler;
            melodyTrackIndex = songViewModel?.MelodyTrackIndex;


            // nothing to check if no file is uploaded for current file handler property
            if (value == null)
                return ValidationResult.Success;
            else fileHandler = value as HttpPostedFileBase;

            // set the subjected validated file type according to the subject file handler
            if (fileHandler == midiFileHandler)
                songFileType = SongFileType.MidiOriginalFile;
            else songFileType = SongFileType.ChordProgressionFile;

            // validate file metadata (size, mime type, etc)
            if (!FileUploadValidation.IsFileMetadataValid(fileHandler, songFileType, out errorMessage))
                return new ValidationResult(errorMessage);

            // fetch chords data from new chords file or from existing db record 
            if (chordsFileHandler != null)
            {
                if(!IsChordsFileValid(chordsFileHandler, songFileType, out bars, out errorMessage))
                    return new ValidationResult(errorMessage);
            }

            // validations for MIDI progression file
            if (songFileType == SongFileType.MidiOriginalFile)
            {
                if (!FileUploadValidation.IsMidiFileValid(midiFileHandler, out midi, out errorMessage))
                    return new ValidationResult(errorMessage);
            }

            // aditional validation for midi file and midi vs chords file 
            if (fileHandler == midiFileHandler)
            {
                // validate melody track index is not out of bounds 
                if (!CompositionContext.IsMelodyTrackIndexValid((int?)melodyTrackIndex, midi, out errorMessage))
                    return new ValidationResult(errorMessage);

                // validate that bars in CHORD progression are compatible with MIDI file 
                if (!CompositionContext.AreBarsCompatible(bars, midi, out errorMessage))
                    return new ValidationResult(errorMessage);
            }

            // if we got this far then hopefully everything is okay 
            return ValidationResult.Success;
        }


        public static bool IsFileMetadataValid(HttpPostedFileBase fileHandler, SongFileType fileType, out string errorMessage)
        {
            // initialize 
            errorMessage = null;

            // validate that the file is not empty 
            if (fileHandler.ContentLength == 0)
            {
                errorMessage = "File Cannot be Empty";
                return false;
            }

            // validate that the file is not too big  
            if (fileHandler.ContentLength > FileMaxLength)
            {
                errorMessage = $"File Size {fileHandler.ContentLength} " +
                               $"Exceeds Max Length of {FileMaxLength} Bytes";
                return false;
            }

            // validate that the file is from the correct MIME type 
            string[] validMimeTypes;
            if (fileType == SongFileType.ChordProgressionFile) // Chords file 
                validMimeTypes = new[] { "text/plain" };
            else // MIDI file 
                validMimeTypes = new[] { "audio/midi", "audio/x-midi", "audio/mid", "audio/x-mid" };

            if (!validMimeTypes.Contains(fileHandler.ContentType))
            {
                string validTypesToken = string.Empty;
                foreach (string mimeType in validMimeTypes)
                    validTypesToken += mimeType + " ";
                errorMessage =
                    $"File MIME Type {fileHandler.ContentType} Is Not Valid." +
                    $"Expected Format Type are one of the following: {validTypesToken}";
                return false;
            }

            // if we got this far then hopefully everything is okay
            return true;
        }

        public static bool IsChordsFileValid(HttpPostedFileBase chordsFileHandler, SongFileType fileType, out IList<IBar> bars, out string errorMessage)
        {
            bars = null;
            errorMessage = null;

            try
            {
                // build a stream reader for reading the chord progression 
                StreamReader streamReader = new StreamReader(chordsFileHandler.InputStream);
                bars = CompositionContext.ReadChordsFromFile(streamReader);

                // reset stream origin seek to beginning 
                chordsFileHandler.InputStream.Position = 0;

                // no errors found while parsing the file 
                return true;
            }
            catch (Exception ex)
            {

                // if this is a chord file validation, return any found errors if such exist
                if (fileType == SongFileType.ChordProgressionFile)
                {
                    errorMessage = ex.Message;
                    return false;
                }

                // if this is a check for midi file, no need to fail on chords
                return true; 
            }
        }

        public static bool IsMidiFileValid(HttpPostedFileBase midiFileHandler, out IMidiFile midiFile, out string errorMessage)
        {
            errorMessage = null;
            try
            {   // fetch midi content from stream 
                midiFile = CompositionContext.ReadMidiFile(midiFileHandler.InputStream);
                return true;
            }
            catch (Exception ex)
            {
                midiFile = null;
                errorMessage = ex.Message;
                return false;
            }
        }
    }



}

