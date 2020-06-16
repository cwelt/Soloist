using System.ComponentModel;

namespace CW.Soloist.CompositionService.UtilEnums
{
    /// <summary>
    /// Source for determining the pitch value range for the composition - 
    /// either the user's custom selection of lower and upper bound parameters,
    /// or the range from the exisiting melody in the midi file, if such melody exists.
    /// </summary>
    public enum PitchRangeSource
    {
        /// <summary> 
        /// Pitch range would be determined according to the existing lowest 
        /// and highest pitches in the existing melody in the midi file.
        /// </summary>
        [Description("Midi File Melody Track Range")]
        MidiFile,

        /// <summary>
        /// Pitch range would be determined by the user's custom selection.
        /// </summary>
        [Description("Custom Range Selecton")]
        Custom
    }
}
