using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Net.NetworkInformation;

namespace AuthorizationLib
{
    [ComVisible(false)]
    public class OnlineAuthorization
    {
        #region Local Variables and Constants
        //Address was reduced to one field, Andrey wanted it that way
        public enum RegCustInd { email, UserName, Company, Address, Phone }
        public static string[] RegIfnoCustomer = { "email", "UserName", "Company", "Address", "Phone" };

        // Online related things
        private const string AuthURL = "http://cadbest.com/auth/index.php";
        //private enum ParametersIndex { ProductID, UserID, SerialNumber }
        private static string[] Parameter = { 
            /*"UserID"*/ "param1",
            /*"SerialNumber"*/ "param2",
            /*"ProductID"*/ "param3"
        };

        // Response from online authorization strings
        private const char SHOULDRUN = '1';
        private const char ISREGISTERED = '1';
        #endregion

        #region Assembly Exports
        public static string GetUserInfoElement(RegCustInd index)
        {
            return CommonAttributes.LoadFromRegistry(RegIfnoCustomer[(int)index]);
        }

        public static bool CheckConnection(int timeout)
        {
            bool result;
            Ping myPing = new Ping();
            String host = "www.cadbest.com";
            byte[] buffer = new byte[32];
            PingOptions pingOptions = new PingOptions();
            try
            {
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                    // presumably online
                    result = true;
                else
                    result = false;
            }
            catch (PingException)
            {
                result = false;
            }
            return result;
        }

