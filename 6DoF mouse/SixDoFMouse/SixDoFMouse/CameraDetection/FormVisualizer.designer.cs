namespace SixDoFMouse
{
    partial class FormVisualizer
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
            this.components = new System.ComponentModel.Container();
            this.imageBoxEmgu = new Emgu.CV.UI.ImageBox();
            this.textBoxData5 = new System.Windows.Forms.TextBox();
            this.textBoxData1 = new System.Windows.Forms.TextBox();
            this.textBoxData4 = new System.Windows.Forms.TextBox();
            this.textBoxData2 = new System.Windows.Forms.TextBox();
            this.textBoxData3 = new System.Windows.Forms.TextBox();
            this.zoomPicBox1 = new SixDoFMouse.CameraDetection.ZoomPicBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxEmgu)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBoxEmgu
            // 
            this.imageBoxEmgu.BackColor = System.Drawing.SystemColors.ControlDark;
            this.imageBoxEmgu.Location = new System.Drawing.Point(506, 238);
            this.imageBoxEmgu.Name = "imageBoxEmgu";
            this.imageBoxEmgu.Size = new System.Drawing.Size(164, 226);
            this.imageBoxEmgu.TabIndex = 2;
            this.imageBoxEmgu.TabStop = false;
            // 
            // textBoxData5
            // 
            this.textBoxData5.Location = new System.Drawing.Point(12, 116);
            this.textBoxData5.Name = "textBoxData5";
            this.textBoxData5.Size = new System.Drawing.Size(158, 20);
            this.textBoxData5.TabIndex = 7;
            this.textBoxData5.Visible = false;
            // 
            // textBoxData1
            // 
            this.textBoxData1.Location = new System.Drawing.Point(12, 12);
            this.textBoxData1.Name = "textBoxData1";
            this.textBoxData1.Size = new System.Drawing.Size(158, 20);
            this.textBoxData1.TabIndex = 3;
            this.textBoxData1.Visible = false;
            // 
            // textBoxData4
            // 
            this.textBoxData4.Location = new System.Drawing.Point(12, 90);
            this.textBoxData4.Name = "textBoxData4";
            this.textBoxData4.Size = new System.Drawing.Size(158, 20);
            this.textBoxData4.TabIndex = 6;
            this.textBoxData4.Visible = false;
            // 
            // textBoxData2
            // 
            this.textBoxData2.Location = new System.Drawing.Point(12, 38);
            this.textBoxData2.Name = "textBoxData2";
            this.textBoxData2.Size = new System.Drawing.Size(158, 20);
            this.textBoxData2.TabIndex = 4;
            this.textBoxData2.Visible = false;
            // 
            // textBoxData3
            // 
            this.textBoxData3.Location = new System.Drawing.Point(12, 64);
            this.textBoxData3.Name = "textBoxData3";
            this.textBoxData3.Size = new System.Drawing.Size(158, 20);
            this.textBoxData3.TabIndex = 5;
            this.textBoxData3.Visible = false;
            // 
            // zoomPicBox1
            // 
            this.zoomPicBox1.AutoScroll = true;
            this.zoomPicBox1.AutoScrollMinSize = new System.Drawing.Size(372, 317);
            this.zoomPicBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.zoomPicBox1.Image = null;
            this.zoomPicBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.zoomPicBox1.Location = new System.Drawing.Point(13, 181);
            this.zoomPicBox1.Name = "zoomPicBox1";
            this.zoomPicBox1.Size = new System.Drawing.Size(372, 317);
            this.zoomPicBox1.TabIndex = 0;
            this.zoomPicBox1.Text = "zoomPicBox1";
            this.zoomPicBox1.Zoom = 1F;
            // 
            // FormVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 601);
            this.Controls.Add(this.textBoxData5);
            this.Controls.Add(this.imageBoxEmgu);
            this.Controls.Add(this.textBoxData1);
            this.Controls.Add(this.textBoxData4);
            this.Controls.Add(this.zoomPicBox1);
            this.Controls.Add(this.textBoxData2);
            this.Controls.Add(this.textBoxData3);
            this.Location = new System.Drawing.Point(10, 10);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormVisualizer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Formsdasad";
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxEmgu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SixDoFMouse.CameraDetection.ZoomPicBox zoomPicBox1;
        public Emgu.CV.UI.ImageBox imageBoxEmgu;
        private System.Windows.Forms.TextBox textBoxData1;
        private System.Windows.Forms.TextBox textBoxData2;
        private System.Windows.Forms.TextBox textBoxData3;
        private System.Windows.Forms.TextBox textBoxData4;
        private System.Windows.Forms.TextBox textBoxData5;

    }
}