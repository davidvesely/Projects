using System;
using System.Windows;
namespace SixDoFMouse
{
    partial class FormControls
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
            this.timerPanelPosition = new System.Windows.Forms.Timer(this.components);
            this.pbEmpty = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbInt = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbRelative = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbAbsolute = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbMoveZ = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbMoveY = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbRotateZ = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbRotateY = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbMoveX = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbRotateX = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbSettings = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbMoveAll = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pbRotateAll = new SixDoFMouse.AlphaForm2.PictureButton();
            this.pb6DOF = new SixDoFMouse.AlphaForm2.PictureButton();
            ((System.ComponentModel.ISupportInitialize)(this.pbEmpty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRelative)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAbsolute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb6DOF)).BeginInit();
            this.SuspendLayout();
            // 
            // timerPanelPosition
            // 
            this.timerPanelPosition.Interval = 35;
            this.timerPanelPosition.Tick += new System.EventHandler(this.timerPanelPosition_Tick);
            // 
            // pbEmpty
            // 
            this.pbEmpty.BackColor = System.Drawing.Color.Fuchsia;
            this.pbEmpty.DefaultImage = global::SixDoFMouse.Properties.Resources.Empty;
            this.pbEmpty.Image = global::SixDoFMouse.Properties.Resources.Empty;
            this.pbEmpty.IsSingleButton = false;
            this.pbEmpty.Location = new System.Drawing.Point(149, 70);
            this.pbEmpty.Margin = new System.Windows.Forms.Padding(0);
            this.pbEmpty.Name = "pbEmpty";
            this.pbEmpty.PressedImage = global::SixDoFMouse.Properties.Resources.empty1;
            this.pbEmpty.Size = new System.Drawing.Size(34, 16);
            this.pbEmpty.TabIndex = 13;
            this.pbEmpty.TabStop = false;
            this.pbEmpty.Click += new System.EventHandler(this.pbEmpty_Click);
            // 
            // pbInt
            // 
            this.pbInt.BackColor = System.Drawing.Color.Fuchsia;
            this.pbInt.DefaultImage = global::SixDoFMouse.Properties.Resources._int;
            this.pbInt.Image = global::SixDoFMouse.Properties.Resources._int;
            this.pbInt.IsSingleButton = false;
            this.pbInt.Location = new System.Drawing.Point(115, 70);
            this.pbInt.Margin = new System.Windows.Forms.Padding(0);
            this.pbInt.Name = "pbInt";
            this.pbInt.PressedImage = global::SixDoFMouse.Properties.Resources.int_in;
            this.pbInt.Size = new System.Drawing.Size(34, 16);
            this.pbInt.TabIndex = 12;
            this.pbInt.TabStop = false;
            // 
            // pbRelative
            // 
            this.pbRelative.BackColor = System.Drawing.Color.Fuchsia;
            this.pbRelative.DefaultImage = global::SixDoFMouse.Properties.Resources.rel;
            this.pbRelative.Image = global::SixDoFMouse.Properties.Resources.rel;
            this.pbRelative.IsSingleButton = false;
            this.pbRelative.Location = new System.Drawing.Point(81, 70);
            this.pbRelative.Margin = new System.Windows.Forms.Padding(0);
            this.pbRelative.Name = "pbRelative";
            this.pbRelative.PressedImage = global::SixDoFMouse.Properties.Resources.rel_in;
            this.pbRelative.Size = new System.Drawing.Size(34, 16);
            this.pbRelative.TabIndex = 11;
            this.pbRelative.TabStop = false;
            // 
            // pbAbsolute
            // 
            this.pbAbsolute.BackColor = System.Drawing.Color.Fuchsia;
            this.pbAbsolute.DefaultImage = global::SixDoFMouse.Properties.Resources.abs;
            this.pbAbsolute.Image = global::SixDoFMouse.Properties.Resources.abs;
            this.pbAbsolute.IsSingleButton = false;
            this.pbAbsolute.Location = new System.Drawing.Point(47, 70);
            this.pbAbsolute.Margin = new System.Windows.Forms.Padding(0);
            this.pbAbsolute.Name = "pbAbsolute";
            this.pbAbsolute.PressedImage = global::SixDoFMouse.Properties.Resources.abs_in;
            this.pbAbsolute.Size = new System.Drawing.Size(34, 16);
            this.pbAbsolute.TabIndex = 10;
            this.pbAbsolute.TabStop = false;
            // 
            // pbMoveZ
            // 
            this.pbMoveZ.BackColor = System.Drawing.Color.Fuchsia;
            this.pbMoveZ.DefaultImage = global::SixDoFMouse.Properties.Resources.moveZ;
            this.pbMoveZ.Image = global::SixDoFMouse.Properties.Resources.moveZ;
            this.pbMoveZ.IsSingleButton = false;
            this.pbMoveZ.Location = new System.Drawing.Point(149, 38);
            this.pbMoveZ.Margin = new System.Windows.Forms.Padding(0);
            this.pbMoveZ.Name = "pbMoveZ";
            this.pbMoveZ.PressedImage = global::SixDoFMouse.Properties.Resources.moveZ_in;
            this.pbMoveZ.Size = new System.Drawing.Size(34, 32);
            this.pbMoveZ.TabIndex = 9;
            this.pbMoveZ.TabStop = false;
            this.pbMoveZ.Click += new System.EventHandler(this.pbMoveZ_Click);
            // 
            // pbMoveY
            // 
            this.pbMoveY.BackColor = System.Drawing.Color.Fuchsia;
            this.pbMoveY.DefaultImage = global::SixDoFMouse.Properties.Resources.moveY;
            this.pbMoveY.Image = global::SixDoFMouse.Properties.Resources.moveY;
            this.pbMoveY.IsSingleButton = false;
            this.pbMoveY.Location = new System.Drawing.Point(115, 38);
            this.pbMoveY.Margin = new System.Windows.Forms.Padding(0);
            this.pbMoveY.Name = "pbMoveY";
            this.pbMoveY.PressedImage = global::SixDoFMouse.Properties.Resources.moveY_in;
            this.pbMoveY.Size = new System.Drawing.Size(34, 32);
            this.pbMoveY.TabIndex = 8;
            this.pbMoveY.TabStop = false;
            this.pbMoveY.Click += new System.EventHandler(this.pbMoveY_Click);
            // 
            // pbRotateZ
            // 
            this.pbRotateZ.BackColor = System.Drawing.Color.Fuchsia;
            this.pbRotateZ.DefaultImage = global::SixDoFMouse.Properties.Resources.rotZ;
            this.pbRotateZ.Image = global::SixDoFMouse.Properties.Resources.rotZ;
            this.pbRotateZ.IsSingleButton = false;
            this.pbRotateZ.Location = new System.Drawing.Point(149, 6);
            this.pbRotateZ.Margin = new System.Windows.Forms.Padding(0);
            this.pbRotateZ.Name = "pbRotateZ";
            this.pbRotateZ.PressedImage = global::SixDoFMouse.Properties.Resources.rotZ_in;
            this.pbRotateZ.Size = new System.Drawing.Size(34, 32);
            this.pbRotateZ.TabIndex = 7;
            this.pbRotateZ.TabStop = false;
            this.pbRotateZ.Click += new System.EventHandler(this.pbRotateZ_Click);
            // 
            // pbRotateY
            // 
            this.pbRotateY.BackColor = System.Drawing.Color.Fuchsia;
            this.pbRotateY.DefaultImage = global::SixDoFMouse.Properties.Resources.rotY;
            this.pbRotateY.Image = global::SixDoFMouse.Properties.Resources.rotY;
            this.pbRotateY.IsSingleButton = false;
            this.pbRotateY.Location = new System.Drawing.Point(115, 6);
            this.pbRotateY.Margin = new System.Windows.Forms.Padding(0);
            this.pbRotateY.Name = "pbRotateY";
            this.pbRotateY.PressedImage = global::SixDoFMouse.Properties.Resources.rotY_in;
            this.pbRotateY.Size = new System.Drawing.Size(34, 32);
            this.pbRotateY.TabIndex = 6;
            this.pbRotateY.TabStop = false;
            this.pbRotateY.Click += new System.EventHandler(this.pbRotateY_Click);
            // 
            // pbMoveX
            // 
            this.pbMoveX.BackColor = System.Drawing.Color.Fuchsia;
            this.pbMoveX.DefaultImage = global::SixDoFMouse.Properties.Resources.moveX;
            this.pbMoveX.Image = global::SixDoFMouse.Properties.Resources.moveX;
            this.pbMoveX.IsSingleButton = false;
            this.pbMoveX.Location = new System.Drawing.Point(81, 38);
            this.pbMoveX.Margin = new System.Windows.Forms.Padding(0);
            this.pbMoveX.Name = "pbMoveX";
            this.pbMoveX.PressedImage = global::SixDoFMouse.Properties.Resources.movex_in;
            this.pbMoveX.Size = new System.Drawing.Size(34, 32);
            this.pbMoveX.TabIndex = 5;
            this.pbMoveX.TabStop = false;
            this.pbMoveX.Click += new System.EventHandler(this.pbMoveX_Click);
            // 
            // pbRotateX
            // 
            this.pbRotateX.BackColor = System.Drawing.Color.Fuchsia;
            this.pbRotateX.DefaultImage = global::SixDoFMouse.Properties.Resources.rotateX;
            this.pbRotateX.Image = global::SixDoFMouse.Properties.Resources.rotateX;
            this.pbRotateX.IsSingleButton = false;
            this.pbRotateX.Location = new System.Drawing.Point(81, 6);
            this.pbRotateX.Margin = new System.Windows.Forms.Padding(0);
            this.pbRotateX.Name = "pbRotateX";
            this.pbRotateX.PressedImage = global::SixDoFMouse.Properties.Resources.rotateX_in;
            this.pbRotateX.Size = new System.Drawing.Size(34, 32);
            this.pbRotateX.TabIndex = 4;
            this.pbRotateX.TabStop = false;
            this.pbRotateX.Click += new System.EventHandler(this.pbRotateX_Click);
            // 
            // pbSettings
            // 
            this.pbSettings.BackColor = System.Drawing.Color.Fuchsia;
            this.pbSettings.DefaultImage = global::SixDoFMouse.Properties.Resources.settings;
            this.pbSettings.Image = global::SixDoFMouse.Properties.Resources.settings;
            this.pbSettings.IsSingleButton = true;
            this.pbSettings.Location = new System.Drawing.Point(5, 70);
            this.pbSettings.Margin = new System.Windows.Forms.Padding(0);
            this.pbSettings.Name = "pbSettings";
            this.pbSettings.PressedImage = global::SixDoFMouse.Properties.Resources.settings_in;
            this.pbSettings.Size = new System.Drawing.Size(42, 16);
            this.pbSettings.TabIndex = 3;
            this.pbSettings.TabStop = false;
            this.pbSettings.Click += new System.EventHandler(this.pbSettings_Click);
            // 
            // pbMoveAll
            // 
            this.pbMoveAll.BackColor = System.Drawing.Color.Fuchsia;
            this.pbMoveAll.DefaultImage = global::SixDoFMouse.Properties.Resources.moveall;
            this.pbMoveAll.Image = global::SixDoFMouse.Properties.Resources.moveall;
            this.pbMoveAll.IsSingleButton = false;
            this.pbMoveAll.Location = new System.Drawing.Point(47, 38);
            this.pbMoveAll.Margin = new System.Windows.Forms.Padding(0);
            this.pbMoveAll.Name = "pbMoveAll";
            this.pbMoveAll.PressedImage = global::SixDoFMouse.Properties.Resources.moveall_in;
            this.pbMoveAll.Size = new System.Drawing.Size(34, 32);
            this.pbMoveAll.TabIndex = 2;
            this.pbMoveAll.TabStop = false;
            this.pbMoveAll.Click += new System.EventHandler(this.pbMoveAll_Click);
            // 
            // pbRotateAll
            // 
            this.pbRotateAll.BackColor = System.Drawing.Color.Fuchsia;
            this.pbRotateAll.DefaultImage = global::SixDoFMouse.Properties.Resources.rotateall;
            this.pbRotateAll.Image = global::SixDoFMouse.Properties.Resources.rotateall;
            this.pbRotateAll.IsSingleButton = false;
            this.pbRotateAll.Location = new System.Drawing.Point(47, 6);
            this.pbRotateAll.Margin = new System.Windows.Forms.Padding(0);
            this.pbRotateAll.Name = "pbRotateAll";
            this.pbRotateAll.PressedImage = global::SixDoFMouse.Properties.Resources.rotateall_in;
            this.pbRotateAll.Size = new System.Drawing.Size(34, 32);
            this.pbRotateAll.TabIndex = 1;
            this.pbRotateAll.TabStop = false;
            this.pbRotateAll.Click += new System.EventHandler(this.pbRotateAll_Click);
            // 
            // pb6DOF
            // 
            this.pb6DOF.BackColor = System.Drawing.Color.Fuchsia;
            this.pb6DOF.DefaultImage = global::SixDoFMouse.Properties.Resources._6DOF;
            this.pb6DOF.Image = global::SixDoFMouse.Properties.Resources._6DOF;
            this.pb6DOF.IsSingleButton = false;
            this.pb6DOF.Location = new System.Drawing.Point(5, 6);
            this.pb6DOF.Margin = new System.Windows.Forms.Padding(0);
            this.pb6DOF.Name = "pb6DOF";
            this.pb6DOF.PressedImage = global::SixDoFMouse.Properties.Resources._6DOF_in;
            this.pb6DOF.Size = new System.Drawing.Size(42, 65);
            this.pb6DOF.TabIndex = 0;
            this.pb6DOF.TabStop = false;
            this.pb6DOF.Click += new System.EventHandler(this.pb6DOF_Click);
            // 
            // FormControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.BackgroundImage = global::SixDoFMouse.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(189, 93);
            this.Controls.Add(this.pbEmpty);
            this.Controls.Add(this.pbInt);
            this.Controls.Add(this.pbRelative);
            this.Controls.Add(this.pbAbsolute);
            this.Controls.Add(this.pbMoveZ);
            this.Controls.Add(this.pbMoveY);
            this.Controls.Add(this.pbRotateZ);
            this.Controls.Add(this.pbRotateY);
            this.Controls.Add(this.pbMoveX);
            this.Controls.Add(this.pbRotateX);
            this.Controls.Add(this.pbSettings);
            this.Controls.Add(this.pbMoveAll);
            this.Controls.Add(this.pbRotateAll);
            this.Controls.Add(this.pb6DOF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(1700, 20);
            this.Name = "FormControls";
            this.Opacity = 0.7D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormControls";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            ((System.ComponentModel.ISupportInitialize)(this.pbEmpty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRelative)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAbsolute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMoveAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRotateAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb6DOF)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AlphaForm2.PictureButton pb6DOF;
        private AlphaForm2.PictureButton pbRotateAll;
        private AlphaForm2.PictureButton pbMoveAll;
        private AlphaForm2.PictureButton pbSettings;
        private AlphaForm2.PictureButton pbRotateX;
        private AlphaForm2.PictureButton pbMoveX;
        private AlphaForm2.PictureButton pbRotateY;
        private AlphaForm2.PictureButton pbRotateZ;
        private AlphaForm2.PictureButton pbMoveY;
        private AlphaForm2.PictureButton pbMoveZ;
        private AlphaForm2.PictureButton pbAbsolute;
        private AlphaForm2.PictureButton pbRelative;
        private AlphaForm2.PictureButton pbInt;
        private AlphaForm2.PictureButton pbEmpty;

        private void InitializeArrays()
        {
            Buttons[0] = pb6DOF;       Buttons[1] = pbRotateAll;
            Buttons[2] = pbRotateX;    Buttons[3] = pbRotateY;
            Buttons[4] = pbRotateZ;    Buttons[5] = pbMoveAll;
            Buttons[6] = pbMoveX;      Buttons[7] = pbMoveY;
            Buttons[8] = pbMoveZ;      Buttons[9] = pbSettings;
            Buttons[10] = pbAbsolute;  Buttons[11] = pbRelative;
            Buttons[12] = pbInt;       Buttons[13] = pbEmpty;

            //Arrays for creating radio buttons
            Rotation[0] = pbRotateAll; Rotation[1] = pbRotateX;
            Rotation[2] = pbRotateY;   Rotation[3] = pbRotateZ;
            Movement[0] = pbMoveAll;   Movement[1] = pbMoveX;
            Movement[2] = pbMoveY;     Movement[3] = pbMoveZ;
            //Image arrays
            ButtonsDefault[0] = Properties.Resources._6DOF;
            ButtonsPressed[0] = Properties.Resources._6DOF_in;
            ButtonsDefault[1] = Properties.Resources.rotateall;
            ButtonsPressed[1] = Properties.Resources.rotateall_in;
            ButtonsDefault[2] = Properties.Resources.rotateX;
            ButtonsPressed[2] = Properties.Resources.rotateX_in;
            ButtonsDefault[3] = Properties.Resources.rotY;
            ButtonsPressed[3] = Properties.Resources.rotY_in;
            ButtonsDefault[4] = Properties.Resources.rotZ;
            ButtonsPressed[4] = Properties.Resources.rotZ_in;
            ButtonsDefault[5] = Properties.Resources.moveall;
            ButtonsPressed[5] = Properties.Resources.moveall_in;
            ButtonsDefault[6] = Properties.Resources.moveX;
            ButtonsPressed[6] = Properties.Resources.movex_in;
            ButtonsDefault[7] = Properties.Resources.moveY;
            ButtonsPressed[7] = Properties.Resources.moveY_in;
            ButtonsDefault[8] = Properties.Resources.moveZ;
            ButtonsPressed[8] = Properties.Resources.moveZ_in;
            ButtonsDefault[9] = Properties.Resources.settings;
            ButtonsPressed[9] = Properties.Resources.settings_in;
            ButtonsDefault[10] = Properties.Resources.abs;
            ButtonsPressed[10] = Properties.Resources.abs_in;
            ButtonsDefault[11] = Properties.Resources.rel;
            ButtonsPressed[11] = Properties.Resources.rel_in;
            ButtonsDefault[12] = Properties.Resources._int;
            ButtonsPressed[12] = Properties.Resources.int_in;
            ButtonsDefault[13] = Properties.Resources.Empty;
            ButtonsPressed[13] = Properties.Resources.empty1;
        }

        private void pb6DOF_Click(object sender, EventArgs e)
        {
            ClickedMovement = ClickedRotation = pb6DOF;
            TurnOffMovementButtons();
            TurnOffRotationButtons();
        }

        private void pbSettings_Click(object sender, EventArgs e)
        {
            pbSettings.TurnOff();
            MessageBox.Show("Settings appears here");
        }

        private void pbRotateAll_Click(object sender, EventArgs e)
        {
            ClickedRotation = pbRotateAll;
            TurnOffRotationButtons();
        }

        private void pbRotateX_Click(object sender, EventArgs e)
        {
            ClickedRotation = pbRotateX;
            TurnOffRotationButtons();
        }

        private void pbRotateY_Click(object sender, EventArgs e)
        {
            ClickedRotation = pbRotateY;
            TurnOffRotationButtons();
        }

        private void pbRotateZ_Click(object sender, EventArgs e)
        {
            ClickedRotation = pbRotateZ;
            TurnOffRotationButtons();
        }

        private void pbMoveAll_Click(object sender, EventArgs e)
        {
            ClickedMovement = pbMoveAll;
            TurnOffMovementButtons();
        }

        private void pbMoveX_Click(object sender, EventArgs e)
        {
            ClickedMovement = pbMoveX;
            TurnOffMovementButtons();
            byX = 10 * toward;
            byY = 0;
            byZ = 0;
        }

        private void pbMoveY_Click(object sender, EventArgs e)
        {
            ClickedMovement = pbMoveY;
            TurnOffMovementButtons();
            byX = 0;
            byY = 10 * toward;
            byZ = 0;
        }

        private void pbMoveZ_Click(object sender, EventArgs e)
        {
            ClickedMovement = pbMoveZ;
            TurnOffMovementButtons();
            byX = 0;
            byY = 0;
            byZ = 10 * toward;
        }
        private System.Windows.Forms.Timer timerPanelPosition;
    }
}