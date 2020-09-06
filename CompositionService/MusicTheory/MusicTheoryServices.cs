using System;
using System.Linq;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using CW.Soloist.CompositionService.UtilEnums;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Provides services which support operations and manipulations on 
    /// low level musical components such as note, chord scale etc. 
    /// </summary>
    internal class MusicTheoryServices
    {
        #region Constants  
        /// <summary> The number of half-tones in a single octave. </summary>
        internal const byte SemitonesInOctave = 12;

        /// <summary> Util enumeration for operations on durations. </summary>
        internal enum ArithmeticOperation { Add, Subtract };
        #endregion


        #region Methods

        #region GetNoteName
        /// <summary> Returns the <see cref="NoteName"/> for the given pitch.</summary>
        /// <param name="notePitch"></param>
        /// <returns> The name of the given pitch</returns>
        private static NoteName GetNoteName(NotePitch notePitch)
        {
            var pitch = (SevenBitNumber)(int)(notePitch);
            var noteName = Melanchall.DryWetMidi.MusicTheory.Note.Get(pitch).NoteName;
            return ConvertToInternalNoteName(noteName);
        }
        #endregion


        #region ConvertToInternalNoteName
        /// <summary>
        /// Converts note name given in a third-party used library to the internal used note name. 
        /// </summary>
        /// <param name="noteName"> Note name as defined in the third-party library. </param>
        /// <returns> The note name (see <see cref="NoteName"/> as defined in this project.</returns>
        private static NoteName ConvertToInternalNoteName(Melanchall.DryWetMidi.MusicTheory.NoteName noteName)
        {
            return (NoteName)Enum.Parse(typeof(NoteName), noteName.ToString());
        }
        #endregion


        #region ConvertToExternalNoteName
        /// <summary>
        /// Converts the given internal note name to a name defined in a third-party used library. 
        /// </summary>
        /// <param name="noteName"> The note name (see <see cref="NoteName"/> as defined in this project.</param>
        /// <returns> Note name as defined in the third-party library. </returns>
        private static Melanchall.DryWetMidi.MusicTheory.NoteName ConvertToExternalNoteName(NoteName noteName)
        {
            switch (noteName)
            {
                case NoteName.A:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.A;
                case NoteName.ASharp:
                case NoteName.BFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.ASharp;
                case NoteName.B:
                case NoteName.CFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.B;
                case NoteName.BSharp:
                case NoteName.C:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.C;
                case NoteName.CSharp:
                case NoteName.DFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.CSharp;
                case NoteName.D:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.D;
                case NoteName.DSharp:
                case NoteName.EFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.DSharp;
                case NoteName.E:
                case NoteName.FFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.E;
                case NoteName.ESharp:
                case NoteName.F:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.F;
                case NoteName.FSharp:
                case NoteName.GFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.FSharp;
                case NoteName.G:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.G;
                case NoteName.GSharp:
                case NoteName.AFlat:
                    return Melanchall.DryWetMidi.MusicTheory.NoteName.GSharp;
                default:
                    throw new InvalidCastException("Undefined casting for this note name");
            }
        }
        #endregion


        #region GetNotes
        /// <summary>
        /// Returns notes that sound "good" under the given chord and pitch range restriction. 
        /// </summary>
        /// <param name="chord"> he requested chrod to map the notes against. </param>
        /// <param name="mappingSource"> The mapping source - either scale notes or the chord's arpeggio notes. </param>
        /// <param name="minPitch"> Lower bound pitch range constraint for the mapped notes. </param>
        /// <param name="maxPitch"> Upper bound pitch range constraint for the mapped notes. </param>
        /// <returns> Notes that sound "good" under the given chord, mapping source, and pitch range restriction. </returns>
        internal static IEnumerable<NotePitch> GetNotes(IChord chord, ChordNoteMappingSource mappingSource, NotePitch minPitch, NotePitch maxPitch)
        {
            // build the scale (sequence of intervals from the chord's root note)
            var rootNote = ConvertToExternalNoteName(chord.ChordRoot);
            Scale scale = null;
            
            switch (chord.ChordType)
            {
                case ChordType.Diminished:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Three, Interval.Six }, rootNote);
                    else // diminished arppeigo with added 9th  
                        scale = new Scale(new Interval[] { Interval.One, Interval.Two, Interval.Three, Interval.Six }, rootNote);
                    break;
                case ChordType.Dominant7:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Two }, rootNote);
                    else // mixolydian scale with omitted 4th
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Three, Interval.Two, Interval.One, Interval.Two}, rootNote);
                    break;
                case ChordType.Dominant7b9:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Three, Interval.FromHalfSteps(-1) }, rootNote);
                    else // harmonic minor v scale with 6th omitted
                        scale = new Scale(new Interval[] { Interval.One, Interval.Three, Interval.One, Interval.Two, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7Suspended4:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Five, Interval.Two, Interval.Three, Interval.Two }, rootNote);
                    else // mixolydian scale
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.One, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7Augmented5:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Four, Interval.Two, Interval.Two }, rootNote);
                    else // whole tone scale
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Two, Interval.Two, Interval.Two, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7Sharped9:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Four, Interval.FromHalfSteps(-3) }, rootNote);
                    else // mixolydian #2 scale with omitted 4th
                        scale = new Scale(new Interval[] { Interval.Three, Interval.One, Interval.Three, Interval.Two, Interval.One, Interval.Two }, rootNote);
                    break;
                case ChordType.HalfDiminished:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Three, Interval.Four, Interval.Two }, rootNote);
                    else // locrian mode scale with ommited 2nd
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.Two }, rootNote);
                    break;
                case ChordType.Major:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Five }, rootNote);
                    else // dorian major scale with omitted 4th and 7th 
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Three, Interval.Two, Interval.Three }, rootNote);
                    break;
                case ChordType.Major7:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Four, Interval.One }, rootNote);
                    else // dorian major scale with omitted 4th
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Three, Interval.Two, Interval.Two, Interval.One }, rootNote);
                    break;
                case ChordType.MajorAugmented5:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Four, Interval.Four }, rootNote);
                    else // lydian augmented scale
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Two, Interval.Two, Interval.One, Interval.Two, Interval.One }, rootNote);
                    break;
                case ChordType.MajorSuspended4:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Five, Interval.Two, Interval.Five }, rootNote);
                    else // mixolydian scale
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.One, Interval.Two }, rootNote);
                    break;
                case ChordType.Minor:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Five }, rootNote);
                    else // aeolian minor scale with omitted 2nd and 6th
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Two, Interval.Two, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.Minor6:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Two, Interval.Three }, rootNote);
                    else // dorian minor scale with omitted 7th 
                        scale = new Scale(new Interval[] { Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.Two, Interval.Three }, rootNote);
                    break;
                case ChordType.Minor7:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Three, Interval.Two }, rootNote);
                    else // aeolian minor scale with omitted 6th
                        scale = new Scale(new Interval[] { Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.MinorMajor7:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Four, Interval.One }, rootNote);
                    else // harmonic minor scale
                        scale = new Scale(new Interval[] { Interval.Two, Interval.One, Interval.Two, Interval.Two, Interval.One, Interval.Three, Interval.One }, rootNote);
                    break;
                case ChordType.Major13:
                    if (mappingSource == ChordNoteMappingSource.Chord)
                        scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Two, Interval.Three }, rootNote);
                    else // dorian major scale with omitted 4th and 7th 
                        scale = new Scale(new Interval[] { Interval.Two, Interval.Two, Interval.Three, Interval.Two, Interval.Three }, rootNote);
                    break;
                default:
                    break;
            }

            // get actual notes and filter out results according to range constraints
            var result = from note in scale.GetNotes()
                         where (int)note.NoteNumber >= (byte)minPitch && (int)note.NoteNumber <= (byte)maxPitch
                         select (NotePitch)(int)note.NoteNumber;

            return result;
        }


        /// <inheritdoc cref="GetNotes(IChord, ChordNoteMappingSource, NotePitch, NotePitch)"/>
        internal static IEnumerable<NotePitch> GetNotes(IChord chord, ChordNoteMappingSource mappingSource, int minOctave = 0, int maxOctave = 9)
        {
            int basePitch = MusicTheoryServices.SemitonesInOctave;
            NotePitch minPitch = (NotePitch)(basePitch + (minOctave * MusicTheoryServices.SemitonesInOctave));
            NotePitch maxPitch = (NotePitch)(basePitch + ((maxOctave + 1) * MusicTheoryServices.SemitonesInOctave) - 1);
            return MusicTheoryServices.GetNotes(chord, mappingSource, minPitch, maxPitch);
        }
        #endregion


        #region DurationArithmetic
        /// <summary>
        /// Calculates durations that are the result of adding and/or subtracting 
        /// length of existing durations. 
        /// </summary>
        /// <param name="operation"> The requested arithmetic operaton (add, subtract). </param>
        /// <param name="duration1"> The first duration operand. </param>
        /// <param name="duration2"> The second duration operand. </param>
        /// <returns> The result duration of the operation (sum or difference). </returns>
        internal static IDuration DurationArithmetic(ArithmeticOperation operation, IDuration duration1, IDuration duration2)
        {
            MusicalTimeSpan timeSpan1 = new MusicalTimeSpan(duration1.Numerator, duration1.Denominator, true);
            MusicalTimeSpan timeSpan2 = new MusicalTimeSpan(duration2.Numerator, duration2.Denominator, true);
            MusicalTimeSpan newTimeSpan = null;
            switch (operation)
            {
                case ArithmeticOperation.Add:
                    newTimeSpan = timeSpan1.Add(timeSpan2, TimeSpanMode.LengthLength) as MusicalTimeSpan;
                    break;
                case ArithmeticOperation.Subtract:
                    newTimeSpan = timeSpan1.Subtract(timeSpan2, TimeSpanMode.LengthLength) as MusicalTimeSpan;
                    break;
                default:
                    throw new NotSupportedException($"{nameof(operation)} is currently unsupported");
            }
            return new Duration((byte)newTimeSpan?.Numerator, (byte)newTimeSpan?.Denominator);
        }
        #endregion

        #endregion
    }
}
