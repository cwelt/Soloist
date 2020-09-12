using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Enums
{
    /// <summary>
    /// Represents a way to split a note's durtaion into two new durations 
    /// which sum up the original note duration.
    /// </summary>
    internal enum DurationSplitRatio
    {
        /// <summary> 
        /// Represents an equal ratio split of 1:1.
        /// <para> the two new durations after the split have an 
        /// equal duration which is half of the original duration. </para>
        /// </summary>
        Equal,

        /// <summary> 
        /// Represents an unequal ratio split of 1:3.
        /// <para> The first duration after the split would be 1/4 long
        /// in relation to the original duration, and the second duration 
        /// would be 3/4 long in relation to the original duration.</para>
        /// <para> For example, if the original duration was 1/2,  
        /// then after the split the first duration would be 1/8,
        /// and the second duration would be 3/8. </para>
        /// </summary>
        Anticipation,

        /// <summary> 
        /// Represents an unequal ratio split of 3:1.
        /// <para> The first duration after the split would be 3/4 long
        /// in relation to the original duration, and the second duration 
        /// would be 1/4 long in relation to the original duration.</para>
        /// <para> For example, if the original duration was 1/2,  
        /// then after the split the first duration would be 3/8,
        /// and the second duration would be 1/8. </para>
        /// </summary>
        Delay
    }
}
