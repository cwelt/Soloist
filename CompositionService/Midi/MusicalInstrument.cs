using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Acoustic Grand Piano")]
        AcousticGrandPiano,

        /// <summary> Bright Acoustic Piano. </summary>
        [Display(Name = "Bright Acoustic Piano")]
        BrightAcousticPiano,

        /// <summary>  Electric Grand Piano. </summary>
        [Display(Name = "Electric Grand Piano")]
        ElectricGrandPiano,

        /// <summary> Honky-tonk Piano. </summary>
        [Display(Name = "Honky-tonk Piano")]
        /// 
        HonkyTonkPiano,

        /// <summary> Electric Piano 1. </summary>
        [Display(Name = "Electric Piano 1")]
        ElectricPiano1,

        /// <summary> Electric Piano 2. </summary>
        [Display(Name = "Electric Piano 2")]
        ElectricPiano2,

        /// <summary> Harpsichord. </summary>
        [Display(Name = "Harpsichord")]
        Harpsichord,

        /// <summary> Clavichord. </summary>
        [Display(Name = "Clavichord")]
        Clavichord,

        /**********************************************/
        /*            Chromatic Percussions           */
        /**********************************************/
        /// <summary> Celesta. </summary>
        [Display(Name = "Celesta")]
        Celesta,

        /// <summary> Glockenspiel. </summary>
        [Display(Name = "Glockenspiel")]
        Glockenspiel,

        /// <summary> Music Box. </summary>
        [Display(Name = "Music Box")]
        MusicBox,

        /// <summary> Vibraphone. </summary>
        [Display(Name = "Vibraphone")]
        Vibraphone,

        /// <summary> Marimba. </summary>
        [Display(Name = "Marimba")]
        Marimba,

        /// <summary> Xylophone. </summary>
        [Display(Name = "Xylophone")]
        Xylophone,

        /// <summary> Tubular Bells. </summary>
        [Display(Name = "Tubular Bells")]
        TubularBells,

        /// <summary> Dulcimer. </summary>
        [Display(Name = "Dulcimer")]
        Dulcimer,

        /**********************************************/
        /*                 Organs                     */
        /**********************************************/
        /// <summary> Drawbar Organ. </summary>
        [Display(Name = "Drawbar Organ")]
        DrawbarOrgan,

        /// <summary> Percussive Organ. </summary>
        [Display(Name = "Percussive Organ")]
        PercussiveOrgan,

        /// <summary> Rock Organ. </summary>
        [Display(Name = "Rock Organ")]
        RockOrgan,

        /// <summary> Church Organ. </summary>
        [Display(Name = "Church Organ")]
        ChurchOrgan,

        /// <summary> Reed Organ. </summary>
        [Display(Name = "Reed Organ")]
        ReedOrgan,

        /// <summary> Accordion. </summary>
        [Display(Name = "Accordion")]
        Accordion,

        /// <summary> Harmonica. </summary>
        [Display(Name = "Harmonica")]
        Harmonica,

        /// <summary> Tango Accordion. </summary>
        [Display(Name = "Tango Accordion")]
        TangoAccordion,

        /**********************************************/
        /*                 Guitars                    */
        /**********************************************/
        /// <summary> Classical Nylon Guitar. </summary>
        [Display(Name = "Classical Nylon Guitar")]
        ClassicalGuitar,

        /// <summary> Acoustic Steel Guitar. </summary>
        [Display(Name = "Acoustic Steel Guitar")]
        AcousticGuitar,

        /// <summary> Electric Jazz Guitar. </summary>
        [Display(Name = "Electric Jazz Guitar")]
        JazzGuitar,

        /// <summary> Electric Clean Guitar. </summary>
        [Display(Name = "Electric Clean Guitar")]
        ElectricGuitar,

        /// <summary> Electric Guitar With Muted Strokes. </summary>
        [Display(Name = "Electric Guitar With Muted Strokes")]
        MutedGuitar,

        /// <summary> Electric Overdriven Guitar. </summary>
        [Display(Name = "Electric Overdriven Guitar")]
        OverdrivenGuitar,

        /// <summary> Electric Distorted Guitar </summary>
        [Display(Name = "Electric Distorted Guitar")]
        DistortionGuitar,

        /// <summary> Electric Guitar With Artificial Pitch Harmonics. </summary>
        [Display(Name = "Electric Guitar With Artificial Pitch Harmonics")]
        HarmonicsGuitar,

        /**********************************************/
        /*                   Bass                     */
        /**********************************************/
        /// <summary> Acoustic Bass Guitar. </summary>
        [Display(Name = "Acoustic Bass Guitar")]
        AcousticBass,

        /// <summary> Electric Bass Guitar With Finger Picking Sound. </summary>
        [Display(Name = "Electric Bass Guitar With Finger Picking Sound")]
        ElectricFingerBass,

        /// <summary> Electric Bass Guitar With Plastic Pick Picking Sound. </summary>
        [Display(Name = "Electric Bass Guitar With Plastic Pick Picking Sound")]
        ElectricPickBass,

        /// <summary> Electric Fretless Bass. </summary>
        [Display(Name = "Electric Fretless Bass")]
        FretlessBass,

        /// <summary> Electric Slap Bass Guitar 1. </summary>
        [Display(Name = "Electric Slap Bass Guitar 1")]
        SlapBass1,

        /// <summary> Electric Slap Bass Guitar 2. </summary>
        [Display(Name = "Electric Slap Bass Guitar 2")]
        SlapBass2,

        /// <summary> Synthesizer Keyboard Bass 1. </summary>
        [Display(Name = "Synthesizer Keyboard Bass 1")]
        SynthBass1,

        /// <summary> Synthesizer Keyboard Bass 2. </summary>
        [Display(Name = "Synthesizer Keyboard Bass 2")]
        SynthBass2,

        /**********************************************/
        /*                 Strings                    */
        /**********************************************/
        /// <summary> Violin. </summary>
        [Display(Name = "Violin")]
        Violin,

        /// <summary> Viola. </summary>
        [Display(Name = "Viola")]
        Viola,

        /// <summary> Cello. </summary>
        [Display(Name = "Cello")]
        Cello,

        /// <summary> Contra Bass (Double Bass). </summary>
        [Display(Name = "Contra Bass (Double Bass)")]
        ContraBass,

        /// <summary> Tremelo Strings. </summary>
        [Display(Name = "Tremelo Strings")]
        TremeloStrings,

        /// <summary> Pizzicato Strings. </summary>
        [Display(Name = "Pizzicato Strings")]
        PizzicatoStrings,

        /// <summary> Orchestral Harp. </summary>
        [Display(Name = "Orchestral Harp")]
        OrchestralHarp,

        /// <summary> Timpani. </summary>
        [Display(Name = "Timpani")]
        Timpani,

        /**********************************************/
        /*                 Ensemble                   */
        /**********************************************/
        /// <summary> String Ensemble 1. </summary>
        [Display(Name = "String Ensemble 1")]
        StringEnsemble1,

        /// <summary> String Ensemble 2. </summary>
        [Display(Name = "String Ensemble 2")]
        StringEnsemble2,

        /// <summary> Synthesizer Strings 1. </summary>
        [Display(Name = "Synthesizer Strings 1")]
        SynthStrings1,

        /// <summary> Synthesizer Strings 2. </summary>
        [Display(Name = "Synthesizer Strings 2")]
        SynthStrings2,

        /// <summary> Choir Aahs. </summary>
        [Display(Name = "Choir Aahs")]
        ChoirAahs,

        /// <summary> Voice Oohs. </summary>
        [Display(Name = "Voice Oohs")]
        VoiceOohs,

        /// <summary> Synthesizer Choir. </summary>
        [Display(Name = "Synthesizer Choir")]
        SynthChoir,

        /// <summary> Orchestra Hit. </summary>
        [Display(Name = "Orchestra Hit")]
        OrchestraHit,

        /**********************************************/
        /*                 Brass                      */
        /**********************************************/
        /// <summary> Trumpet. </summary>
        [Display(Name = "Trumpet")]
        Trumpet,

        /// <summary> Trombone. </summary>
        [Display(Name = "Trombone")]
        Trombone,

        /// <summary> Tuba. </summary>
        [Display(Name = "Tuba")]
        Tuba,

        /// <summary> Muted Trumpet. </summary>
        [Display(Name = "Muted Trumpet")]
        MutedTrumpet,

        /// <summary> French Horn. </summary>
        [Display(Name = "French Horn")]
        FrenchHorn,

        /// <summary> Brass Section. </summary>
        [Display(Name = "Brass Section")]
        BrassSection,

        /// <summary> Synthesizer Brass 1. </summary>
        [Display(Name = "Synthesizer Brass 1")]
        SynthBrass1,

        /// <summary> Synthesizer Brass 2. </summary>
        [Display(Name = "Synthesizer Brass 2")]
        SynthBrass2,

        /**********************************************/
        /*                  Reed                      */
        /**********************************************/
        /// <summary> Soprano Saxophone. </summary>
        [Display(Name = "Soprano Saxophone")]
        SopranoSax,

        /// <summary> Alto Saxophone. </summary>
        [Display(Name = "Alto Saxophone")]
        AltoSax,

        /// <summary> Tenor Saxophone. </summary>
        [Display(Name = "Tenor Saxophone")]
        TenorSax,

        /// <summary> Baritone Saxophone. </summary>
        [Display(Name = "Baritone Saxophone")]
        BaritoneSax,

        /// <summary> Oboe. </summary>
        [Display(Name = "Oboe")]
        Oboe,

        /// <summary> English Horn. </summary>
        [Display(Name = "English Horn")]
        EnglishHorn,

        /// <summary> Bassoon. </summary>
        [Display(Name = "Bassoon")]
        Bassoon,

        /// <summary> Clarinet. </summary>
        [Display(Name = "Clarinet")]
        Clarinet,

        /**********************************************/
        /*                  Pipes                     */
        /**********************************************/
        /// <summary> Piccolo. </summary>
        [Display(Name = "Piccolo")]
        Piccolo,

        /// <summary> Flute. </summary>
        [Display(Name = "Flute")]
        Flute,

        /// <summary> Recorder. </summary>
        [Display(Name = "Recorder")]
        Recorder,

        /// <summary> Pan Flute. </summary>
        [Display(Name = "Pan Flute")]
        PanFlute,

        /// <summary> Blown Bottle. </summary>
        [Display(Name = "Blown Bottle")]
        BlownBottle,

        /// <summary> Shakuhachi. </summary>
        [Display(Name = "Shakuhachi")]
        Shakuhachi,

        /// <summary> Whistle. </summary>
        [Display(Name = "Whistle")]
        Whistle,

        /// <summary> Ocarina. </summary>
        [Display(Name = "Ocarina")]
        Ocarina,

        /**********************************************/
        /*                  Pipes                     */
        /**********************************************/
        /// <summary> Lead1 Square. </summary>
        [Display(Name = "Lead1 Square")]
        Lead1Square,

        /// <summary> Lead2 Sawtooth. </summary>
        [Display(Name = "Lead2 Sawtooth")]
        Lead2Sawtooth,

        /// <summary> Lead3 Calliope. </summary>
        [Display(Name = "Lead3 Calliope")]
        Lead3Calliope,

        /// <summary> Lead4 Chiff. </summary>
        [Display(Name = "Lead4 Chiff")]
        Lead4Chiff,

        /// <summary> Lead5 Charang. </summary>
        [Display(Name = "Lead5 Charang")]
        Lead5Charang,

        /// <summary> Lead6 Voice. </summary>
        [Display(Name = "Lead6 Voice")]
        Lead6Voice,

        /// <summary> Lead7 Voice. </summary>
        [Display(Name = "Lead7 Voice")]
        Lead7Voice,

        /// <summary> Lead8 Bass and Lead. </summary>
        [Display(Name = "Lead8 Bass and Lead")]
        Lead8BassLead,

        /**********************************************/
        /*              Synthesizer Pad               */
        /**********************************************/
        /// <summary> Pad1 New Age. </summary>
        [Display(Name = "Pad1 New Age")]
        Pad1NewAge,

        /// <summary> Pad2 Warm. </summary>
        [Display(Name = "Pad2 Warm")]
        Pad2Warm,

        /// <summary> Pad3 Polysynth. </summary>
        [Display(Name = "Pad3 Polysynth")]
        Pad3Polysynth,

        /// <summary> Pad4 Choir. </summary>
        [Display(Name = "Pad4 Choir")]
        Pad4Choir,

        /// <summary> Pad5 Bowed. </summary>
        [Display(Name = "Pad5 Bowed")]
        Pad5Bowed,

        /// <summary> Pad6 Metallic. </summary>
        [Display(Name = "Pad6 Metallic")]
        Pad6Metallic,

        /// <summary> Pad7 Halo. </summary>
        [Display(Name = "Pad7 Halo")]
        Pad7Halo,

        /// <summary> Pad8 Sweep. </summary>
        [Display(Name = "Pad8 Sweep")]
        Pad8Sweep,

        /**********************************************/
        /*            Synthesizer Effects             */
        /**********************************************/
        /// <summary> FX1 Rain. </summary>
        [Display(Name = "FX1 Rain")]
        FX1Rain,

        /// <summary> FX2 Soundtrack. </summary>
        [Display(Name = "FX2 Soundtrack")]
        FX2Soundtrack,

        /// <summary> FX3 Crystal. </summary>
        [Display(Name = "FX3 Crystal")]
        FX3Crystal,

        /// <summary> FX4 Atmosphere. </summary>
        [Display(Name = "FX4 Atmosphere")]
        FX4Atmosphere,

        /// <summary> FX5 Brightness. </summary>
        [Display(Name = "FX5 Brightness")]
        FX5Brightness,

        /// <summary> FX6 Goblins. </summary>
        [Display(Name = "FX6 Goblins")]
        FX6Goblins,

        /// <summary> FX7 Echoes. </summary>
        [Display(Name = "FX7 Echoes")]
        FX7Echoes,

        /// <summary> FX8 Sci-Fi. </summary>
        [Display(Name = "FX8 Sci-Fi")]
        FX8SciFi,

        /**********************************************/
        /*                  Ethnic                    */
        /**********************************************/
        /// <summary> Sitar. </summary>
        [Display(Name = "Sitar")]
        Sitar,

        /// <summary> Banjo. </summary>
        [Display(Name = "Banjo")]
        Banjo,

        /// <summary> Shamisen. </summary>
        [Display(Name = "Shamisen")]
        Shamisen,

        /// <summary> Koto. </summary>
        [Display(Name = "Koto")]
        Koto,

        /// <summary> Kalimba. </summary>
        [Display(Name = "Kalimba")]
        Kalimba,

        /// <summary> Bagpipe. </summary>
        [Display(Name = "Bagpipe")]
        Bagpipe,

        /// <summary> Fiddle. </summary>
        [Display(Name = "Fiddle")]
        Fiddle,

        /// <summary> Shanai. </summary>
        [Display(Name = "Shanai")]
        Shanai
    }
}
