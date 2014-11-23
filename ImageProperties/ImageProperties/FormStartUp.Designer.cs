namespace ImageProperties
{
    partial class FormStartUp
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
            this.buttonViewer = new System.Windows.Forms.Button();
            this.buttonWebCam = new System.Windows.Forms.Button();
            this.buttonOpenCV = new System.Windows.Forms.Button();
            this.BarrelBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonViewer
            // 
            this.buttonViewer.Location = new System.Drawing.Point(183, 113);
            this.buttonViewer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonViewer.Name = "buttonViewer";
            this.buttonViewer.Size = new System.Drawing.Size(175, 28);
            this.buttonViewer.TabIndex = 0;
            this.buttonViewer.Text = "Viewer";
            this.buttonViewer.UseVisualStyleBackColor = true;
            this.buttonViewer.Click += new System.EventHandler(this.buttonViewer_Click);
            // 
            // buttonWebCam
            // 
            this.buttonWebCam.Location = new System.Drawing.Point(183, 149);
            this.buttonWebCam.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonWebCam.Name = "buttonWebCam";
            this.buttonWebCam.Size = new System.Drawing.Size(175, 28);
            this.buttonWebCam.TabIndex = 1;
            this.buttonWebCam.Text = "Webcamera Filters";
            this.buttonWebCam.UseVisualStyleBackColor = true;
            this.buttonWebCam.Click += new System.EventHandler(this.buttonWebCam_Click);
            // 
            // buttonOpenCV
            // 
            this.buttonOpenCV.Location = new System.Drawing.Point(183, 185);
            this.buttonOpenCV.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOpenCV.Name = "buttonOpenCV";
            this.buttonOpenCV.Size = new System.Drawing.Size(175, 28);
            this.buttonOpenCV.TabIndex = 2;
            this.buttonOpenCV.Text = "Webcamera OpenCV";
            this.buttonOpenCV.UseVisualStyleBackColor = true;
            this.buttonOpenCV.Click += new System.EventHandler(this.buttonOpenCV_Click);
            // 
            // BarrelBut
            // 
            this.BarrelBut.Location = new System.Drawing.Point(183, 71);
            this.BarrelBut.Name = "BarrelBut";
            this.BarrelBut.Size = new System.Drawing.Size(175, 35);
            this.BarrelBut.TabIndex = 3;
            this.BarrelBut.Text = "Barrel Distorsion";
            this.BarrelBut.UseVisualStyleBackColor = true;
            this.BarrelBut.Click += new System.EventHandler(this.BarrelBut_Click);
            // 
            // FormStartUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 337);
            this.Controls.Add(this.BarrelBut);
            this.Controls.Add(this.buttonOpenCV);
            this.Controls.Add(this.buttonWebCam);
            this.Controls.Add(this.buttonViewer);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormStartUp";
            this.Text = "StartUpForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonViewer;
        private System.Windows.Forms.Button buttonWebCam;
        private System.Windows.Forms.Button buttonOpenCV;
        private System.Windows.Forms.Button BarrelBut;
    }
}