using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.MusicTheory
{
    /// <summary>
    /// Provides services which support operations and manipulations low level musical components 
    /// such as note, chord scale etc. 
    /// </summary>
    internal partial class MusicTheoryServices : IMusicTheoryService
    {

        internal static readonly byte SemitonesInOctave = 12;

        private static NoteName GetNoteName(NotePitch notePitch)
        {
            var pitch = (SevenBitNumber)(int)(notePitch);
            var noteName = Melanchall.DryWetMidi.MusicTheory.Note.Get(pitch).NoteName;
            return ConvertToInternalNoteName(noteName);
        }

        private static NoteName ConvertToInternalNoteName(Melanchall.DryWetMidi.MusicTheory.NoteName noteName)
        {
            return (NoteName)Enum.Parse(typeof(NoteName), noteName.ToString());
        }

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

        public IEnumerable<NotePitch> GetChordNotes(IChord chord, ChordNoteMappingSource mappingSource = ChordNoteMappingSource.Chord, int minOctave = 0, int maxOctave = 9)
        {
            return MusicTheoryServices.GetNotes(chord, mappingSource, minOctave, maxOctave);
        }

        internal static IEnumerable<NotePitch> GetNotes(IChord chord, ChordNoteMappingSource mappingSource, int minOctave = 0, int maxOctave = 9)
        {
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

            int basePitch = MusicTheoryServices.SemitonesInOctave;
            int minPitch = basePitch + (minOctave * MusicTheoryServices.SemitonesInOctave);
            int maxPitch = basePitch + ((maxOctave + 1) * MusicTheoryServices.SemitonesInOctave) - 1;
            var result = from note in scale.GetNotes()
                         where (int)note.NoteNumber >= minPitch && (int)note.NoteNumber <= maxPitch
                         select (NotePitch)(int)note.NoteNumber;

            return result;
        }
    }
}
