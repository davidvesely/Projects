using System.Windows.Forms;
namespace ImageProperties
{
    partial class FormViewer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelZoom = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelB = new System.Windows.Forms.Label();
            this.labelG = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cornerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grayscaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inverColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middleColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testBWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxBorder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.labelBPick = new System.Windows.Forms.Label();
            this.labelGPick = new System.Windows.Forms.Label();
            this.labelRPick = new System.Windows.Forms.Label();
            this.Magnifier = new ImageProperties.MagnifierBox();
            this.zoomPicBox1 = new ImageProperties.ZoomPicBox();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Magnifier)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelZoom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.labelY);
            this.groupBox1.Controls.Add(this.labelX);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labelB);
            this.groupBox1.Controls.Add(this.labelG);
            this.groupBox1.Controls.Add(this.labelR);
            this.groupBox1.Location = new System.Drawing.Point(804, 26);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(181, 124);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pixel Info";
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(54, 101);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 8;
            this.labelZoom.Text = "Zoom";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Zoom:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 67);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "X:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 84);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Y:";
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(54, 84);
            this.labelY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(14, 13);
            this.labelY.TabIndex = 5;
            this.labelY.Text = "Y";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(54, 67);
            this.labelX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(14, 13);
            this.labelX.TabIndex = 4;
            this.labelX.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Blue";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 36);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Green";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 22);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Red";
            // 
            // labelB
            // 
            this.labelB.AutoSize = true;
            this.labelB.Location = new System.Drawing.Point(54, 50);
            this.labelB.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelB.Name = "labelB";
            this.labelB.Size = new System.Drawing.Size(28, 13);
            this.labelB.TabIndex = 2;
            this.labelB.Text = "Blue";
            // 
            // labelG
            // 
            this.labelG.AutoSize = true;
            this.labelG.Location = new System.Drawing.Point(54, 36);
            this.labelG.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelG.Name = "labelG";
            this.labelG.Size = new System.Drawing.Size(36, 13);
            this.labelG.TabIndex = 1;
            this.labelG.Text = "Green";
            // 
            // labelR
            // 
            this.labelR.AutoSize = true;
            this.labelR.Location = new System.Drawing.Point(54, 22);
            this.labelR.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelR.Name = "labelR";
            this.labelR.Size = new System.Drawing.Size(27, 13);
            this.labelR.TabIndex = 0;
            this.labelR.Text = "Red";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.filterToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(998, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sobelToolStripMenuItem,
            this.cornerToolStripMenuItem,
            this.grayscaleToolStripMenuItem,
            this.inverColorToolStripMenuItem,
            this.middleColorToolStripMenuItem,
            this.testBWToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // sobelToolStripMenuItem
            // 
            this.sobelToolStripMenuItem.Name = "sobelToolStripMenuItem";
            this.sobelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.sobelToolStripMenuItem.Text = "Sobel";
            this.sobelToolStripMenuItem.Click += new System.EventHandler(this.sobelToolStripMenuItem_Click);
            // 
            // cornerToolStripMenuItem
            // 
            this.cornerToolStripMenuItem.Name = "cornerToolStripMenuItem";
            this.cornerToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.cornerToolStripMenuItem.Text = "Corner";
            this.cornerToolStripMenuItem.Click += new System.EventHandler(this.cornerToolStripMenuItem_Click);
            // 
            // grayscaleToolStripMenuItem
            // 
            this.grayscaleToolStripMenuItem.Name = "grayscaleToolStripMenuItem";
            this.grayscaleToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.grayscaleToolStripMenuItem.Text = "Grayscale";
            this.grayscaleToolStripMenuItem.Click += new System.EventHandler(this.grayscaleToolStripMenuItem_Click);
            // 
            // inverColorToolStripMenuItem
            // 
            this.inverColorToolStripMenuItem.Name = "inverColorToolStripMenuItem";
            this.inverColorToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.inverColorToolStripMenuItem.Text = "InvertColor";
            this.inverColorToolStripMenuItem.Click += new System.EventHandler(this.inverColorToolStripMenuItem_Click);
            // 
            // middleColorToolStripMenuItem
            // 
            this.middleColorToolStripMenuItem.Name = "middleColorToolStripMenuItem";
            this.middleColorToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.middleColorToolStripMenuItem.Text = "MiddleColor";
            this.middleColorToolStripMenuItem.Click += new System.EventHandler(this.middleColorToolStripMenuItem_Click);
            // 
            // testBWToolStripMenuItem
            // 
            this.testBWToolStripMenuItem.Name = "testBWToolStripMenuItem";
            this.testBWToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.testBWToolStripMenuItem.Text = "Test B&W";
            this.testBWToolStripMenuItem.Click += new System.EventHandler(this.testBWToolStripMenuItem_Click);
            // 
            // textBoxBorder
            // 
            this.textBoxBorder.Location = new System.Drawing.Point(872, 359);
            this.textBoxBorder.Name = "textBoxBorder";
            this.textBoxBorder.Size = new System.Drawing.Size(100, 20);
            this.textBoxBorder.TabIndex = 10;
            this.textBoxBorder.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(804, 362);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Border pixel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(838, 414);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Blue";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(830, 400);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Green";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(839, 386);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Red";
            // 
            // labelBPick
            // 
            this.labelBPick.AutoSize = true;
            this.labelBPick.Location = new System.Drawing.Point(870, 414);
            this.labelBPick.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBPick.Name = "labelBPick";
            this.labelBPick.Size = new System.Drawing.Size(28, 13);
            this.labelBPick.TabIndex = 11;
            this.labelBPick.Text = "Blue";
            // 
            // labelGPick
            // 
            this.labelGPick.AutoSize = true;
            this.labelGPick.Location = new System.Drawing.Point(870, 400);
            this.labelGPick.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGPick.Name = "labelGPick";
            this.labelGPick.Size = new System.Drawing.Size(36, 13);
            this.labelGPick.TabIndex = 10;
            this.labelGPick.Text = "Green";
            // 
            // labelRPick
            // 
            this.labelRPick.AutoSize = true;
            this.labelRPick.Location = new System.Drawing.Point(870, 386);
            this.labelRPick.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRPick.Name = "labelRPick";
            this.labelRPick.Size = new System.Drawing.Size(27, 13);
            this.labelRPick.TabIndex = 9;
            this.labelRPick.Text = "Red";
            // 
            // Magnifier
            // 
            this.Magnifier.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.Magnifier.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Magnifier.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Magnifier.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Magnifier.Location = new System.Drawing.Point(807, 154);
            this.Magnifier.Margin = new System.Windows.Forms.Padding(2);
            this.Magnifier.Name = "Magnifier";
            this.Magnifier.Size = new System.Drawing.Size(180, 180);
            this.Magnifier.TabIndex = 9;
            this.Magnifier.TabStop = false;
            // 
            // zoomPicBox1
            // 
            this.zoomPicBox1.AutoScroll = true;
            this.zoomPicBox1.AutoScrollMinSize = new System.Drawing.Size(799, 666);
            this.zoomPicBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.zoomPicBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.zoomPicBox1.Image = null;
            this.zoomPicBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.zoomPicBox1.Location = new System.Drawing.Point(0, 26);
            this.zoomPicBox1.Name = "zoomPicBox1";
            this.zoomPicBox1.Size = new System.Drawing.Size(799, 666);
            this.zoomPicBox1.TabIndex = 1;
            this.zoomPicBox1.Text = "zoomPicBox1";
            this.zoomPicBox1.Zoom = 1F;
            this.zoomPicBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.zoomPicBox1_MouseDoubleClick);
            this.zoomPicBox1.MouseLeave += new System.EventHandler(this.zoomPicBox1_MouseLeave);
            this.zoomPicBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.zoomPicBox1_MouseMove);
            // 
            // FormViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 708);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxBorder);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.labelBPick);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelGPick);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelRPick);
            this.Controls.Add(this.Magnifier);
            this.Controls.Add(this.zoomPicBox1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(639, 478);
            this.Name = "FormViewer";
            this.Text = "Form1";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Magnifier)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoomPicBox zoomPicBox1;
        private GroupBox groupBox1;
        private Label label8;
        private Label label7;
        private Label labelY;
        private Label labelX;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label labelB;
        private Label labelG;
        private Label labelR;
        private Label labelZoom;
        private Label label1;
        private MagnifierBox Magnifier;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem filterToolStripMenuItem;
        private ToolStripMenuItem sobelToolStripMenuItem;
        private ToolStripMenuItem cornerToolStripMenuItem;
        private ToolStripMenuItem grayscaleToolStripMenuItem;
        private ToolStripMenuItem inverColorToolStripMenuItem;
        private ToolStripMenuItem middleColorToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem testBWToolStripMenuItem;
        private TextBox textBoxBorder;
        private Label label2;
        private Label label3;
        private Label label9;
        private Label label10;
        private Label labelBPick;
        private Label labelGPick;
        private Label labelRPick;
    }
}