        public static void CheckProgramState(string ProductID, string SerialNumberIn, 
            string RegSerialField, string ProductName, out Boolean AllowRun, out Boolean IsRegistered)
        {
            // cadbest.com/en/key.php?id=213&data2=1231321&.... - sample for parsing parameteres to the url
            try
            {
                string userID, serialNumber;
                string encrProductID, encrUserID, encrSerialNum;

                // Get the serial number of C:\
                userID = GetUserID();
                // Get the serial number from registry if is not provided
                if  (string.IsNullOrEmpty(SerialNumberIn))
                    // From registry serial number is already encrypted
                    serialNumber = CommonAttributes.LoadSNFromRegistry(RegSerialField);
                else
                    // When is passed from About or other module, it is not encrypted
                    serialNumber = SerialNumberIn;

                //Encrypt the data
                encrProductID = Encryption.Encrypt(ProductID, 150);
                encrUserID = Encryption.Encrypt(userID, 200);
                encrSerialNum = Encryption.Encrypt(serialNumber, 250);

                using (WebClient client = new WebClient())
                {
                    string responseStr;
                    NameValueCollection data = new NameValueCollection();
#if DEBUG
                    data[Parameter[0]] = encrUserID;
                    data[Parameter[1]] = encrSerialNum;
                    data[Parameter[2]] = encrProductID;
#else
                    data[Parameter[0]] = encrUserID;
                    data[Parameter[1]] = encrSerialNum;
                    data[Parameter[2]] = encrProductID;
#endif
                    // Send and recieve from the authorization url
                    Byte[] response = client.UploadValues(AuthURL.ToString(), "POST", data);
                    responseStr = System.Text.Encoding.UTF8.GetString(response);
                    // Replace <br /> with \n for proper work of StringReader
                    responseStr = responseStr.Replace("<br />", "\n");
                    
                    // Reads the response string line by line
                    using (StringReader reader = new StringReader(responseStr))
                    {
                        string line;
                        //Initializing
                        AllowRun = false;
                        IsRegistered = false;
                        // Read "Should run" responce
                        line = reader.ReadLine();

#if DEBUG
                        // Check if the response string contains backslash
                        if (line.Contains("\\"))
                        {
                            MessageBox.Show("Response contains backslash\n" + line);
                        }
#endif

                        if (line != null)
                        {
                            DateTime nextCheck;
                            line = Encryption.Decrypt(line);
                            if (line[0] == SHOULDRUN)
                                AllowRun = true;
                            if (line[1] == ISREGISTERED)
                                IsRegistered = true;

                            if (AllowRun && IsRegistered) // Product works for 30 days
                            {
                                nextCheck = DateTime.Now.AddDays(30);
                                CommonAttributes.SaveNextDateReg(nextCheck);
                            }
                            // Product works in trial mode for 3 days until next check
                            else if (AllowRun && !IsRegistered)
                            {
                                nextCheck = DateTime.Now.AddDays(3);
                                CommonAttributes.SaveNextDateReg(nextCheck);
                                if ((SerialNumberIn == string.Empty) || (SerialNumberIn == null))
                                    OfflineAuthorization.DisplayTrialMessage(ProductName, RegSerialField);
                            }
                            else if (!AllowRun) // The product is not registered and trial is over
                            {
#if DEBUG
                                using (StreamWriter fsParams = File.AppendText("C:\\params.txt"))
                                {
                                    fsParams.WriteLine(string.Format("{0} = {1} = {2}", Parameter[0], encrUserID, userID));
                                    fsParams.WriteLine(string.Format("{0} = {1} = {2}", Parameter[1], encrSerialNum, serialNumber));
                                    fsParams.WriteLine(string.Format("{0} = {1} = {2}", Parameter[2], encrProductID, ProductID));
                                    fsParams.WriteLine();
                                }
#endif
                                // Delete the corresponding registry, so program will not run next time in offline mode
                                CommonAttributes.DelKeyRegistry(CommonAttributes.RegNewDate);
                                // If any serial number was get from registry, which is validated as wrong,
                                // set the registry to "0", which means trial and allows entering a correct serial number
                                if ((SerialNumberIn == string.Empty) || (SerialNumberIn == null))
                                    CommonAttributes.SaveToRegistry(RegSerialField, "0");
                            }
                        }
                        else
                            throw new GetResponceException();

                        // Reading of user data if is returned
                        while ((line = reader.ReadLine()) != null)
                        {
                            string fieldDelimiter, field;
                            int positionField;
                            fieldDelimiter = line.Substring(1, 2); // The delimiter which must be ::
                            field = line.Substring(3); // The remaining text
                            try
                            {
                                //positionField = int.Parse(String.Format("{0}", line[0]));
                                positionField = int.Parse(line.Substring(0, 1)); // The first character must be digit
                                if ((positionField >= 1) && (positionField <= 5) && (fieldDelimiter == "::"))
                                    CommonAttributes.SaveToRegistry(RegIfnoCustomer[positionField - 1], field);
                            }
                            catch (FormatException) // If the first symbol is not a digit, do nothing
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                AllowRun = false;
                IsRegistered = false;
            }
        }
        #endregion

        #region Local Methods (not exported)
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool GetVolumeInformation(string Volume, StringBuilder VolumeName,
            uint VolumeNameSize, out uint SerialNumber, out uint SerialNumberLength,
            out uint flags, StringBuilder fs, uint fs_size);

        // Generates the UserID through the HDD serial number
        private static string GetUserID()
        {
            string UserID = string.Empty;
            uint SerialNumber;

            // Variables that are filled by GetVolumeInformation and are not used
            StringBuilder VolumeName = new StringBuilder(256);
            StringBuilder fstype = new StringBuilder(256);
            uint SerialNumberLenght, flags;
            // Get the HDD Serial Number
            if (GetVolumeInformation("C:\\", VolumeName, (uint)VolumeName.Capacity - 1, out SerialNumber,
                out SerialNumberLenght, out flags, fstype, (uint)fstype.Capacity - 1))
            {
                UserID = SerialNumber.ToString();
            }
            return UserID;
        }
        #endregion
    } // End class

    [ComVisible(false)]
    public class GetSerialNumberException : System.ApplicationException
    {
        public GetSerialNumberException()
        {
        }

        public GetSerialNumberException(string message)
            : base(message)
        {
        }
    }

    [ComVisible(false)]
    public class GetResponceException : System.ApplicationException
    {
        public GetResponceException()
        {
        }

        public GetResponceException(string message)
            : base(message)
        {
        }
    }
} // End namespace
