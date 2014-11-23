namespace ImageProperties
{
    partial class BarrelForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.BarrelzoomPicBox = new ImageProperties.ZoomPicBox();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.barelToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(931, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // barelToolStripMenuItem
            // 
            this.barelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.barelToolStripMenuItem1});
            this.barelToolStripMenuItem.Name = "barelToolStripMenuItem";
            this.barelToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.barelToolStripMenuItem.Text = "Barel";
            // 
            // barelToolStripMenuItem1
            // 
            this.barelToolStripMenuItem1.Name = "barelToolStripMenuItem1";
            this.barelToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
            this.barelToolStripMenuItem1.Text = "Barel";
            this.barelToolStripMenuItem1.Click += new System.EventHandler(this.barelToolStripMenuItem1_Click);
            // 
            // BarrelzoomPicBox
            // 
            this.BarrelzoomPicBox.AutoScroll = true;
            this.BarrelzoomPicBox.AutoScrollMinSize = new System.Drawing.Size(907, 521);
            this.BarrelzoomPicBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BarrelzoomPicBox.Image = null;
            this.BarrelzoomPicBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.BarrelzoomPicBox.Location = new System.Drawing.Point(12, 86);
            this.BarrelzoomPicBox.Name = "BarrelzoomPicBox";
            this.BarrelzoomPicBox.Size = new System.Drawing.Size(907, 521);
            this.BarrelzoomPicBox.TabIndex = 1;
            this.BarrelzoomPicBox.Text = "zoomPicBox1";
            this.BarrelzoomPicBox.Zoom = 1F;
            this.BarrelzoomPicBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BarrelzoomPicBox_MouseMove);
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(271, 11);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(46, 17);
            this.labelX.TabIndex = 2;
            this.labelX.Text = "label1";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(337, 11);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(46, 17);
            this.labelY.TabIndex = 3;
            this.labelY.Text = "label1";
            // 
            // BarrelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 619);
            this.Controls.Add(this.labelY);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.BarrelzoomPicBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BarrelForm";
            this.Text = "BarelForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private ZoomPicBox BarrelzoomPicBox;
        private System.Windows.Forms.ToolStripMenuItem barelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem barelToolStripMenuItem1;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
    }
}