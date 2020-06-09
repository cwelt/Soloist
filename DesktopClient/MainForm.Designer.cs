namespace DesktopClient
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fileUploadGroupBox = new System.Windows.Forms.GroupBox();
            this.lblChordsFileName = new System.Windows.Forms.Label();
            this.lblMidiFileName = new System.Windows.Forms.Label();
            this.btnUploadChords = new System.Windows.Forms.Button();
            this.btnUploadMidi = new System.Windows.Forms.Button();
            this.midiDataGroupBox = new System.Windows.Forms.GroupBox();
            this.lblMidiBars = new System.Windows.Forms.Label();
            this.lblMidiBpm = new System.Windows.Forms.Label();
            this.lblMidiTimeSignature = new System.Windows.Forms.Label();
            this.lblMidiTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBoxInstruments = new System.Windows.Forms.ComboBox();
            this.lblInstrument = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCompose = new System.Windows.Forms.Button();
            this.fileUploadGroupBox.SuspendLayout();
            this.midiDataGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileUploadGroupBox
            // 
            this.fileUploadGroupBox.BackColor = System.Drawing.Color.SkyBlue;
            this.fileUploadGroupBox.Controls.Add(this.lblChordsFileName);
            this.fileUploadGroupBox.Controls.Add(this.lblMidiFileName);
            this.fileUploadGroupBox.Controls.Add(this.btnUploadChords);
            this.fileUploadGroupBox.Controls.Add(this.btnUploadMidi);
            this.fileUploadGroupBox.Location = new System.Drawing.Point(12, 12);
            this.fileUploadGroupBox.Name = "fileUploadGroupBox";
            this.fileUploadGroupBox.Size = new System.Drawing.Size(391, 120);
            this.fileUploadGroupBox.TabIndex = 0;
            this.fileUploadGroupBox.TabStop = false;
            this.fileUploadGroupBox.Text = "File Upload";
            // 
            // lblChordsFileName
            // 
            this.lblChordsFileName.AutoSize = true;
            this.lblChordsFileName.Location = new System.Drawing.Point(174, 85);
            this.lblChordsFileName.Name = "lblChordsFileName";
            this.lblChordsFileName.Size = new System.Drawing.Size(105, 17);
            this.lblChordsFileName.TabIndex = 4;
            this.lblChordsFileName.Text = "No file selected";
            // 
            // lblMidiFileName
            // 
            this.lblMidiFileName.AutoSize = true;
            this.lblMidiFileName.Location = new System.Drawing.Point(174, 37);
            this.lblMidiFileName.Name = "lblMidiFileName";
            this.lblMidiFileName.Size = new System.Drawing.Size(105, 17);
            this.lblMidiFileName.TabIndex = 1;
            this.lblMidiFileName.Text = "No file selected";
            // 
            // btnUploadChords
            // 
            this.btnUploadChords.Location = new System.Drawing.Point(6, 79);
            this.btnUploadChords.Name = "btnUploadChords";
            this.btnUploadChords.Size = new System.Drawing.Size(141, 23);
            this.btnUploadChords.TabIndex = 3;
            this.btnUploadChords.Text = "Browse Chords File";
            this.btnUploadChords.UseVisualStyleBackColor = true;
            this.btnUploadChords.Click += new System.EventHandler(this.btnUploadChords_Click);
            // 
            // btnUploadMidi
            // 
            this.btnUploadMidi.Location = new System.Drawing.Point(6, 34);
            this.btnUploadMidi.Name = "btnUploadMidi";
            this.btnUploadMidi.Size = new System.Drawing.Size(141, 23);
            this.btnUploadMidi.TabIndex = 2;
            this.btnUploadMidi.Text = "Browse Midi File";
            this.btnUploadMidi.UseVisualStyleBackColor = true;
            this.btnUploadMidi.Click += new System.EventHandler(this.btnUploadMidi_Click);
            // 
            // midiDataGroupBox
            // 
            this.midiDataGroupBox.BackColor = System.Drawing.Color.LightSkyBlue;
            this.midiDataGroupBox.Controls.Add(this.lblMidiBars);
            this.midiDataGroupBox.Controls.Add(this.lblMidiBpm);
            this.midiDataGroupBox.Controls.Add(this.lblMidiTimeSignature);
            this.midiDataGroupBox.Controls.Add(this.lblMidiTitle);
            this.midiDataGroupBox.Location = new System.Drawing.Point(12, 155);
            this.midiDataGroupBox.Name = "midiDataGroupBox";
            this.midiDataGroupBox.Size = new System.Drawing.Size(391, 151);
            this.midiDataGroupBox.TabIndex = 1;
            this.midiDataGroupBox.TabStop = false;
            this.midiDataGroupBox.Text = "Midi Data";
            // 
            // lblMidiBars
            // 
            this.lblMidiBars.AutoSize = true;
            this.lblMidiBars.Location = new System.Drawing.Point(6, 116);
            this.lblMidiBars.Name = "lblMidiBars";
            this.lblMidiBars.Size = new System.Drawing.Size(110, 17);
            this.lblMidiBars.TabIndex = 6;
            this.lblMidiBars.Text = "Number Of Bars";
            // 
            // lblMidiBpm
            // 
            this.lblMidiBpm.AutoSize = true;
            this.lblMidiBpm.Location = new System.Drawing.Point(6, 88);
            this.lblMidiBpm.Name = "lblMidiBpm";
            this.lblMidiBpm.Size = new System.Drawing.Size(37, 17);
            this.lblMidiBpm.TabIndex = 5;
            this.lblMidiBpm.Text = "BPM";
            // 
            // lblMidiTimeSignature
            // 
            this.lblMidiTimeSignature.AutoSize = true;
            this.lblMidiTimeSignature.Location = new System.Drawing.Point(6, 57);
            this.lblMidiTimeSignature.Name = "lblMidiTimeSignature";
            this.lblMidiTimeSignature.Size = new System.Drawing.Size(104, 17);
            this.lblMidiTimeSignature.TabIndex = 4;
            this.lblMidiTimeSignature.Text = "Time Signature";
            // 
            // lblMidiTitle
            // 
            this.lblMidiTitle.AutoSize = true;
            this.lblMidiTitle.Location = new System.Drawing.Point(6, 29);
            this.lblMidiTitle.Name = "lblMidiTitle";
            this.lblMidiTitle.Size = new System.Drawing.Size(35, 17);
            this.lblMidiTitle.TabIndex = 1;
            this.lblMidiTitle.Text = "Title";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBox1.Controls.Add(this.cBoxInstruments);
            this.groupBox1.Controls.Add(this.lblInstrument);
            this.groupBox1.Location = new System.Drawing.Point(12, 312);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 92);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Composition Parameters";
            // 
            // cBoxInstruments
            // 
            this.cBoxInstruments.FormattingEnabled = true;
            this.cBoxInstruments.Location = new System.Drawing.Point(167, 29);
            this.cBoxInstruments.Name = "cBoxInstruments";
            this.cBoxInstruments.Size = new System.Drawing.Size(121, 24);
            this.cBoxInstruments.TabIndex = 8;
            this.cBoxInstruments.SelectedIndexChanged += new System.EventHandler(this.cBoxInstruments_SelectedIndexChanged);
            // 
            // lblInstrument
            // 
            this.lblInstrument.AutoSize = true;
            this.lblInstrument.Location = new System.Drawing.Point(6, 29);
            this.lblInstrument.Name = "lblInstrument";
            this.lblInstrument.Size = new System.Drawing.Size(125, 17);
            this.lblInstrument.TabIndex = 1;
            this.lblInstrument.Text = "Musical Instrument";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBox2.BackgroundImage = global::DesktopClient.Properties.Resources.musicPattern1;
            this.groupBox2.Controls.Add(this.btnCompose);
            this.groupBox2.Location = new System.Drawing.Point(431, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 392);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Composer";
            // 
            // btnCompose
            // 
            this.btnCompose.BackColor = System.Drawing.Color.LimeGreen;
            this.btnCompose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCompose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCompose.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompose.ForeColor = System.Drawing.Color.Black;
            this.btnCompose.Location = new System.Drawing.Point(161, 169);
            this.btnCompose.Name = "btnCompose";
            this.btnCompose.Size = new System.Drawing.Size(221, 48);
            this.btnCompose.TabIndex = 0;
            this.btnCompose.Text = "Compose!";
            this.btnCompose.UseVisualStyleBackColor = false;
            this.btnCompose.Click += new System.EventHandler(this.btnCompose_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ClientSize = new System.Drawing.Size(971, 416);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.midiDataGroupBox);
            this.Controls.Add(this.fileUploadGroupBox);
            this.Name = "MainForm";
            this.Text = "Soloist";
            this.fileUploadGroupBox.ResumeLayout(false);
            this.fileUploadGroupBox.PerformLayout();
            this.midiDataGroupBox.ResumeLayout(false);
            this.midiDataGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fileUploadGroupBox;
        private System.Windows.Forms.Button btnUploadChords;
        private System.Windows.Forms.Button btnUploadMidi;
        private System.Windows.Forms.Label lblMidiFileName;
        private System.Windows.Forms.Label lblChordsFileName;
        private System.Windows.Forms.GroupBox midiDataGroupBox;
        private System.Windows.Forms.Label lblMidiTimeSignature;
        private System.Windows.Forms.Label lblMidiTitle;
        private System.Windows.Forms.Label lblMidiBars;
        private System.Windows.Forms.Label lblMidiBpm;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cBoxInstruments;
        private System.Windows.Forms.Label lblInstrument;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCompose;
    }
}

