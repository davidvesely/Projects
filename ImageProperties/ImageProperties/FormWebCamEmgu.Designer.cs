namespace ImageProperties
{
    partial class FormWebCamEmgu
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
            this.buttonCapture = new System.Windows.Forms.Button();
            this.textBoxTreshold = new System.Windows.Forms.TextBox();
            this.labelTresh = new System.Windows.Forms.Label();
            this.labelWidth = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.textBoxWidth = new System.Windows.Forms.TextBox();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonScreenshots = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonLoadFrame = new System.Windows.Forms.Button();
            this.checkBoxVizDetectedSpots = new System.Windows.Forms.CheckBox();
            this.checkBoxWriteDetected = new System.Windows.Forms.CheckBox();
            this.trbY_Prag = new ImageProperties.MyTrackBar();
            this.trbY_Add = new ImageProperties.MyTrackBar();
            this.trbY_K = new ImageProperties.MyTrackBar();
            this.trbB_Prag = new ImageProperties.MyTrackBar();
            this.trbB_Add = new ImageProperties.MyTrackBar();
            this.trbB_K = new ImageProperties.MyTrackBar();
            this.trbG_Prag = new ImageProperties.MyTrackBar();
            this.trbG_Add = new ImageProperties.MyTrackBar();
            this.trbG_K = new ImageProperties.MyTrackBar();
            this.trbR_Prag = new ImageProperties.MyTrackBar();
            this.trbR_Add = new ImageProperties.MyTrackBar();
            this.trbR_K = new ImageProperties.MyTrackBar();
            this.buttonCalibrate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCapture
            // 
            this.buttonCapture.Location = new System.Drawing.Point(12, 12);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(75, 23);
            this.buttonCapture.TabIndex = 3;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // textBoxTreshold
            // 
            this.textBoxTreshold.Location = new System.Drawing.Point(199, 15);
            this.textBoxTreshold.Name = "textBoxTreshold";
            this.textBoxTreshold.Size = new System.Drawing.Size(66, 20);
            this.textBoxTreshold.TabIndex = 4;
            this.textBoxTreshold.Text = "20";
            // 
            // labelTresh
            // 
            this.labelTresh.AutoSize = true;
            this.labelTresh.Location = new System.Drawing.Point(103, 18);
            this.labelTresh.Name = "labelTresh";
            this.labelTresh.Size = new System.Drawing.Size(90, 13);
            this.labelTresh.TabIndex = 5;
            this.labelTresh.Text = "Treshold F.A.S.T.";
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(290, 18);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(35, 13);
            this.labelWidth.TabIndex = 9;
            this.labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(429, 17);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(38, 13);
            this.labelHeight.TabIndex = 10;
            this.labelHeight.Text = "Height";
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.Location = new System.Drawing.Point(331, 14);
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(71, 20);
            this.textBoxWidth.TabIndex = 11;
            this.textBoxWidth.Text = "1280";
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Location = new System.Drawing.Point(473, 14);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(71, 20);
            this.textBoxHeight.TabIndex = 12;
            this.textBoxHeight.Text = "720";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 265);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Blue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "K";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Add";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(166, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Праг";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(550, 16);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 17);
            this.checkBox1.TabIndex = 51;
            this.checkBox1.Text = "Filter";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(453, 54);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(115, 17);
            this.radioButton1.TabIndex = 53;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "[0] Virtual Webcam";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(453, 71);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(124, 17);
            this.radioButton2.TabIndex = 54;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "[1] Microsoft LifeCam";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(232, 265);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 55;
            this.label9.Text = "Yellow";
            // 
            // buttonScreenshots
            // 
            this.buttonScreenshots.Location = new System.Drawing.Point(472, 326);
            this.buttonScreenshots.Name = "buttonScreenshots";
            this.buttonScreenshots.Size = new System.Drawing.Size(126, 23);
            this.buttonScreenshots.TabIndex = 59;
            this.buttonScreenshots.Text = "Take Screenshots";
            this.buttonScreenshots.UseVisualStyleBackColor = true;
            this.buttonScreenshots.Click += new System.EventHandler(this.buttonScreenshots_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(392, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 62;
            this.label7.Text = "Праг";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(341, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "Add";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(290, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 60;
            this.label10.Text = "K";
            // 
            // buttonLoadFrame
            // 
            this.buttonLoadFrame.Location = new System.Drawing.Point(473, 297);
            this.buttonLoadFrame.Name = "buttonLoadFrame";
            this.buttonLoadFrame.Size = new System.Drawing.Size(125, 23);
            this.buttonLoadFrame.TabIndex = 63;
            this.buttonLoadFrame.Text = "Load Frame";
            this.buttonLoadFrame.UseVisualStyleBackColor = true;
            this.buttonLoadFrame.Click += new System.EventHandler(this.buttonLoadFrame_Click);
            // 
            // checkBoxVizDetectedSpots
            // 
            this.checkBoxVizDetectedSpots.AutoSize = true;
            this.checkBoxVizDetectedSpots.Checked = true;
            this.checkBoxVizDetectedSpots.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVizDetectedSpots.Location = new System.Drawing.Point(453, 112);
            this.checkBoxVizDetectedSpots.Name = "checkBoxVizDetectedSpots";
            this.checkBoxVizDetectedSpots.Size = new System.Drawing.Size(140, 17);
            this.checkBoxVizDetectedSpots.TabIndex = 64;
            this.checkBoxVizDetectedSpots.Text = "Visualize detected spots";
            this.checkBoxVizDetectedSpots.UseVisualStyleBackColor = true;
            // 
            // checkBoxWriteDetected
            // 
            this.checkBoxWriteDetected.AutoSize = true;
            this.checkBoxWriteDetected.Location = new System.Drawing.Point(453, 130);
            this.checkBoxWriteDetected.Name = "checkBoxWriteDetected";
            this.checkBoxWriteDetected.Size = new System.Drawing.Size(151, 17);
            this.checkBoxWriteDetected.TabIndex = 65;
            this.checkBoxWriteDetected.Text = "Draw detected spots in file";
            this.checkBoxWriteDetected.UseVisualStyleBackColor = true;
            // 
            // trbY_Prag
            // 
            this.trbY_Prag.Increment = 2F;
            this.trbY_Prag.LargeChange = 64;
            this.trbY_Prag.Location = new System.Drawing.Point(392, 223);
            this.trbY_Prag.MaxVal = 64;
            this.trbY_Prag.MinVal = 1;
            this.trbY_Prag.Name = "trbY_Prag";
            this.trbY_Prag.Size = new System.Drawing.Size(41, 126);
            this.trbY_Prag.SmallChange = 64;
            this.trbY_Prag.TabIndex = 58;
            this.trbY_Prag.Value = 42D;
            // 
            // trbY_Add
            // 
            this.trbY_Add.Increment = 2F;
            this.trbY_Add.LargeChange = 64;
            this.trbY_Add.Location = new System.Drawing.Point(341, 223);
            this.trbY_Add.MaxVal = 64;
            this.trbY_Add.MinVal = 1;
            this.trbY_Add.Name = "trbY_Add";
            this.trbY_Add.Size = new System.Drawing.Size(41, 126);
            this.trbY_Add.SmallChange = 64;
            this.trbY_Add.TabIndex = 57;
            this.trbY_Add.Value = 32D;
            // 
            // trbY_K
            // 
            this.trbY_K.Increment = 0.1F;
            this.trbY_K.LargeChange = 5;
            this.trbY_K.Location = new System.Drawing.Point(284, 223);
            this.trbY_K.MaxVal = 15;
            this.trbY_K.MinVal = 5;
            this.trbY_K.Name = "trbY_K";
            this.trbY_K.Size = new System.Drawing.Size(41, 126);
            this.trbY_K.SmallChange = 1;
            this.trbY_K.TabIndex = 56;
            this.trbY_K.Value = 0.5D;
            // 
            // trbB_Prag
            // 
            this.trbB_Prag.Increment = 2F;
            this.trbB_Prag.LargeChange = 64;
            this.trbB_Prag.Location = new System.Drawing.Point(173, 223);
            this.trbB_Prag.MaxVal = 64;
            this.trbB_Prag.MinVal = 1;
            this.trbB_Prag.Name = "trbB_Prag";
            this.trbB_Prag.Size = new System.Drawing.Size(41, 126);
            this.trbB_Prag.SmallChange = 64;
            this.trbB_Prag.TabIndex = 47;
            this.trbB_Prag.Value = 16D;
            // 
            // trbB_Add
            // 
            this.trbB_Add.Increment = 2F;
            this.trbB_Add.LargeChange = 64;
            this.trbB_Add.Location = new System.Drawing.Point(122, 223);
            this.trbB_Add.MaxVal = 64;
            this.trbB_Add.MinVal = 1;
            this.trbB_Add.Name = "trbB_Add";
            this.trbB_Add.Size = new System.Drawing.Size(41, 126);
            this.trbB_Add.SmallChange = 64;
            this.trbB_Add.TabIndex = 46;
            this.trbB_Add.Value = 16D;
            // 
            // trbB_K
            // 
            this.trbB_K.Increment = 0.1F;
            this.trbB_K.LargeChange = 5;
            this.trbB_K.Location = new System.Drawing.Point(65, 223);
            this.trbB_K.MaxVal = 15;
            this.trbB_K.MinVal = 5;
            this.trbB_K.Name = "trbB_K";
            this.trbB_K.Size = new System.Drawing.Size(41, 126);
            this.trbB_K.SmallChange = 1;
            this.trbB_K.TabIndex = 45;
            this.trbB_K.Value = 0.5D;
            // 
            // trbG_Prag
            // 
            this.trbG_Prag.Increment = 2F;
            this.trbG_Prag.LargeChange = 64;
            this.trbG_Prag.Location = new System.Drawing.Point(392, 85);
            this.trbG_Prag.MaxVal = 64;
            this.trbG_Prag.MinVal = 1;
            this.trbG_Prag.Name = "trbG_Prag";
            this.trbG_Prag.Size = new System.Drawing.Size(41, 126);
            this.trbG_Prag.SmallChange = 64;
            this.trbG_Prag.TabIndex = 44;
            this.trbG_Prag.Value = 16D;
            // 
            // trbG_Add
            // 
            this.trbG_Add.Increment = 2F;
            this.trbG_Add.LargeChange = 64;
            this.trbG_Add.Location = new System.Drawing.Point(341, 85);
            this.trbG_Add.MaxVal = 64;
            this.trbG_Add.MinVal = 1;
            this.trbG_Add.Name = "trbG_Add";
            this.trbG_Add.Size = new System.Drawing.Size(41, 126);
            this.trbG_Add.SmallChange = 64;
            this.trbG_Add.TabIndex = 43;
            this.trbG_Add.Value = 16D;
            // 
            // trbG_K
            // 
            this.trbG_K.Increment = 0.1F;
            this.trbG_K.LargeChange = 5;
            this.trbG_K.Location = new System.Drawing.Point(284, 85);
            this.trbG_K.MaxVal = 15;
            this.trbG_K.MinVal = 5;
            this.trbG_K.Name = "trbG_K";
            this.trbG_K.Size = new System.Drawing.Size(41, 126);
            this.trbG_K.SmallChange = 1;
            this.trbG_K.TabIndex = 42;
            this.trbG_K.Value = 0.5D;
            // 
            // trbR_Prag
            // 
            this.trbR_Prag.Increment = 2F;
            this.trbR_Prag.LargeChange = 64;
            this.trbR_Prag.Location = new System.Drawing.Point(169, 85);
            this.trbR_Prag.MaxVal = 64;
            this.trbR_Prag.MinVal = 1;
            this.trbR_Prag.Name = "trbR_Prag";
            this.trbR_Prag.Size = new System.Drawing.Size(41, 126);
            this.trbR_Prag.SmallChange = 64;
            this.trbR_Prag.TabIndex = 41;
            this.trbR_Prag.Value = 16D;
            // 
            // trbR_Add
            // 
            this.trbR_Add.Increment = 2F;
            this.trbR_Add.LargeChange = 64;
            this.trbR_Add.Location = new System.Drawing.Point(118, 85);
            this.trbR_Add.MaxVal = 64;
            this.trbR_Add.MinVal = 1;
            this.trbR_Add.Name = "trbR_Add";
            this.trbR_Add.Size = new System.Drawing.Size(41, 126);
            this.trbR_Add.SmallChange = 64;
            this.trbR_Add.TabIndex = 40;
            this.trbR_Add.Value = 16D;
            // 
            // trbR_K
            // 
            this.trbR_K.Increment = 0.1F;
            this.trbR_K.LargeChange = 5;
            this.trbR_K.Location = new System.Drawing.Point(61, 85);
            this.trbR_K.MaxVal = 15;
            this.trbR_K.MinVal = 5;
            this.trbR_K.Name = "trbR_K";
            this.trbR_K.Size = new System.Drawing.Size(41, 126);
            this.trbR_K.SmallChange = 1;
            this.trbR_K.TabIndex = 39;
            this.trbR_K.Value = 1D;
            // 
            // buttonCalibrate
            // 
            this.buttonCalibrate.Location = new System.Drawing.Point(473, 265);
            this.buttonCalibrate.Name = "buttonCalibrate";
            this.buttonCalibrate.Size = new System.Drawing.Size(125, 23);
            this.buttonCalibrate.TabIndex = 66;
            this.buttonCalibrate.Text = "Calibrate";
            this.buttonCalibrate.UseVisualStyleBackColor = true;
            this.buttonCalibrate.Click += new System.EventHandler(this.buttonCalibrateCenter_Click);
            // 
            // FormWebCamEmgu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 358);
            this.Controls.Add(this.buttonCalibrate);
            this.Controls.Add(this.checkBoxWriteDetected);
            this.Controls.Add(this.checkBoxVizDetectedSpots);
            this.Controls.Add(this.buttonLoadFrame);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonScreenshots);
            this.Controls.Add(this.trbY_Prag);
            this.Controls.Add(this.trbY_Add);
            this.Controls.Add(this.trbY_K);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.trbB_Prag);
            this.Controls.Add(this.trbB_Add);
            this.Controls.Add(this.trbB_K);
            this.Controls.Add(this.trbG_Prag);
            this.Controls.Add(this.trbG_Add);
            this.Controls.Add(this.trbG_K);
            this.Controls.Add(this.trbR_Prag);
            this.Controls.Add(this.trbR_Add);
            this.Controls.Add(this.trbR_K);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHeight);
            this.Controls.Add(this.textBoxWidth);
            this.Controls.Add(this.labelHeight);
            this.Controls.Add(this.labelWidth);
            this.Controls.Add(this.labelTresh);
            this.Controls.Add(this.textBoxTreshold);
            this.Controls.Add(this.buttonCapture);
            this.Location = new System.Drawing.Point(1, 1);
            this.Name = "FormWebCamEmgu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormEmguWebCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWebCamEmgu_FormClosing);
            this.Load += new System.EventHandler(this.FormWebCamEmgu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.TextBox textBoxTreshold;
        private System.Windows.Forms.Label labelTresh;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.TextBox textBoxWidth;
        private System.Windows.Forms.TextBox textBoxHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private MyTrackBar trbR_K;
        private MyTrackBar trbR_Add;
        private MyTrackBar trbR_Prag;
        private MyTrackBar trbG_Prag;
        private MyTrackBar trbG_Add;
        private MyTrackBar trbG_K;
        private MyTrackBar trbB_Prag;
        private MyTrackBar trbB_Add;
        private MyTrackBar trbB_K;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private MyTrackBar trbY_Prag;
        private MyTrackBar trbY_Add;
        private MyTrackBar trbY_K;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonScreenshots;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonLoadFrame;
        private System.Windows.Forms.CheckBox checkBoxVizDetectedSpots;
        private System.Windows.Forms.CheckBox checkBoxWriteDetected;
        private System.Windows.Forms.Button buttonCalibrate;
    }
}