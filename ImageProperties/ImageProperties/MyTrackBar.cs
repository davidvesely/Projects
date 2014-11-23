using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProperties
{
    public partial class MyTrackBar : UserControl
    {
        public MyTrackBar()
        {
            InitializeComponent();
        }

        private void MyTrackBar_Load(object sender, EventArgs e)
        {
            textBox1.Text = (trackBar1.Value * _increment).ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = (trackBar1.Value * _increment).ToString();
        }

        public double Value
        {
            get { return Convert.ToDouble(textBox1.Text); }
            set
            {
                textBox1.Text = value.ToString();
                int val = (int)Math.Round(value / _increment);
                if ((trackBar1.Minimum <= val) && (val <= trackBar1.Maximum))
                    trackBar1.Value = val;
            }
        }

        public int MinVal
        {
            get { return trackBar1.Minimum; }
            set { trackBar1.Minimum = value; }
        }

        public int MaxVal
        {
            get { return trackBar1.Maximum; }
            set { trackBar1.Maximum = value; }
        }

        public int SmallChange
        {
            get { return trackBar1.SmallChange; }
            set { trackBar1.SmallChange = value; }
        }

        public int LargeChange
        {
            get { return trackBar1.LargeChange; }
            set { trackBar1.LargeChange = value; }
        }

        float _increment;
        [DefaultValue(1)]
        public float Increment
        {
            get { return _increment; }
            set
            {
                _increment = value;
                textBox1.Text = (trackBar1.Value * _increment).ToString();
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                double number;
                int val;
                if (textBox1.Text != "")
                {
                    number = Convert.ToDouble(textBox1.Text);
                }
                else
                    return;
                val = (int)(number / _increment);
                if ((trackBar1.Minimum <= val) && (val <= trackBar1.Maximum))
                    trackBar1.Value = val;
            }
            catch (FormatException)
            {
                textBox1.Text = (trackBar1.Value * _increment).ToString();
                textBox1.Select(textBox1.Text.Length, 0);
                MessageBox.Show("Enter only digits please!");
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.Text = (trackBar1.Value * _increment).ToString();
        }
    }
}
