using CW.Soloist.CompositionService;
using CW.Soloist.CompositionService.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class MainForm : Form
    {
        private List<Bitmap> _backgroundImageList;
        private System.Windows.Forms.Timer _imageTimer;
        private int _imageIndex = 1;

        private OpenFileDialog _midiFileDialog;
        private OpenFileDialog _chordsFileDialog;

        private string _midiFileName;
        private string _midiFilePath;
        private string _chordsFileName;
        private string _chordsFilePath;

        private MusicalInstrument _musicalInstrument;
        private Composition _composition;
        private IMidiFile _midiFile;


        public MainForm()
        {
            InitializeComponent();
            //InitializeBackGroundImages();

            // init file dialogs 
            _midiFileDialog = new OpenFileDialog { Filter = "Midi files(*.mid)|*.mid" };
            _chordsFileDialog = new OpenFileDialog { Filter = "Text files(*.txt)|*.txt" };

            cBoxInstruments.DataSource = Enum.GetValues(typeof(MusicalInstrument));

            
            

        }

        private void InitializeBackGroundImages()
        {
            // set initial image 
            this.BackgroundImage = Properties.Resources.musicPattern1;

            // initialize image list 
            _backgroundImageList = new List<Bitmap>();
            _backgroundImageList.Add(Properties.Resources.musicPattern1);
            _backgroundImageList.Add(Properties.Resources.notes);
            _backgroundImageList.Add(Properties.Resources.musicHead);

            // initialize timer & register tick event handler every 5 seconds
            _imageTimer = new System.Windows.Forms.Timer();
            _imageTimer.Interval = 3000;
            _imageTimer.Tick += new EventHandler(timer_Tick);
            _imageTimer.Start();
        }

        // tick event handler 
        void timer_Tick(object sender, EventArgs e)
        {
            // set next index 
            _imageIndex = (_imageIndex + 1) % _backgroundImageList.Count;

            // set next image 
            this.BackgroundImage = _backgroundImageList[_imageIndex];
        }

        private void btnUploadMidi_Click(object sender, EventArgs e)
        {
            if (_midiFileDialog.ShowDialog() == DialogResult.OK)
            {
                _midiFilePath = _midiFileDialog.FileName;
                _midiFileName = _midiFileDialog.SafeFileName;

                lblMidiFileName.Text = _midiFileName;
            }
        }

        private void btnUploadChords_Click(object sender, EventArgs e)
        {
            if (_chordsFileDialog.ShowDialog() == DialogResult.OK)
            {
                _chordsFilePath = _chordsFileDialog.FileName;
                _chordsFileName = _chordsFileDialog.SafeFileName;

                lblChordsFileName.Text = _chordsFileName;

                if(!string.IsNullOrWhiteSpace(_chordsFilePath) && !string.IsNullOrWhiteSpace(_midiFilePath))
                {
                    _composition = new Composition(_chordsFilePath, _midiFilePath, melodyTrackIndex: MelodyTrackIndex.TrackNumber1);
                    _midiFile = _composition.Compose()[0];
                    lblMidiTitle.Text += ": " + _midiFile.Title;
                    lblMidiBpm.Text += ": " + _midiFile.BeatsPerMinute.ToString();
                    lblMidiTimeSignature.Text += ": " + _midiFile.KeySignature.ToString();
                    lblMidiBars.Text += ": " + _midiFile.NumberOfBars.ToString();
                }
            }
        }

        private void cBoxInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            _musicalInstrument = (MusicalInstrument)cBoxInstruments.SelectedItem;
        }

        private void btnCompose_Click(object sender, EventArgs e)
        {

            var composition = new Composition(_chordsFilePath, _midiFilePath, melodyTrackIndex: MelodyTrackIndex.TrackNumber1);
            var newMelody = composition.Compose(musicalInstrument: _musicalInstrument);

            composition.MidiOutputFile.PlayAsync();
            var res = MessageBox.Show("Want to pause?", "Playing", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.Yes)
                composition.MidiOutputFile.Stop();
        }
    }
}
