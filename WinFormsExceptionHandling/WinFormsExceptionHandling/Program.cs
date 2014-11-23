using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsExceptionHandling.Properties;

namespace WinFormsExceptionHandling
{
    public class Program
    {
        // Starts the application. 
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void Main(string[] args)
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through 
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            InitializeLogging();

            // Runs the application.
            Application.Run(new ErrorHandlerForm());
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether 
        // or not they wish to abort execution. 
        // NOTE: This exception cannot be kept from terminating the application - it can only  
        // log the event, and inform the user about it.  
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errorMsg = "An application error occurred. Please contact the adminstrator " +
                    "with the following information:\n\n" + ex.ToString();

                // Create an EventLog instance and assign its source.
                EventLog.WriteEntry(Settings.Default.LogSource, errorMsg);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show(
                        "Fatal Error. Could not write the error to the event log. Reason:\n"
                        + exc.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether 
        // or not they wish to abort execution. 
        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Грешка", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort. 
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Creates the error message and displays it. 
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            //string errorMsg = "An application error occurred. Please contact the adminstrator " +
            //    "with the following information:\n\n";
            //errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            //return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
            //    MessageBoxIcon.Stop);

            Exception ex = e;
            // Ако има Inner Exception взима неговото съобщение за грешка
            if (ex.InnerException != null)
                ex = ex.InnerException;
            string errorMsg = ex.Message;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }

        private static void InitializeLogging()
        {
            // Since we can't prevent the app from terminating, log this to the event log. 
            if (!EventLog.SourceExists(Settings.Default.LogSource))
            {
                EventLog.CreateEventSource(Settings.Default.LogSource, Settings.Default.LogName);
            }
        }
    }
}
