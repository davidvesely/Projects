namespace QuickStichNamespace
{
    partial class FormResult
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
            this.zoomPicBoxResult = new QuickStichNamespace.ZoomPicBox();
            this.SuspendLayout();
            // 
            // zoomPicBoxResult
            // 
            this.zoomPicBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomPicBoxResult.AutoScroll = true;
            this.zoomPicBoxResult.AutoScrollMinSize = new System.Drawing.Size(302, 248);
            this.zoomPicBoxResult.BackColor = System.Drawing.SystemColors.ControlDark;
            this.zoomPicBoxResult.Image = null;
            this.zoomPicBoxResult.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.zoomPicBoxResult.Location = new System.Drawing.Point(46, 32);
            this.zoomPicBoxResult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.zoomPicBoxResult.Name = "zoomPicBoxResult";
            this.zoomPicBoxResult.Size = new System.Drawing.Size(505, 353);
            this.zoomPicBoxResult.TabIndex = 0;
            this.zoomPicBoxResult.Text = "Result";
            this.zoomPicBoxResult.Zoom = 1F;
            // 
            // FormResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 464);
            this.Controls.Add(this.zoomPicBoxResult);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormResult";
            this.Text = "Result";
            this.ResumeLayout(false);

        }

        #endregion

        private ZoomPicBox zoomPicBoxResult;
    }
}