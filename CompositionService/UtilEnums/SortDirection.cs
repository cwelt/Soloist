using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.UtilEnums
{
    /// <summary>
    /// Represents a sort ordering direction: ascending or descending.
    /// </summary>
    internal enum SortOrder
    {
        /// <summary> 
        /// Ascending order sequence. 
        /// Elements are sorted in increasing order from small to big.
        /// </summary>
        Ascending,

        /// <summary> 
        /// Descending order sequence. 
        /// Elements are sorted in decreasing order from big to small.
        /// </summary>
        Descending
    }
}
