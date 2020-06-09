using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.Midi
{
    /// <summary>
    /// General MIDI <a href="https://bit.ly/30pmSzP">program number</a>  
    /// which represents a musical instrument.
    /// </summary>
    public enum MusicalInstrument
    {
        /**********************************************/
        /*                 Pianos                     */
        /**********************************************/
        /// <summary> Acoustic Grand Piano. </summary>
        AcousticGrandPiano,

        /// <summary> Bright Acoustic Piano. </summary>
        BrightAcousticPiano,

        /// <summary>  Electric Grand Piano. </summary>
        ElectricGrandPiano,

        /// <summary> Honky-tonk Piano. </summary>
        HonkyTonkPiano,

        /// <summary> Electric Piano 1. </summary>
        ElectricPiano1,

        /// <summary> Electric Piano 2. </summary>
        ElectricPiano2,

        /// <summary> Harpsichord. </summary>
        Harpsichord,

        /// <summary> Clavichord. </summary>
        Clavichord,

        /**********************************************/
        /*            Chromatic Percussions           */
        /**********************************************/
        /// <summary> Celesta. </summary>
        Celesta,

        /// <summary> Glockenspiel. </summary>
        Glockenspiel,

        /// <summary> Music Box. </summary>
        MusicBox,

        /// <summary> Vibraphone. </summary>
        Vibraphone,

        /// <summary> Marimba. </summary>
        Marimba,

        /// <summary> Xylophone. </summary>
        Xylophone,

        /// <summary> Tubular Bells. </summary>
        TubularBells,

        /// <summary> Dulcimer. </summary>
        Dulcimer,

        /**********************************************/
        /*                 Organs                     */
        /**********************************************/
        /// <summary> Drawbar Organ. </summary>
        DrawbarOrgan,

        /// <summary> Percussive Organ. </summary>
        PercussiveOrgan,

        /// <summary> Rock Organ. </summary>
        RockOrgan,

        /// <summary> Church Organ. </summary>
        ChurchOrgan,

        /// <summary> Reed Organ. </summary>
        ReedOrgan,

        /// <summary> Accordion. </summary>
        Accordion,

        /// <summary> Harmonica. </summary>
        Harmonica,

        /// <summary> Tango Accordion. </summary>
        TangoAccordion,

        /**********************************************/
        /*                 Guitars                    */
        /**********************************************/
        /// <summary> Classical Nylon Guitar. </summary>
        ClassicalGuitar,

        /// <summary> Acoustic Steel Guitar. </summary>
        AcousticGuitar,

        /// <summary> Electric Jazz Guitar. </summary>
        JazzGuitar,

        /// <summary> Electric Clean Guitar. </summary>
        ElectricGuitar,

        /// <summary> Electric Guitar With Muted Strokes. </summary>
        MutedGuitar,

        /// <summary> Electric Overdriven Guitar. </summary>
        OverdrivenGuitar,

        /// <summary> Electric Distorted Guitar </summary>
        DistortionGuitar,

        /// <summary> Electric Guitar With Artificial Pitch Harmonics. </summary>
        HarmonicsGuitar,

        /**********************************************/
        /*                   Bass                     */
        /**********************************************/
        /// <summary> Acoustic Bass Guitar. </summary>
        AcousticBass,

        /// <summary> Electric Bass Guitar With Finger Picking Sound. </summary>
        ElectricFingerBass,

        /// <summary> Electric Bass Guitar With Plastic Pick Picking Sound. </summary>
        ElectricPickBass,

        /// <summary> Electric Fretless Bass. </summary>
        FretlessBass,

        /// <summary> Electric Slap Bass Guitar 1. </summary>
        SlapBass1,

        /// <summary> Electric Slap Bass Guitar 2. </summary>
        SlapBass2,

        /// <summary> Synthesizer Keyboard Bass 1. </summary>
        SynthBass1,

        /// <summary> Synthesizer Keyboard Bass 2. </summary>
        SynthBass2,

        /**********************************************/
        /*                 Strings                    */
        /**********************************************/
        /// <summary> Violin. </summary>
        Violin,

        /// <summary> Viola. </summary>
        Viola,

        /// <summary> Cello. </summary>
        Cello,

        /// <summary> Contra Bass (Double Bass). </summary>
        ContraBass,

        /// <summary> Tremelo Strings. </summary>
        TremeloStrings,

        /// <summary> Pizzicato Strings. </summary>
        PizzicatoStrings,

        /// <summary> Orchestral Harp. </summary>
        OrchestralHarp,

        /// <summary> Timpani. </summary>
        Timpani,

        /**********************************************/
        /*                 Ensemble                   */
        /**********************************************/
        /// <summary> String Ensemble 1. </summary>
        StringEnsemble1,

        /// <summary> String Ensemble 2. </summary>
        StringEnsemble2,

        /// <summary> Synthesizer Strings 1. </summary>
        SynthStrings1,

        /// <summary> Synthesizer Strings 2. </summary>
        SynthStrings2,

        /// <summary> Choir Aahs. </summary>
        ChoirAahs,

        /// <summary> Voice Oohs. </summary>
        VoiceOohs,

        /// <summary> Synthesizer Choir. </summary>
        SynthChoir,

        /// <summary> Orchestra Hit. </summary>
        OrchestraHit,

        /**********************************************/
        /*                 Brass                      */
        /**********************************************/
        /// <summary> Trumpet. </summary>
        Trumpet,

        /// <summary> Trombone. </summary>
        Trombone,

        /// <summary> Tuba. </summary>
        Tuba,

        /// <summary> Muted Trumpet. </summary>
        MutedTrumpet,

        /// <summary> French Horn. </summary>
        FrenchHorn,

        /// <summary> Brass Section. </summary>
        BrassSection,

        /// <summary> Synthesizer Brass 1. </summary>
        SynthBrass1,

        /// <summary> Synthesizer Brass 2. </summary>
        SynthBrass2,

        /**********************************************/
        /*                  Reed                      */
        /**********************************************/
        /// <summary> Soprano Saxophone. </summary>
        SopranoSax,

        /// <summary> Alto Saxophone. </summary>
        AltoSax,

        /// <summary> Tenor Saxophone. </summary>
        TenorSax,

        /// <summary> Baritone Saxophone. </summary>
        BaritoneSax,

        /// <summary> Oboe. </summary>
        Oboe,

        /// <summary> English Horn. </summary>
        EnglishHorn,

        /// <summary> Bassoon. </summary>
        Bassoon,

        /// <summary> Clarinet. </summary>
        Clarinet,

        /**********************************************/
        /*                  Pipes                     */
        /**********************************************/
        /// <summary> Piccolo. </summary>
        Piccolo,

        /// <summary> Flute. </summary>
        Flute,

        /// <summary> Recorder. </summary>
        Recorder,

        /// <summary> Pan Flute. </summary>
        PanFlute,

        /// <summary> Blown Bottle. </summary>
        BlownBottle,

        /// <summary> Shakuhachi. </summary>
        Shakuhachi,

        /// <summary> Whistle. </summary>
        Whistle,

        /// <summary> Ocarina. </summary>
        Ocarina,

        /**********************************************/
        /*                  Pipes                     */
        /**********************************************/
        /// <summary> Lead1 Square. </summary>
        Lead1Square,

        /// <summary> Lead2 Sawtooth. </summary>
        Lead2Sawtooth,

        /// <summary> Lead3 Calliope. </summary>
        Lead3Calliope,

        /// <summary> Lead4 Chiff. </summary>
        Lead4Chiff,

        /// <summary> Lead5 Charang. </summary>
        Lead5Charang,

        /// <summary> Lead6 Voice. </summary>
        Lead6Voice,

        /// <summary> Lead7 Voice. </summary>
        Lead7Voice,

        /// <summary> Lead8 Bass and Lead. </summary>
        Lead8BassLead,

        /**********************************************/
        /*              Synthesizer Pad               */
        /**********************************************/
        /// <summary> Pad1 New Age. </summary>
        Pad1NewAge,

        /// <summary> Pad2 Warm. </summary>
        Pad2Warm,

        /// <summary> Pad3 Polysynth. </summary>
        Pad3Polysynth,

        /// <summary> Pad4 Choir. </summary>
        Pad4Choir,

        /// <summary> Pad5 Bowed. </summary>
        Pad5Bowed,

        /// <summary> Pad6 Metallic. </summary>
        Pad6Metallic,

        /// <summary> Pad7 Halo. </summary>
        Pad7Halo,

        /// <summary> Pad8 Sweep. </summary>
        Pad8Sweep,

        /**********************************************/
        /*            Synthesizer Effects             */
        /**********************************************/
        /// <summary> FX1 Rain. </summary>
        FX1Rain,

        /// <summary> FX2 Soundtrack. </summary>
        FX2Soundtrack,

        /// <summary> FX3 Crystal. </summary>
        FX3Crystal,

        /// <summary> FX4 Atmosphere. </summary>
        FX4Atmosphere,

        /// <summary> FX5 Brightness. </summary>
        FX5Brightness,

        /// <summary> FX6 Goblins. </summary>
        FX6Goblins,

        /// <summary> FX7 Echoes. </summary>
        FX7Echoes,

        /// <summary> FX8 Sci-Fi. </summary>
        FX8SciFi,

        /**********************************************/
        /*                  Ethnic                    */
        /**********************************************/
        /// <summary> Sitar. </summary>
        Sitar,

        /// <summary> Banjo. </summary>
        Banjo,

        /// <summary> Shamisen. </summary>
        Shamisen,

        /// <summary> Koto. </summary>
        Koto,

        /// <summary> Kalimba. </summary>
        Kalimba,

        /// <summary> Bagpipe. </summary>
        Bagpipe,

        /// <summary> Fiddle. </summary>
        Fiddle,

        /// <summary> Shanai. </summary>
        Shanai
    }
}
