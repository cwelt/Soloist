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
        
        private protected virtual void ChordPitchMutation(IBar bar)
        {
            ChangePitchForARandomNote(bar, mappingSource: ChordNoteMappingSource.Chord);
        }

        private protected virtual void ScalePitchMutation(IBar bar)
        {
            ChangePitchForARandomNote(bar, mappingSource: ChordNoteMappingSource.Scale);
        }

        private protected virtual void DurationEqualSplitMutation(IBar bar)
        {
            DurationSplitOfARandomNote(bar, DurationSplitRatio.Equal);
        }

        private protected virtual void DurationAnticipationSplitMutation(IBar bar)
        {
            DurationSplitOfARandomNote(bar, DurationSplitRatio.Anticipation);
        }

        private protected virtual void DurationDelaySplitMutation(IBar bar)
        {
            DurationSplitOfARandomNote(bar, DurationSplitRatio.Delay);
        }
    }
}
