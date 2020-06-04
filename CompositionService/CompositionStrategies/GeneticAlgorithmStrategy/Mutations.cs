using CW.Soloist.CompositionService.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies.GeneticAlgorithmStrategy
{
    public partial class GeneticAlgorithmCompositor
    {
        #region Pitch Change Mutation 

        // change pitches of individual notes in the genome a few step to other notes in the scale 
        private void ChangePitchMutation(MelodyGenome melodyGenome)
        {
            NotePitch[] chordNotes; //note pitches of current chord  
            INote[] actualNotes; // note of the individual candidate 
            INote newNote, currentNote;
            NotePitch newPitch, currentPitch;
            Random randomizer = new Random();
            int randomNoteIndex, randomPitchIndex;
            int delta = DefaultDuration;
            Boolean firstChord = true;

            foreach (IBar bar in melodyGenome.Bars)
            {
                foreach (IChord chord in bar.Chords)
                {
                    // skip empty bars 
                    if (bar.Notes.Count == 0) 
                        continue;

                    // select a random index of the note to be changed
                    if (firstChord)
                    {
                        if (bar.Notes.Count >= 2)
                            randomNoteIndex = randomizer.Next(0, (bar.Notes.Count / 2) - 1); //first bar half
                        else
                            randomNoteIndex = 0;
                        firstChord = false;
                    }
                    else
                    {
                        if (bar.Notes.Count >= 2)
                            randomNoteIndex = randomizer.Next(bar.Notes.Count / 2, bar.Notes.Count); //second bar half 
                        else
                            randomNoteIndex = 0;
                        firstChord = true;
                    }

                    // select a random pitch 
                    currentNote = bar.Notes[randomNoteIndex];
                    currentPitch = currentNote.Pitch;
                    chordNotes = (from notePitch in chord.GetNotes(MinOctave, MaxOctave)
                                  where notePitch != currentPitch
                                  && (int)notePitch <= (int)currentPitch + 4
                                  && (int)notePitch >= (int)currentPitch - 4
                                  select notePitch).ToArray();
                    if (chordNotes.Length != 0)
                    {
                        randomPitchIndex = randomizer.Next(0, chordNotes.Length - 1);
                        newPitch = chordNotes[randomPitchIndex];
                        newNote = new Note(newPitch, currentNote.Duration);
                        bar.Notes.RemoveAt(randomNoteIndex);
                        bar.Notes.Insert(randomNoteIndex, newNote);
                    }
                }
            }
        }
        #endregion
    }
}
