namespace QuickStichNamespace
{
    partial class FormMain
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
            this.buttonProcess = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonAddLeft = new System.Windows.Forms.Button();
            this.buttonAddRight = new System.Windows.Forms.Button();
            this.buttonStitch = new System.Windows.Forms.Button();
            this.buttonLoadMain = new System.Windows.Forms.Button();
            this.panelImages = new MyPanel();
            this.SuspendLayout();
            // 
            // buttonProcess
            // 
            this.buttonProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProcess.Location = new System.Drawing.Point(996, 583);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(75, 25);
            this.buttonProcess.TabIndex = 2;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.ButtonProcess_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(13, 583);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(735, 25);
            this.progressBar1.TabIndex = 4;
            // 
            // buttonAddLeft
            // 
            this.buttonAddLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddLeft.Location = new System.Drawing.Point(834, 583);
            this.buttonAddLeft.Name = "buttonAddLeft";
            this.buttonAddLeft.Size = new System.Drawing.Size(75, 25);
            this.buttonAddLeft.TabIndex = 5;
            this.buttonAddLeft.Text = "Add Left";
            this.buttonAddLeft.UseVisualStyleBackColor = true;
            this.buttonAddLeft.Click += new System.EventHandler(this.buttonAddImage_Click);
            // 
            // buttonAddRight
            // 
            this.buttonAddRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddRight.Location = new System.Drawing.Point(915, 583);
            this.buttonAddRight.Name = "buttonAddRight";
            this.buttonAddRight.Size = new System.Drawing.Size(75, 25);
            this.buttonAddRight.TabIndex = 6;
            this.buttonAddRight.Text = "Add Right";
            this.buttonAddRight.UseVisualStyleBackColor = true;
            this.buttonAddRight.Click += new System.EventHandler(this.buttonAddImage_Click);
            // 
            // buttonStitch
            // 
            this.buttonStitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStitch.Location = new System.Drawing.Point(753, 583);
            this.buttonStitch.Name = "buttonStitch";
            this.buttonStitch.Size = new System.Drawing.Size(75, 25);
            this.buttonStitch.TabIndex = 7;
            this.buttonStitch.Text = "Stitch";
            this.buttonStitch.UseVisualStyleBackColor = true;
            this.buttonStitch.Click += new System.EventHandler(this.buttonStitch_Click);
            // 
            // buttonLoadMain
            // 
            this.buttonLoadMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadMain.Location = new System.Drawing.Point(834, 583);
            this.buttonLoadMain.Name = "buttonLoadMain";
            this.buttonLoadMain.Size = new System.Drawing.Size(156, 25);
            this.buttonLoadMain.TabIndex = 8;
            this.buttonLoadMain.Text = "Load Main Image";
            this.buttonLoadMain.UseVisualStyleBackColor = true;
            this.buttonLoadMain.Click += new System.EventHandler(this.buttonLoadMain_Click);
            // 
            // panelImages
            // 
            this.panelImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImages.AutoScroll = true;
            this.panelImages.BackColor = System.Drawing.SystemColors.Control;
            this.panelImages.Location = new System.Drawing.Point(13, 13);
            this.panelImages.Name = "panelImages";
            this.panelImages.Size = new System.Drawing.Size(1058, 564);
            this.panelImages.TabIndex = 9;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 618);
            this.Controls.Add(this.panelImages);
            this.Controls.Add(this.buttonStitch);
            this.Controls.Add(this.buttonAddRight);
            this.Controls.Add(this.buttonAddLeft);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.buttonLoadMain);
            this.Name = "FormMain";
            this.Text = "Quick Stitch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonProcess;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonAddLeft;
        private System.Windows.Forms.Button buttonAddRight;
        private System.Windows.Forms.Button buttonStitch;
        private System.Windows.Forms.Button buttonLoadMain;
        private MyPanel panelImages;
    }
}

