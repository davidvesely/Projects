namespace SixDoFMouse
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

        private void buttonScreenshots_Click(object sender, System.EventArgs e)
        {
            SaveScreenshots();
        }

        private void buttonLoadFrame_Click(object sender, System.EventArgs e)
        {
            
        }

        private void buttonCapture_Click(object sender, System.EventArgs e)
        {
            ResetCapturing();
        }

        private void trbR_K_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_R_K = trbR_K.Value;
        }

        private void trbR_Add_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_R_ADD = (int)trbR_Add.Value;
        }

        private void trbR_Prag_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_R_PRAG = (int)trbR_Prag.Value;
        }

        private void trbG_K_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_G_K = trbG_K.Value;
        }

        private void trbG_Add_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_G_ADD = (int)trbG_Add.Value;
        }

        private void trbG_Prag_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_G_PRAG = (int)trbG_Prag.Value;
        }

        private void trbB_K_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_B_K = trbB_K.Value;
        }

        private void trbB_Add_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_B_ADD = (int)trbB_Add.Value;
        }

        private void trbB_Prag_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_B_PRAG = (int)trbB_Prag.Value;
        }

        private void trbY_K_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_Y_K = trbY_K.Value;
        }

        private void trbY_Add_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_Y_ADD = (int)trbY_Add.Value;
        }

        private void trbY_Prag_MyScroll(object sender, System.EventArgs e)
        {
            Coef.IM_Y_PRAG = (int)trbY_Prag.Value;
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
            this.radioButtonDevice1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDevice2 = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonScreenshots = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonLoadFrame = new System.Windows.Forms.Button();
            this.checkBoxShowTextViz = new System.Windows.Forms.CheckBox();
            this.checkBoxTopFlickering = new System.Windows.Forms.CheckBox();
            this.buttonCalibrate = new System.Windows.Forms.Button();
            this.trackBarBrightness = new System.Windows.Forms.TrackBar();
            this.labelBrightness = new System.Windows.Forms.Label();
            this.checkBoxDrawViewPoint = new System.Windows.Forms.CheckBox();
            this.checkBoxCharts = new System.Windows.Forms.CheckBox();
            this.checkBoxSendControlPars = new System.Windows.Forms.CheckBox();
            this.checkBoxPan = new System.Windows.Forms.CheckBox();
            this.trackBarHDown = new System.Windows.Forms.TrackBar();
            this.checkBoxOldFiltration = new System.Windows.Forms.CheckBox();
            this.labelHDown = new System.Windows.Forms.Label();
            this.labelHUp = new System.Windows.Forms.Label();
            this.trackBarHUp = new System.Windows.Forms.TrackBar();
            this.labelSDown = new System.Windows.Forms.Label();
            this.trackBarSDown = new System.Windows.Forms.TrackBar();
            this.labelSUp = new System.Windows.Forms.Label();
            this.trackBarSUp = new System.Windows.Forms.TrackBar();
            this.buttonGetWhiteColor = new System.Windows.Forms.Button();
            this.trackBarBlack = new System.Windows.Forms.TrackBar();
            this.trackBarWhite = new System.Windows.Forms.TrackBar();
            this.labelUpLevel = new System.Windows.Forms.Label();
            this.labelDownLevel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.trbY_Prag = new SixDoFMouse.MyTrackBar();
            this.trbY_Add = new SixDoFMouse.MyTrackBar();
            this.trbY_K = new SixDoFMouse.MyTrackBar();
            this.trbB_Prag = new SixDoFMouse.MyTrackBar();
            this.trbB_Add = new SixDoFMouse.MyTrackBar();
            this.trbB_K = new SixDoFMouse.MyTrackBar();
            this.trbG_Prag = new SixDoFMouse.MyTrackBar();
            this.trbG_Add = new SixDoFMouse.MyTrackBar();
            this.trbG_K = new SixDoFMouse.MyTrackBar();
            this.trbR_Prag = new SixDoFMouse.MyTrackBar();
            this.trbR_Add = new SixDoFMouse.MyTrackBar();
            this.trbR_K = new SixDoFMouse.MyTrackBar();
            this.radioButtonRed = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonYellow = new System.Windows.Forms.RadioButton();
            this.radioButtonBlue = new System.Windows.Forms.RadioButton();
            this.radioButtonGreen = new System.Windows.Forms.RadioButton();
            this.buttonDrawHomography = new System.Windows.Forms.Button();
            this.checkBoxWeight2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWhite)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCapture
            // 
            this.buttonCapture.Location = new System.Drawing.Point(16, 15);
            this.buttonCapture.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(100, 28);
            this.buttonCapture.TabIndex = 3;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // textBoxTreshold
            // 
            this.textBoxTreshold.Location = new System.Drawing.Point(265, 18);
            this.textBoxTreshold.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxTreshold.Name = "textBoxTreshold";
            this.textBoxTreshold.Size = new System.Drawing.Size(61, 22);
            this.textBoxTreshold.TabIndex = 4;
            this.textBoxTreshold.Text = "20";
            this.textBoxTreshold.Visible = false;
            // 
            // labelTresh
            // 
            this.labelTresh.AutoSize = true;
            this.labelTresh.Location = new System.Drawing.Point(137, 22);
            this.labelTresh.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTresh.Name = "labelTresh";
            this.labelTresh.Size = new System.Drawing.Size(119, 17);
            this.labelTresh.TabIndex = 5;
            this.labelTresh.Text = "Treshold F.A.S.T.";
            this.labelTresh.Visible = false;
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(387, 22);
            this.labelWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(44, 17);
            this.labelWidth.TabIndex = 9;
            this.labelWidth.Text = "Width";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(572, 21);
            this.labelHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(49, 17);
            this.labelHeight.TabIndex = 10;
            this.labelHeight.Text = "Height";
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.Location = new System.Drawing.Point(441, 17);
            this.textBoxWidth.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(93, 22);
            this.textBoxWidth.TabIndex = 11;
            this.textBoxWidth.Text = "800";
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Location = new System.Drawing.Point(631, 17);
            this.textBoxHeight.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(93, 22);
            this.textBoxHeight.TabIndex = 12;
            this.textBoxHeight.Text = "600";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 156);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 22;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(305, 156);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 326);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 17);
            this.label3.TabIndex = 24;
            this.label3.Text = "Blue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 82);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 17);
            this.label4.TabIndex = 25;
            this.label4.Text = "K";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(153, 82);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 17);
            this.label5.TabIndex = 26;
            this.label5.Text = "Add";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(221, 82);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "Праг";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(733, 20);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 21);
            this.checkBox1.TabIndex = 51;
            this.checkBox1.Text = "Filter";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // radioButtonDevice1
            // 
            this.radioButtonDevice1.AutoSize = true;
            this.radioButtonDevice1.Location = new System.Drawing.Point(595, 78);
            this.radioButtonDevice1.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDevice1.Name = "radioButtonDevice1";
            this.radioButtonDevice1.Size = new System.Drawing.Size(148, 21);
            this.radioButtonDevice1.TabIndex = 53;
            this.radioButtonDevice1.Text = "[0] Virtual Webcam";
            this.radioButtonDevice1.UseVisualStyleBackColor = true;
            // 
            // radioButtonDevice2
            // 
            this.radioButtonDevice2.AutoSize = true;
            this.radioButtonDevice2.Checked = true;
            this.radioButtonDevice2.Location = new System.Drawing.Point(595, 98);
            this.radioButtonDevice2.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDevice2.Name = "radioButtonDevice2";
            this.radioButtonDevice2.Size = new System.Drawing.Size(161, 21);
            this.radioButtonDevice2.TabIndex = 54;
            this.radioButtonDevice2.TabStop = true;
            this.radioButtonDevice2.Text = "[1] Microsoft LifeCam";
            this.radioButtonDevice2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(309, 326);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 17);
            this.label9.TabIndex = 55;
            this.label9.Text = "Yellow";
            // 
            // buttonScreenshots
            // 
            this.buttonScreenshots.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScreenshots.Location = new System.Drawing.Point(708, 715);
            this.buttonScreenshots.Margin = new System.Windows.Forms.Padding(4);
            this.buttonScreenshots.Name = "buttonScreenshots";
            this.buttonScreenshots.Size = new System.Drawing.Size(168, 28);
            this.buttonScreenshots.TabIndex = 59;
            this.buttonScreenshots.Text = "Take Screenshots";
            this.buttonScreenshots.UseVisualStyleBackColor = true;
            this.buttonScreenshots.Click += new System.EventHandler(this.buttonScreenshots_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(523, 82);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 17);
            this.label7.TabIndex = 62;
            this.label7.Text = "Праг";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(455, 82);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 17);
            this.label8.TabIndex = 61;
            this.label8.Text = "Add";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(387, 82);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 17);
            this.label10.TabIndex = 60;
            this.label10.Text = "K";
            // 
            // buttonLoadFrame
            // 
            this.buttonLoadFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadFrame.Location = new System.Drawing.Point(709, 679);
            this.buttonLoadFrame.Margin = new System.Windows.Forms.Padding(4);
            this.buttonLoadFrame.Name = "buttonLoadFrame";
            this.buttonLoadFrame.Size = new System.Drawing.Size(167, 28);
            this.buttonLoadFrame.TabIndex = 63;
            this.buttonLoadFrame.Text = "Load Frame";
            this.buttonLoadFrame.UseVisualStyleBackColor = true;
            this.buttonLoadFrame.Click += new System.EventHandler(this.buttonLoadFrame_Click);
            // 
            // checkBoxShowTextViz
            // 
            this.checkBoxShowTextViz.AutoSize = true;
            this.checkBoxShowTextViz.Location = new System.Drawing.Point(595, 149);
            this.checkBoxShowTextViz.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxShowTextViz.Name = "checkBoxShowTextViz";
            this.checkBoxShowTextViz.Size = new System.Drawing.Size(153, 21);
            this.checkBoxShowTextViz.TabIndex = 64;
            this.checkBoxShowTextViz.Text = "Show text visualizer";
            this.checkBoxShowTextViz.UseVisualStyleBackColor = true;
            this.checkBoxShowTextViz.CheckedChanged += new System.EventHandler(this.checkBoxShowTextViz_CheckedChanged);
            // 
            // checkBoxTopFlickering
            // 
            this.checkBoxTopFlickering.AutoSize = true;
            this.checkBoxTopFlickering.Location = new System.Drawing.Point(595, 174);
            this.checkBoxTopFlickering.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxTopFlickering.Name = "checkBoxTopFlickering";
            this.checkBoxTopFlickering.Size = new System.Drawing.Size(186, 21);
            this.checkBoxTopFlickering.TabIndex = 65;
            this.checkBoxTopFlickering.Text = "Stop descriptor flickering";
            this.checkBoxTopFlickering.UseVisualStyleBackColor = true;
            // 
            // buttonCalibrate
            // 
            this.buttonCalibrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalibrate.Location = new System.Drawing.Point(709, 640);
            this.buttonCalibrate.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCalibrate.Name = "buttonCalibrate";
            this.buttonCalibrate.Size = new System.Drawing.Size(167, 28);
            this.buttonCalibrate.TabIndex = 66;
            this.buttonCalibrate.Text = "Calibrate";
            this.buttonCalibrate.UseVisualStyleBackColor = true;
            this.buttonCalibrate.Click += new System.EventHandler(this.buttonCalibrateCenter_Click);
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.Location = new System.Drawing.Point(591, 348);
            this.trackBarBrightness.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarBrightness.Maximum = 15;
            this.trackBarBrightness.Minimum = -15;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.Size = new System.Drawing.Size(193, 56);
            this.trackBarBrightness.TabIndex = 67;
            this.trackBarBrightness.Value = -8;
            this.trackBarBrightness.Scroll += new System.EventHandler(this.trackBarBrightness_Scroll);
            // 
            // labelBrightness
            // 
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(627, 326);
            this.labelBrightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(75, 17);
            this.labelBrightness.TabIndex = 68;
            this.labelBrightness.Text = "Brightness";
            // 
            // checkBoxDrawViewPoint
            // 
            this.checkBoxDrawViewPoint.AutoSize = true;
            this.checkBoxDrawViewPoint.Location = new System.Drawing.Point(595, 198);
            this.checkBoxDrawViewPoint.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxDrawViewPoint.Name = "checkBoxDrawViewPoint";
            this.checkBoxDrawViewPoint.Size = new System.Drawing.Size(187, 21);
            this.checkBoxDrawViewPoint.TabIndex = 69;
            this.checkBoxDrawViewPoint.Text = "Draw View Point in ACAD";
            this.checkBoxDrawViewPoint.UseVisualStyleBackColor = true;
            // 
            // checkBoxCharts
            // 
            this.checkBoxCharts.AutoSize = true;
            this.checkBoxCharts.Location = new System.Drawing.Point(595, 127);
            this.checkBoxCharts.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxCharts.Name = "checkBoxCharts";
            this.checkBoxCharts.Size = new System.Drawing.Size(109, 21);
            this.checkBoxCharts.TabIndex = 71;
            this.checkBoxCharts.Text = "Show Charts";
            this.checkBoxCharts.UseVisualStyleBackColor = true;
            this.checkBoxCharts.CheckedChanged += new System.EventHandler(this.checkBoxCharts_CheckedChanged);
            // 
            // checkBoxSendControlPars
            // 
            this.checkBoxSendControlPars.AutoSize = true;
            this.checkBoxSendControlPars.Location = new System.Drawing.Point(595, 222);
            this.checkBoxSendControlPars.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxSendControlPars.Name = "checkBoxSendControlPars";
            this.checkBoxSendControlPars.Size = new System.Drawing.Size(189, 21);
            this.checkBoxSendControlPars.TabIndex = 72;
            this.checkBoxSendControlPars.Text = "Send Control Parameters";
            this.checkBoxSendControlPars.UseVisualStyleBackColor = true;
            // 
            // checkBoxPan
            // 
            this.checkBoxPan.AutoSize = true;
            this.checkBoxPan.Checked = true;
            this.checkBoxPan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPan.Location = new System.Drawing.Point(595, 244);
            this.checkBoxPan.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxPan.Name = "checkBoxPan";
            this.checkBoxPan.Size = new System.Drawing.Size(55, 21);
            this.checkBoxPan.TabIndex = 72;
            this.checkBoxPan.Text = "Pan";
            this.checkBoxPan.UseVisualStyleBackColor = true;
            // 
            // trackBarHDown
            // 
            this.trackBarHDown.Location = new System.Drawing.Point(56, 444);
            this.trackBarHDown.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarHDown.Maximum = 255;
            this.trackBarHDown.Name = "trackBarHDown";
            this.trackBarHDown.Size = new System.Drawing.Size(556, 56);
            this.trackBarHDown.TabIndex = 73;
            this.trackBarHDown.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // checkBoxOldFiltration
            // 
            this.checkBoxOldFiltration.AutoSize = true;
            this.checkBoxOldFiltration.Location = new System.Drawing.Point(595, 266);
            this.checkBoxOldFiltration.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxOldFiltration.Name = "checkBoxOldFiltration";
            this.checkBoxOldFiltration.Size = new System.Drawing.Size(132, 21);
            this.checkBoxOldFiltration.TabIndex = 74;
            this.checkBoxOldFiltration.Text = "Use old filtration";
            this.checkBoxOldFiltration.UseVisualStyleBackColor = true;
            // 
            // labelHDown
            // 
            this.labelHDown.AutoSize = true;
            this.labelHDown.Location = new System.Drawing.Point(5, 449);
            this.labelHDown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHDown.Name = "labelHDown";
            this.labelHDown.Size = new System.Drawing.Size(16, 17);
            this.labelHDown.TabIndex = 75;
            this.labelHDown.Text = "0";
            // 
            // labelHUp
            // 
            this.labelHUp.AutoSize = true;
            this.labelHUp.Location = new System.Drawing.Point(5, 495);
            this.labelHUp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelHUp.Name = "labelHUp";
            this.labelHUp.Size = new System.Drawing.Size(16, 17);
            this.labelHUp.TabIndex = 77;
            this.labelHUp.Text = "0";
            // 
            // trackBarHUp
            // 
            this.trackBarHUp.Location = new System.Drawing.Point(56, 490);
            this.trackBarHUp.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarHUp.Maximum = 255;
            this.trackBarHUp.Name = "trackBarHUp";
            this.trackBarHUp.Size = new System.Drawing.Size(556, 56);
            this.trackBarHUp.TabIndex = 76;
            this.trackBarHUp.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelSDown
            // 
            this.labelSDown.AutoSize = true;
            this.labelSDown.Location = new System.Drawing.Point(5, 543);
            this.labelSDown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSDown.Name = "labelSDown";
            this.labelSDown.Size = new System.Drawing.Size(16, 17);
            this.labelSDown.TabIndex = 79;
            this.labelSDown.Text = "0";
            // 
            // trackBarSDown
            // 
            this.trackBarSDown.Location = new System.Drawing.Point(51, 538);
            this.trackBarSDown.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarSDown.Maximum = 255;
            this.trackBarSDown.Name = "trackBarSDown";
            this.trackBarSDown.Size = new System.Drawing.Size(556, 56);
            this.trackBarSDown.TabIndex = 78;
            this.trackBarSDown.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelSUp
            // 
            this.labelSUp.AutoSize = true;
            this.labelSUp.Location = new System.Drawing.Point(5, 591);
            this.labelSUp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSUp.Name = "labelSUp";
            this.labelSUp.Size = new System.Drawing.Size(16, 17);
            this.labelSUp.TabIndex = 81;
            this.labelSUp.Text = "0";
            // 
            // trackBarSUp
            // 
            this.trackBarSUp.Location = new System.Drawing.Point(49, 586);
            this.trackBarSUp.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarSUp.Maximum = 255;
            this.trackBarSUp.Name = "trackBarSUp";
            this.trackBarSUp.Size = new System.Drawing.Size(556, 56);
            this.trackBarSUp.TabIndex = 80;
            this.trackBarSUp.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // buttonGetWhiteColor
            // 
            this.buttonGetWhiteColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGetWhiteColor.Location = new System.Drawing.Point(709, 604);
            this.buttonGetWhiteColor.Margin = new System.Windows.Forms.Padding(4);
            this.buttonGetWhiteColor.Name = "buttonGetWhiteColor";
            this.buttonGetWhiteColor.Size = new System.Drawing.Size(167, 28);
            this.buttonGetWhiteColor.TabIndex = 82;
            this.buttonGetWhiteColor.Text = "Get White";
            this.buttonGetWhiteColor.UseVisualStyleBackColor = true;
            this.buttonGetWhiteColor.Click += new System.EventHandler(this.buttonGetWhiteColor_Click);
            // 
            // trackBarBlack
            // 
            this.trackBarBlack.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarBlack.Location = new System.Drawing.Point(51, 640);
            this.trackBarBlack.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarBlack.Maximum = 255;
            this.trackBarBlack.Name = "trackBarBlack";
            this.trackBarBlack.Size = new System.Drawing.Size(556, 56);
            this.trackBarBlack.TabIndex = 84;
            this.trackBarBlack.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBarWhite
            // 
            this.trackBarWhite.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarWhite.Location = new System.Drawing.Point(49, 703);
            this.trackBarWhite.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarWhite.Maximum = 255;
            this.trackBarWhite.Name = "trackBarWhite";
            this.trackBarWhite.Size = new System.Drawing.Size(556, 56);
            this.trackBarWhite.TabIndex = 83;
            this.trackBarWhite.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelUpLevel
            // 
            this.labelUpLevel.AutoSize = true;
            this.labelUpLevel.Location = new System.Drawing.Point(8, 706);
            this.labelUpLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUpLevel.Name = "labelUpLevel";
            this.labelUpLevel.Size = new System.Drawing.Size(16, 17);
            this.labelUpLevel.TabIndex = 85;
            this.labelUpLevel.Text = "0";
            // 
            // labelDownLevel
            // 
            this.labelDownLevel.AutoSize = true;
            this.labelDownLevel.Location = new System.Drawing.Point(5, 652);
            this.labelDownLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDownLevel.Name = "labelDownLevel";
            this.labelDownLevel.Size = new System.Drawing.Size(16, 17);
            this.labelDownLevel.TabIndex = 86;
            this.labelDownLevel.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(615, 593);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 17);
            this.label15.TabIndex = 90;
            this.label15.Text = "S Up";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(615, 545);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 17);
            this.label16.TabIndex = 89;
            this.label16.Text = "S Down";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(615, 497);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 17);
            this.label17.TabIndex = 88;
            this.label17.Text = "H Up";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(615, 452);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(57, 17);
            this.label18.TabIndex = 87;
            this.label18.Text = "H Down";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(615, 709);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(39, 17);
            this.label19.TabIndex = 92;
            this.label19.Text = "V Up";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(615, 642);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 17);
            this.label20.TabIndex = 91;
            this.label20.Text = "V Down";
            // 
            // trbY_Prag
            // 
            this.trbY_Prag.Increment = 2F;
            this.trbY_Prag.LargeChange = 64;
            this.trbY_Prag.Location = new System.Drawing.Point(523, 274);
            this.trbY_Prag.Margin = new System.Windows.Forms.Padding(5);
            this.trbY_Prag.MaxVal = 64;
            this.trbY_Prag.MinVal = 1;
            this.trbY_Prag.Name = "trbY_Prag";
            this.trbY_Prag.Size = new System.Drawing.Size(55, 155);
            this.trbY_Prag.SmallChange = 64;
            this.trbY_Prag.TabIndex = 58;
            this.trbY_Prag.Value = 42D;
            this.trbY_Prag.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbY_Prag_MyScroll);
            // 
            // trbY_Add
            // 
            this.trbY_Add.Increment = 2F;
            this.trbY_Add.LargeChange = 64;
            this.trbY_Add.Location = new System.Drawing.Point(455, 274);
            this.trbY_Add.Margin = new System.Windows.Forms.Padding(5);
            this.trbY_Add.MaxVal = 64;
            this.trbY_Add.MinVal = 1;
            this.trbY_Add.Name = "trbY_Add";
            this.trbY_Add.Size = new System.Drawing.Size(55, 155);
            this.trbY_Add.SmallChange = 64;
            this.trbY_Add.TabIndex = 57;
            this.trbY_Add.Value = 32D;
            this.trbY_Add.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbY_Add_MyScroll);
            // 
            // trbY_K
            // 
            this.trbY_K.Increment = 0.1F;
            this.trbY_K.LargeChange = 5;
            this.trbY_K.Location = new System.Drawing.Point(379, 274);
            this.trbY_K.Margin = new System.Windows.Forms.Padding(5);
            this.trbY_K.MaxVal = 15;
            this.trbY_K.MinVal = 5;
            this.trbY_K.Name = "trbY_K";
            this.trbY_K.Size = new System.Drawing.Size(55, 155);
            this.trbY_K.SmallChange = 1;
            this.trbY_K.TabIndex = 56;
            this.trbY_K.Value = 0.5D;
            this.trbY_K.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbY_K_MyScroll);
            // 
            // trbB_Prag
            // 
            this.trbB_Prag.Increment = 2F;
            this.trbB_Prag.LargeChange = 64;
            this.trbB_Prag.Location = new System.Drawing.Point(231, 274);
            this.trbB_Prag.Margin = new System.Windows.Forms.Padding(5);
            this.trbB_Prag.MaxVal = 64;
            this.trbB_Prag.MinVal = 1;
            this.trbB_Prag.Name = "trbB_Prag";
            this.trbB_Prag.Size = new System.Drawing.Size(55, 155);
            this.trbB_Prag.SmallChange = 64;
            this.trbB_Prag.TabIndex = 47;
            this.trbB_Prag.Value = 16D;
            this.trbB_Prag.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbB_Prag_MyScroll);
            // 
            // trbB_Add
            // 
            this.trbB_Add.Increment = 2F;
            this.trbB_Add.LargeChange = 64;
            this.trbB_Add.Location = new System.Drawing.Point(163, 274);
            this.trbB_Add.Margin = new System.Windows.Forms.Padding(5);
            this.trbB_Add.MaxVal = 64;
            this.trbB_Add.MinVal = 1;
            this.trbB_Add.Name = "trbB_Add";
            this.trbB_Add.Size = new System.Drawing.Size(55, 155);
            this.trbB_Add.SmallChange = 64;
            this.trbB_Add.TabIndex = 46;
            this.trbB_Add.Value = 16D;
            this.trbB_Add.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbB_Add_MyScroll);
            // 
            // trbB_K
            // 
            this.trbB_K.Increment = 0.1F;
            this.trbB_K.LargeChange = 5;
            this.trbB_K.Location = new System.Drawing.Point(87, 274);
            this.trbB_K.Margin = new System.Windows.Forms.Padding(5);
            this.trbB_K.MaxVal = 15;
            this.trbB_K.MinVal = 5;
            this.trbB_K.Name = "trbB_K";
            this.trbB_K.Size = new System.Drawing.Size(55, 155);
            this.trbB_K.SmallChange = 1;
            this.trbB_K.TabIndex = 45;
            this.trbB_K.Value = 0.5D;
            this.trbB_K.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbB_K_MyScroll);
            // 
            // trbG_Prag
            // 
            this.trbG_Prag.Increment = 2F;
            this.trbG_Prag.LargeChange = 64;
            this.trbG_Prag.Location = new System.Drawing.Point(523, 105);
            this.trbG_Prag.Margin = new System.Windows.Forms.Padding(5);
            this.trbG_Prag.MaxVal = 64;
            this.trbG_Prag.MinVal = 1;
            this.trbG_Prag.Name = "trbG_Prag";
            this.trbG_Prag.Size = new System.Drawing.Size(55, 155);
            this.trbG_Prag.SmallChange = 64;
            this.trbG_Prag.TabIndex = 44;
            this.trbG_Prag.Value = 16D;
            this.trbG_Prag.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbG_Prag_MyScroll);
            // 
            // trbG_Add
            // 
            this.trbG_Add.Increment = 2F;
            this.trbG_Add.LargeChange = 64;
            this.trbG_Add.Location = new System.Drawing.Point(455, 105);
            this.trbG_Add.Margin = new System.Windows.Forms.Padding(5);
            this.trbG_Add.MaxVal = 64;
            this.trbG_Add.MinVal = 1;
            this.trbG_Add.Name = "trbG_Add";
            this.trbG_Add.Size = new System.Drawing.Size(55, 155);
            this.trbG_Add.SmallChange = 64;
            this.trbG_Add.TabIndex = 43;
            this.trbG_Add.Value = 16D;
            this.trbG_Add.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbG_Add_MyScroll);
            // 
            // trbG_K
            // 
            this.trbG_K.Increment = 0.1F;
            this.trbG_K.LargeChange = 5;
            this.trbG_K.Location = new System.Drawing.Point(379, 105);
            this.trbG_K.Margin = new System.Windows.Forms.Padding(5);
            this.trbG_K.MaxVal = 15;
            this.trbG_K.MinVal = 5;
            this.trbG_K.Name = "trbG_K";
            this.trbG_K.Size = new System.Drawing.Size(55, 155);
            this.trbG_K.SmallChange = 1;
            this.trbG_K.TabIndex = 42;
            this.trbG_K.Value = 0.5D;
            this.trbG_K.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbG_K_MyScroll);
            // 
            // trbR_Prag
            // 
            this.trbR_Prag.Increment = 2F;
            this.trbR_Prag.LargeChange = 64;
            this.trbR_Prag.Location = new System.Drawing.Point(225, 105);
            this.trbR_Prag.Margin = new System.Windows.Forms.Padding(5);
            this.trbR_Prag.MaxVal = 64;
            this.trbR_Prag.MinVal = 1;
            this.trbR_Prag.Name = "trbR_Prag";
            this.trbR_Prag.Size = new System.Drawing.Size(55, 155);
            this.trbR_Prag.SmallChange = 64;
            this.trbR_Prag.TabIndex = 41;
            this.trbR_Prag.Value = 16D;
            this.trbR_Prag.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbR_Prag_MyScroll);
            // 
            // trbR_Add
            // 
            this.trbR_Add.Increment = 2F;
            this.trbR_Add.LargeChange = 64;
            this.trbR_Add.Location = new System.Drawing.Point(157, 105);
            this.trbR_Add.Margin = new System.Windows.Forms.Padding(5);
            this.trbR_Add.MaxVal = 64;
            this.trbR_Add.MinVal = 1;
            this.trbR_Add.Name = "trbR_Add";
            this.trbR_Add.Size = new System.Drawing.Size(55, 155);
            this.trbR_Add.SmallChange = 64;
            this.trbR_Add.TabIndex = 40;
            this.trbR_Add.Value = 16D;
            this.trbR_Add.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbR_Add_MyScroll);
            // 
            // trbR_K
            // 
            this.trbR_K.Increment = 0.1F;
            this.trbR_K.LargeChange = 5;
            this.trbR_K.Location = new System.Drawing.Point(81, 105);
            this.trbR_K.Margin = new System.Windows.Forms.Padding(5);
            this.trbR_K.MaxVal = 15;
            this.trbR_K.MinVal = 5;
            this.trbR_K.Name = "trbR_K";
            this.trbR_K.Size = new System.Drawing.Size(55, 155);
            this.trbR_K.SmallChange = 1;
            this.trbR_K.TabIndex = 39;
            this.trbR_K.Value = 1D;
            this.trbR_K.MyScroll += new SixDoFMouse.MyScrollEventHandler(this.trbR_K_MyScroll);
            // 
            // radioButtonRed
            // 
            this.radioButtonRed.AutoSize = true;
            this.radioButtonRed.Checked = true;
            this.radioButtonRed.Location = new System.Drawing.Point(8, 23);
            this.radioButtonRed.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonRed.Name = "radioButtonRed";
            this.radioButtonRed.Size = new System.Drawing.Size(55, 21);
            this.radioButtonRed.TabIndex = 93;
            this.radioButtonRed.TabStop = true;
            this.radioButtonRed.Text = "Red";
            this.radioButtonRed.UseVisualStyleBackColor = true;
            this.radioButtonRed.CheckedChanged += new System.EventHandler(this.radioButtonColors_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonYellow);
            this.groupBox1.Controls.Add(this.radioButtonBlue);
            this.groupBox1.Controls.Add(this.radioButtonGreen);
            this.groupBox1.Controls.Add(this.radioButtonRed);
            this.groupBox1.Location = new System.Drawing.Point(733, 411);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(123, 148);
            this.groupBox1.TabIndex = 94;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base Colors";
            // 
            // radioButtonYellow
            // 
            this.radioButtonYellow.AutoSize = true;
            this.radioButtonYellow.Location = new System.Drawing.Point(8, 108);
            this.radioButtonYellow.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonYellow.Name = "radioButtonYellow";
            this.radioButtonYellow.Size = new System.Drawing.Size(69, 21);
            this.radioButtonYellow.TabIndex = 96;
            this.radioButtonYellow.Text = "Yellow";
            this.radioButtonYellow.UseVisualStyleBackColor = true;
            this.radioButtonYellow.CheckedChanged += new System.EventHandler(this.radioButtonColors_CheckedChanged);
            // 
            // radioButtonBlue
            // 
            this.radioButtonBlue.AutoSize = true;
            this.radioButtonBlue.Location = new System.Drawing.Point(8, 80);
            this.radioButtonBlue.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonBlue.Name = "radioButtonBlue";
            this.radioButtonBlue.Size = new System.Drawing.Size(57, 21);
            this.radioButtonBlue.TabIndex = 95;
            this.radioButtonBlue.Text = "Blue";
            this.radioButtonBlue.UseVisualStyleBackColor = true;
            this.radioButtonBlue.CheckedChanged += new System.EventHandler(this.radioButtonColors_CheckedChanged);
            // 
            // radioButtonGreen
            // 
            this.radioButtonGreen.AutoSize = true;
            this.radioButtonGreen.Location = new System.Drawing.Point(8, 52);
            this.radioButtonGreen.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonGreen.Name = "radioButtonGreen";
            this.radioButtonGreen.Size = new System.Drawing.Size(69, 21);
            this.radioButtonGreen.TabIndex = 94;
            this.radioButtonGreen.Text = "Green";
            this.radioButtonGreen.UseVisualStyleBackColor = true;
            this.radioButtonGreen.CheckedChanged += new System.EventHandler(this.radioButtonColors_CheckedChanged);
            // 
            // buttonDrawHomography
            // 
            this.buttonDrawHomography.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDrawHomography.Location = new System.Drawing.Point(709, 569);
            this.buttonDrawHomography.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDrawHomography.Name = "buttonDrawHomography";
            this.buttonDrawHomography.Size = new System.Drawing.Size(167, 28);
            this.buttonDrawHomography.TabIndex = 95;
            this.buttonDrawHomography.Text = "Draw Homography";
            this.buttonDrawHomography.UseVisualStyleBackColor = true;
            this.buttonDrawHomography.Click += new System.EventHandler(this.buttonDrawHomography_Click);
            // 
            // checkBoxWeight2
            // 
            this.checkBoxWeight2.AutoSize = true;
            this.checkBoxWeight2.Checked = true;
            this.checkBoxWeight2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWeight2.Location = new System.Drawing.Point(595, 295);
            this.checkBoxWeight2.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxWeight2.Name = "checkBoxWeight2";
            this.checkBoxWeight2.Size = new System.Drawing.Size(82, 21);
            this.checkBoxWeight2.TabIndex = 74;
            this.checkBoxWeight2.Text = "Weight2";
            this.checkBoxWeight2.UseVisualStyleBackColor = true;
            this.checkBoxWeight2.CheckedChanged += new System.EventHandler(this.checkBoxWeight_CheckedChanged);
            // 
            // FormWebCamEmgu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 758);
            this.Controls.Add(this.buttonDrawHomography);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.labelDownLevel);
            this.Controls.Add(this.labelUpLevel);
            this.Controls.Add(this.trackBarBlack);
            this.Controls.Add(this.trackBarWhite);
            this.Controls.Add(this.buttonGetWhiteColor);
            this.Controls.Add(this.labelSUp);
            this.Controls.Add(this.trackBarSUp);
            this.Controls.Add(this.labelSDown);
            this.Controls.Add(this.trackBarSDown);
            this.Controls.Add(this.labelHUp);
            this.Controls.Add(this.trackBarHUp);
            this.Controls.Add(this.labelHDown);
            this.Controls.Add(this.checkBoxWeight2);
            this.Controls.Add(this.checkBoxOldFiltration);
            this.Controls.Add(this.trackBarHDown);
            this.Controls.Add(this.checkBoxPan);
            this.Controls.Add(this.checkBoxSendControlPars);
            this.Controls.Add(this.checkBoxCharts);
            this.Controls.Add(this.checkBoxDrawViewPoint);
            this.Controls.Add(this.labelBrightness);
            this.Controls.Add(this.trackBarBrightness);
            this.Controls.Add(this.buttonCalibrate);
            this.Controls.Add(this.checkBoxTopFlickering);
            this.Controls.Add(this.checkBoxShowTextViz);
            this.Controls.Add(this.buttonLoadFrame);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonScreenshots);
            this.Controls.Add(this.trbY_Prag);
            this.Controls.Add(this.trbY_Add);
            this.Controls.Add(this.trbY_K);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.radioButtonDevice2);
            this.Controls.Add(this.radioButtonDevice1);
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
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormWebCamEmgu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormEmguWebCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWebCamEmgu_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWhite)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.RadioButton radioButtonDevice1;
        private System.Windows.Forms.RadioButton radioButtonDevice2;
        private MyTrackBar trbY_Prag;
        private MyTrackBar trbY_Add;
        private MyTrackBar trbY_K;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonScreenshots;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonLoadFrame;
        private System.Windows.Forms.CheckBox checkBoxShowTextViz;
        private System.Windows.Forms.CheckBox checkBoxTopFlickering;
        private System.Windows.Forms.Button buttonCalibrate;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.CheckBox checkBoxDrawViewPoint;
        private System.Windows.Forms.CheckBox checkBoxCharts;
        private System.Windows.Forms.CheckBox checkBoxSendControlPars;
        private System.Windows.Forms.CheckBox checkBoxPan;
        private System.Windows.Forms.TrackBar trackBarHDown;
        private System.Windows.Forms.CheckBox checkBoxOldFiltration;
        private System.Windows.Forms.Label labelHDown;
        private System.Windows.Forms.Label labelHUp;
        private System.Windows.Forms.TrackBar trackBarHUp;
        private System.Windows.Forms.Label labelSDown;
        private System.Windows.Forms.TrackBar trackBarSDown;
        private System.Windows.Forms.Label labelSUp;
        private System.Windows.Forms.TrackBar trackBarSUp;
        private System.Windows.Forms.Button buttonGetWhiteColor;
        private System.Windows.Forms.TrackBar trackBarBlack;
        private System.Windows.Forms.TrackBar trackBarWhite;
        private System.Windows.Forms.Label labelUpLevel;
        private System.Windows.Forms.Label labelDownLevel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.RadioButton radioButtonRed;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonYellow;
        private System.Windows.Forms.RadioButton radioButtonBlue;
        private System.Windows.Forms.RadioButton radioButtonGreen;
        private System.Windows.Forms.Button buttonDrawHomography;
        public System.Windows.Forms.CheckBox checkBoxWeight2;
    }
}