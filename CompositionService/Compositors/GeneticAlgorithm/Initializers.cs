using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    internal partial class GeneticAlgorithmCompositor : Compositor
    {
        private protected void ArppegiateUp(IEnumerable<IBar> bars)
        {
            float chordDurationFraction;
            byte numberOfNotes;
            byte middlePitch = (byte)Math.Floor((byte)MinPitch + (byte)MaxOctave / 2F);
            
                foreach (IBar bar in bars)
            {
                bar.Notes.Clear(); 
                foreach (IChord chord in bar.Chords)
                {
                    chordDurationFraction = chord.Duration.Numerator / (float)(chord.Duration.Denominator);
                    numberOfNotes = (byte)(chordDurationFraction / DefaultDurationFraction);

                    var chordNotes = chord.GetArpeggioNotes(MinPitch, MaxPitch).ToArray();
                    int middle = chordNotes.Length / 2;
                    int step = 1;

                    // get middle root chordNotes.
                    for ( int i = 0, j = middle; i < numberOfNotes; i++)
                    {
                        bar.Notes.Add(new Note(chordNotes[j], DefaultDuration));
                        if (j == chordNotes.Length - 1)
                            j = middle;
                        else j++;
                    }
                }
            }
        }
    }
}
