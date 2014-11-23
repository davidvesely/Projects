using Microsoft.Win32;
//using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AuthorizationLib
{
    [ComVisible(false)]
    public class CommonAttributes
    {
        // Registry path
        public static readonly string RegPath = "SOFTWARE\\CADBEST\\CADBEST SOFT";
        public static readonly RegistryKey RegRoot = Registry.CurrentUser;
        public static readonly string RegNewDate = "NextCheck";
        public static readonly string RegInstalledDate = "InstalledDate";

        public const string DATE_FORMAT = "yyyyMMdd";
        private const string INST = "0";
        private const string NOT_INST = "-1";

        public static void DelKeyRegistry(string SubKey)
        {
            RegistryKey regKey;
            regKey = RegRoot.OpenSubKey(RegPath, true);
            if (regKey != null)
            {
                regKey.DeleteValue(SubKey, false);
                regKey.Close();
            }
        }

        public static void SaveNextDateReg(DateTime Date)
        {
            string dateStr = Date.ToString(DATE_FORMAT);
            string encrDate = Encryption.Encrypt(dateStr);
            SaveToRegistry(RegNewDate, encrDate);
        }

        public static void SaveSNToRegistry(string RegEntry, string Data)
        {
            SaveToRegistry(RegEntry, Encryption.Encrypt(Data));
        }

        public static void SaveInstalledDateRegistry()
        {
            string todayStr = DateTime.Today.ToString(DATE_FORMAT, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            string encrTodayStr = Encryption.Encrypt(todayStr);
            string installedDate = LoadFromRegistry(RegInstalledDate, Registry.LocalMachine);
            if (string.IsNullOrEmpty(installedDate))
            {
                SaveToRegistry(RegInstalledDate, encrTodayStr, Registry.LocalMachine);
            }
        }

        public static void SaveToRegistry(string RegEntry, string Data, RegistryKey Root = null)
        {
            RegistryKey regKey;
            if (Root != null)
                regKey = Root.CreateSubKey(RegPath);
            else
                regKey = RegRoot.CreateSubKey(RegPath);
            regKey.SetValue(RegEntry, Data);
            regKey.Close();
        }

        /// <summary>
        /// Load a value from registry for a given Entry
        /// </summary>
        /// <param name="RegEntry">The registry key, which is loaded</param>
        /// <param name="GivenRoot">If null is passed, the default root used is HKEY_CURRENT_USER,
        /// otherwise, the values are loaded from the given root key</param>
        /// <returns>The value of the entry, null if the entry or path doesn't exists</returns>
        public static string LoadFromRegistry(string RegEntry, RegistryKey GivenRoot = null)
        {
            RegistryKey regKey;
            Object regVal = null;
            string result = null;

            if (GivenRoot == null)
                regKey = RegRoot.OpenSubKey(RegPath, false);
            else
                regKey = GivenRoot.OpenSubKey(RegPath, false);

            if (regKey != null)
            {
                if (RegEntry != null)
                    regVal = regKey.GetValue(RegEntry);
                if (regVal != null)
                    result = (string)regVal;
                regKey.Close();
            }

            return result;
        }

        /// <summary>
        /// Check the HKEY_LOCAL_MACHINE if the product under the given RegEntry is installed,
        /// then check the HKEY_CURRENT_USER for valid serial number
        /// </summary>
        /// <param name="RegEntry">The entry for the current product</param>
        /// <returns>The serial number, if any, otherwise "0" if is trial, or null if it is not installed</returns>
        public static string LoadSNFromRegistry(string RegEntry)
        {
            // Get the installed status of the given product (0 = is installed, -1 = not installed)
            string keyInstalled = LoadFromRegistry(RegEntry, Registry.LocalMachine);
            string keyCurrentUser = null;
            string result = null;
            if (keyInstalled == INST)
            {
                result = keyInstalled;
                // Check the current user root key for the product status
                keyCurrentUser = LoadFromRegistry(RegEntry);
                if (!string.IsNullOrEmpty(keyCurrentUser))
                {
                    if (keyCurrentUser != "0")
                        result = Encryption.Decrypt(keyCurrentUser);
                    else
                        result = keyCurrentUser;
                }
            }
            return result;
        }

        public static int GetRemainingDaysTrial()
        {
            DateTime installedDate = DateTime.MinValue;
            string installedDateStr = LoadFromRegistry(RegInstalledDate, Registry.LocalMachine);
            int daysLeft = 0;
            installedDateStr = Encryption.Decrypt(installedDateStr);
            try
            {
                installedDate = DateTime.ParseExact(installedDateStr, DATE_FORMAT,
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                daysLeft = 14 - (DateTime.Now - installedDate).Days;
                if ((daysLeft < 0) && (daysLeft > 14))
                    daysLeft = 0;
            }
            catch (Exception)
            {
            }

            return daysLeft;
        }

        //public static string GetSerialNumberFromReg(string ProductID)
        //{
        //    string result;
        //    int ProdIndex = -1;
        //    // Find the ProductID index
        //    for (int i = 0; i < ProductIDArr.Length; i++)
        //    {
        //        if (ProductID == ProductIDArr[i])
        //        {
        //            ProdIndex = i;
        //            break;
        //        }
        //    }
        //    if (ProdIndex > -1)
        //        result = CommonAttributes.LoadFromRegistry(RegSerialNumber[ProdIndex]);
        //    else
        //        throw new GetSerialNumberException("Incorrect ProductID in GetSerialNumber");
        //    return result;
        //}

        // *******************************************************************************
        // These are not used, because INNO throwed errors while using exported methods  *
        // *******************************************************************************

        //// Called from installer
        //public static void WriteInstallDateToReg()
        //{
        //    string Today = DateTime.Now.ToString("yyyyMMdd");
        //    string EncryptedToday = Encryption.Encrypt(Today);
        //    SaveToRegistry(RegInstalledDate, EncryptedToday);
        //}

        //// Called from installer
        //public static void WriteSerialNumRegistry(string productSM, string productZM, string productZITR, string productTRP)
        //{
        //    SaveToRegistry(RegSerialNumber[0], productSM);
        //    SaveToRegistry(RegSerialNumber[1], productZM);
        //    SaveToRegistry(RegSerialNumber[2], productZITR);
        //    SaveToRegistry(RegSerialNumber[3], productTRP);
        //}
    }
}
