using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsExceptionHandling
{
    public partial class ErrorHandlerForm : Form
    {
        Thread newThread = null;

        public ErrorHandlerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new ArgumentException("The parameter was invalid");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThreadStart newThreadStart = new ThreadStart(newThread_Execute);
            newThread = new Thread(newThreadStart);
            newThread.Start();
        }

        // The thread we start up to demonstrate non-UI exception handling.  
        void newThread_Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
