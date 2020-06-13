using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
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
        
        private protected virtual void ChordPitchMutation(MelodyCandidate melody, int barIndex)
        {
            ChangePitchForARandomNote(melody.Bars[barIndex], mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected virtual void ScalePitchMutation(MelodyCandidate melody, int barIndex)
        {
            ChangePitchForARandomNote(melody.Bars[barIndex], mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected virtual void DurationEqualSplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Equal);
        }

        private protected virtual void DurationAnticipationSplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Anticipation);
        }

        private protected virtual void DurationDelaySplitMutation(MelodyCandidate melody, int barIndex)
        {
            DurationSplitOfARandomNote(melody.Bars[barIndex], DurationSplitRatio.Delay);
        }


    }
}
