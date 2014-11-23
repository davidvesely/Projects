using Microsoft.Win32;
//using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AuthorizationLib
{
    [ComVisible(false)]
    public class OfflineAuthorization
    {
        // Check the registry for valid date and serial number
        public static bool CheckSerialNumberRegistry(string productName, string productSerialRegistry)
        {
            string nextDateCheck = CommonAttributes.LoadFromRegistry(CommonAttributes.RegNewDate);
            DateTime RegKeyDate;
            nextDateCheck = Encryption.Decrypt(nextDateCheck);
            try
            {
               RegKeyDate = DateTime.ParseExact(nextDateCheck, "yyyyMMdd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception)
            {
                MessageBox.Show("Please provide Internet connection for validating your product");
                return false;
            }
            if ((RegKeyDate <= DateTime.Now) || (nextDateCheck == string.Empty) || (nextDateCheck == null))
            {
                MessageBox.Show("Please provide Internet connection for validating your product");
                return false;
            }

            // TO DO: check for trial period
                      
            string serialNumber = CommonAttributes.LoadSNFromRegistry(productSerialRegistry);
            if (serialNumber == null) // In case of missing registry entry
            {
                MessageBox.Show("Corrupted installation found. Please reinstall " + productName + "!");
                return false;
            }

            // Trial mode or invalid serial number
            if ((serialNumber == "0") || (serialNumber == string.Empty))
            {
                // Displays the trial message once a day
                DisplayTrialMessage(productName, productSerialRegistry);
            }
            // If everything is ok program loading continues
            return true;
        }

        public static void DisplayTrialMessage(string productName, string productSerialRegistry)
        {
            //Check for registry entry, that indicates if the message was displayed previously today
            string msgDispDate = CommonAttributes.LoadFromRegistry(productSerialRegistry + "_tr");
            string todayStr = DateTime.Now.ToString(CommonAttributes.DATE_FORMAT, System.Globalization.CultureInfo.GetCultureInfo("en-US"));

            if (msgDispDate != todayStr)
            {
                int daysRemaining = CommonAttributes.GetRemainingDaysTrial();
                // Displays the trial message once a day
                MessageBox.Show(
                    string.Format("You are running trial version of {0} and you\n" +
                    "have {1} days remaining. You can register your product\n" +
                    "by running About from the {0} menu.", productName, daysRemaining.ToString()), "Please register"
                );
                CommonAttributes.SaveToRegistry(productSerialRegistry + "_tr", todayStr);
            }
        }
    }
}
