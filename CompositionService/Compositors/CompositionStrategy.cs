﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Compositors
{
    /// <summary>
    /// Strategie for composing a melody.
    /// </summary>
    public enum CompositionStrategy
    {
        /// <summary> Genetic algorithm strategy. </summary>
        [Description("Genetic Algorithm Strategy")]
        GeneticAlgorithmStrategy,

        /// <summary> Arpeggio based strategy. </summary>
        [Description("Arpeggio Based Strategy")]
        ArpeggiatorStrategy,

        /// <summary> Scale ased strategy. </summary>
        [Description("Scale Based Strategy")]
        ScaleStrategy,
    }
}