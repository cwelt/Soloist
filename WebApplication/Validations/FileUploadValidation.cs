using CsQuery.StringScanner.Implementation;
using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.CompositionService.MusicTheory;
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

namespace CW.Soloist.WebApplication.Validations
{
    /// <summary>
    /// Validation attribiute class for validating uploaded chord progression & MIDI files.
    /// </summary>
    internal class FileUploadValidation : ValidationAttribute
    {
        // initialization 
        private const int FileMaxLength = 1048576; // 1 MB 
        private string errorMessage = null;
        private IList<IBar> bars = null;
        private IMidiFile midi = null;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // fetch input form data 
            SongViewModel formData = validationContext.ObjectInstance as SongViewModel;
            if (formData == null)
                return ValidationResult.Success;

            // fetch the uploaded file 
            HttpPostedFileBase file = value as HttpPostedFileBase;

            // validate that the file is not empty 
            if (file.ContentLength == 0)
                return new ValidationResult("File Cannot be Empty");

            // validate that the file is not too big  
            if (file.ContentLength > FileMaxLength)
                return new ValidationResult(
                    $"File Size {file.ContentLength} " +
                    $"Exceeds Max Length of {FileMaxLength} Bytes");

            // validate that the file is from the correct MIME type 
            string[] validMimeTypes;
            if (file == formData.ChordsFileHandler) // Chords file 
                validMimeTypes = new[] { "text/plain" };
            else // MIDI file 
                validMimeTypes = new[] { "audio/midi", "audio/x-midi", "audio/mid", "audio/x-mid" }; 

            if (!validMimeTypes.Contains(file.ContentType))
            {
                string validTypesToken = string.Empty;
                foreach (string mimeType in validMimeTypes)
                    validTypesToken += mimeType + " ";
                return new ValidationResult(
                    $"File MIME Type {file.ContentType} Is Not Valid." +
                    $"Expected Format Type are one of the following: {validTypesToken}");
            }

            // fetch chords data from chords file stream 
            if (formData.ChordsFileHandler != null)
            {
                try
                {
                    // build a stream reader for reading the chord progression 
                    StreamReader streamReader = new StreamReader(formData.ChordsFileHandler.InputStream);
                    bars = Composition.ReadChordsFromFile(streamReader);

                    // reset stream origin seek to beginning 
                    formData.ChordsFileHandler.InputStream.Position = 0;
                }
                catch (Exception ex)
                {
                    // if this is a chord file validation, return any found errors if such exist
                    if (file == formData.ChordsFileHandler)
                        return new ValidationResult(ex.Message);
                }
            }

            // validations for MIDI progression file
            if (file == formData.MidiFileHandler)
            {
                try
                {   // fetch midi content from stream 
                    midi = Composition.ReadMidiFile(file.InputStream);
                }
                catch (Exception ex)
                {
                    return new ValidationResult(ex.Message);
                }

                // validate melody track index is not out of bounds 
                if (!Composition.IsMelodyTrackIndexValid((int?)formData.MelodyTrackIndex, midi, out errorMessage))
                    return new ValidationResult(errorMessage);

                // validate that bars in CHORD progression are compatible with MIDI file 
                if (!Composition.AreBarsCompatible(bars, midi, out errorMessage))
                    return new ValidationResult(errorMessage);
            }

            // if we got this far then hopefully everything is okay 
            return ValidationResult.Success;
        }
    }
}


