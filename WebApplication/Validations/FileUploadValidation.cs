﻿using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using CW.Soloist.CompositionService;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.CompositionService.Midi;
using CW.Soloist.WebApplication.ViewModels;
using System.ComponentModel.DataAnnotations;
using CW.Soloist.CompositionService.MusicTheory;


namespace CW.Soloist.WebApplication.Validations
{
    /// <summary>
    /// Validation attribiute class for validating uploaded chord progression and MIDI files.
    /// </summary>
    internal class FileUploadValidation : ValidationAttribute
    {
        #region Fields
        // field initialization
        private const int FileMaxLength = 1048576; // 1 MB 
        private string _errorMessage = string.Empty;
        private IMidiFile _midi = null;
        private IList<IBar> _bars = null;
        private SongFileType _songFileType;
        private SongViewModel _songViewModel = null;
        private HttpPostedFileBase _fileHandler;
        private HttpPostedFileBase _midiFileHandler;
        private HttpPostedFileBase _chordsFileHandler;
        private MelodyTrackIndex? _melodyTrackIndex;
        #endregion

        #region IsValid
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _songViewModel = validationContext.ObjectInstance as SongViewModel;

            _midiFileHandler = _songViewModel?.MidiFileHandler;
            _chordsFileHandler = _songViewModel?.ChordsFileHandler;
            _melodyTrackIndex = _songViewModel?.MelodyTrackIndex;


            // nothing to check if no file is uploaded for current file handler property
            if (value == null)
                return ValidationResult.Success;
            else _fileHandler = value as HttpPostedFileBase;

            // set the subjected validated file type according to the subject file handler
            if (_fileHandler == _midiFileHandler)
                _songFileType = SongFileType.MidiOriginalFile;
            else _songFileType = SongFileType.ChordProgressionFile;

            // validate file metadata (size, mime type, etc)
            if (!FileUploadValidation.IsFileMetadataValid(_fileHandler, _songFileType, out _errorMessage))
                return new ValidationResult(_errorMessage);

            // fetch chords data from new chords file or from existing db record 
            if (_chordsFileHandler != null)
            {
                if(!IsChordsFileValid(_chordsFileHandler, _songFileType, out _bars, out _errorMessage))
                    return new ValidationResult(_errorMessage);
            }

            // validations for MIDI progression file
            if (_songFileType == SongFileType.MidiOriginalFile)
            {
                if (!FileUploadValidation.IsMidiFileValid(_midiFileHandler, out _midi, out _errorMessage))
                    return new ValidationResult(_errorMessage);
            }

            // aditional validation for midi file and midi vs chords file 
            if (_fileHandler == _midiFileHandler)
            {
                // validate melody track index is not out of bounds 
                if (!CompositionContext.IsMelodyTrackIndexValid((int?)_melodyTrackIndex, _midi, out _errorMessage))
                    return new ValidationResult(_errorMessage);

                // validate that bars in CHORD progression are compatible with MIDI file 
                if (!CompositionContext.AreBarsCompatible(_bars, _midi, out _errorMessage))
                    return new ValidationResult(_errorMessage);
            }

            // if we got this far then hopefully everything is okay 
            return ValidationResult.Success;
        }
        #endregion

        #region IsFileMetadataValid
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
        #endregion

        #region IsChordsFileValid
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
        #endregion

        #region IsMidiFileValid
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
        #endregion
    }
}

