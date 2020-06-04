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
    internal static class MidiServices
    {

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

        internal static IEnumerable<NotePitch> GetNotes(NoteName chordRootNote, ChordType chordType, int minOctave = 0, int maxOctave = 9)
        {
            var rootNote = ConvertToExternalNoteName(chordRootNote);
            Scale scale = null;

            switch (chordType)
            {
                case ChordType.Diminished:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Three, Interval.Six }, rootNote);
                    break;
                case ChordType.Dominant7:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7b9:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Three, Interval.FromHalfSteps(-1) }, rootNote);
                    break;
                case ChordType.Dominant7Suspended4:
                    scale = new Scale(new Interval[] { Interval.Five, Interval.Two, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7Augmented5:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Four, Interval.Two, Interval.Two }, rootNote);
                    break;
                case ChordType.Dominant7Sharped9:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Three, Interval.Four, Interval.FromHalfSteps(-3) }, rootNote);
                    break;
                case ChordType.HalfDiminished:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Three, Interval.Four, Interval.Two }, rootNote);
                    break;
                case ChordType.Major:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Five }, rootNote);
                    break;
                case ChordType.Major7:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Four, Interval.One }, rootNote);
                    break;
                case ChordType.MajorAugmented5:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Four, Interval.Four }, rootNote);
                    break;
                case ChordType.MajorSuspended4:
                    scale = new Scale(new Interval[] { Interval.Five, Interval.Two, Interval.Five }, rootNote);
                    break;
                case ChordType.Minor:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Five }, rootNote);
                    break;
                case ChordType.Minor6:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Two, Interval.Three }, rootNote);
                    break;
                case ChordType.Minor7:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Three, Interval.Two }, rootNote);
                    break;
                case ChordType.MinorMajor7:
                    scale = new Scale(new Interval[] { Interval.Three, Interval.Four, Interval.Four, Interval.One }, rootNote);
                    break;
                case ChordType.Major13:
                    scale = new Scale(new Interval[] { Interval.Four, Interval.Three, Interval.Two, Interval.Three }, rootNote);
                    break;
                default:
                    break;
            }

            int basePitch = 12;
            int minPitch = basePitch + (minOctave * 12);
            int maxPitch = basePitch + (maxOctave * 12);
            var result = from note in scale.GetNotes()
                         where (int)note.NoteNumber >= minPitch && (int)note.NoteNumber <= maxPitch
                         select (NotePitch)(int)note.NoteNumber;

            return result;
        }
    }
}
