using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.Services
{
    /// <summary>
    /// Compose a solo-melody over a given playback using a genetic algorithm.
    /// <para>
    /// This class implements a concrete composition strategy. 
    /// </para>
    /// </summary>
    public class GeneticAlgorithmCompositor : CompositorStrategy
    {
        /// <summary>
        /// <inheritdoc></inheritdoc>
        /// </summary>
        public override object Compose(string playback)
        {
            throw new NotImplementedException();
        }
    }
}
