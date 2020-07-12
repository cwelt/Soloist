using CW.Soloist.CompositionService.MusicTheory;
using System.Collections.Generic;
using System.Linq;

namespace CW.Soloist.CompositionService.Compositors.GeneticAlgorithm
{
    // TODO: Use abstraction -> Implement interface of  melody genome 
    internal class MelodyCandidate
    {
        internal uint Generation { get; }
        internal double FitnessGrade { get; set; } = 0;

        /// <summary> List of bars which contain the melody of this candidate. </summary>
        internal IList<IBar> Bars { get; set; }
        protected internal bool IsDirty { get; set; } = true;

        /// <summary>
        ///  Constructs a new candidate with the given generation, based on the 
        ///  given composition structure bar sequence. 
        ///  <para> If <paramref name="includeExistingMelody"/> is set to true, 
        ///  the existing melody in the given composition structure would be copied 
        ///  into the bar sequence of the new instansiated candidate. 
        ///  Otherwise, only time signature and chord data would be copied, 
        ///  and the notes sequence in this candidate's bar sequence would be empty.</para>
        /// </summary>
        /// <param name="generation"> The generation of this candidate regarding the candidate 
        /// population of the genetic algorithm which has this candidate in context. </param>
        /// <param name="compositionStructure"> Fully initialized bar sequence with 
        /// time signatures, chords, and possibly notes, to base on this candidates bar sequence. </param>
        /// <param name="includeExistingMelody"> If set to true, melody from the given 
        /// composition structure bar sequence would be copied into the candidate's bar sequence 
        /// in addition to the time signature and chord, otherwise, only time signature and chord
        /// would be copied and notes would be set to empty collections. </param>
        internal MelodyCandidate(uint generation, IEnumerable<IBar> compositionStructure, bool includeExistingMelody = false)
        {
            // initialize the generation of the current candidate 
            Generation = generation;

            // mark new candidate as "dirty" for evaluation 
            IsDirty = true;

            // initialize bar sequence 
            if (includeExistingMelody)
            {
                /* If this candidate should be based on an existing melody seed,
                 * initialize bar sequence to include all data from the given composition 
                 * structure bar sequence: time signature, chords and notes as well. */
                Bars = new List<IBar>(compositionStructure.Count());
                foreach (IBar bar in compositionStructure)
                    Bars.Add(new Bar(bar));
            }
            else
            {
                /* If this candidate should not be based on any seed, 
                 * initialize bars to contain only the time signature and chords 
                 * from the given composition structure bar sequence, setting the 
                 * notes in each bar to an empty melody. */
                Bars = Composition.CloneChordProgressionBars(compositionStructure);
            }

        }
    }
}