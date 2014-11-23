using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AuthorizationLib
{
    [ComVisible(true)]
    [GuidAttribute("4B01A3E4-6F3B-4076-9600-594BA0553034")]
    public class COMExports : IDotNetAdder
    {
        private FormLoading FormLoadingInstance = null;

        public COMExports()
        {
        }

        public void HelloDllMessage()
        {
            MessageBox.Show("Hello from C# DLL");
        }

        public string StringTest()
        {
            return "returned string from dll";
        }

        public bool CheckConnection(int timeout)
        {
            return OnlineAuthorization.CheckConnection(timeout);
        }

        public void CheckProgramState(string ProductID, string SerialNumberIn,
            string RegSerialField, string ProductName, out Boolean AllowRun, out Boolean IsRegistered)
        {
            bool allowRun, isRegistered;
            OnlineAuthorization.CheckProgramState(ProductID, SerialNumberIn,
                RegSerialField, ProductName, out allowRun, out isRegistered);
            AllowRun = allowRun;
            IsRegistered = isRegistered;
        }

        public bool CheckSerialNumberRegistry(string productName, string productSerialRegistry)
        {
            return OfflineAuthorization.CheckSerialNumberRegistry(productName, productSerialRegistry);
        }

        public void SaveInstalledDateRegistry()
        {
            CommonAttributes.SaveInstalledDateRegistry();
        }

        public void FormLoadingShow()
        {
            FormLoadingInstance = new FormLoading();
            FormLoadingInstance.Show();
            FormLoadingInstance.Update();
        }

        public void FormLoadingHide()
        {
            FormLoadingInstance.Close();
            FormLoadingInstance.Dispose();
            FormLoadingInstance = null;
        }
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("ACEEED92-1A35-43fd-8FD8-9BA0F2D7AC31")]
    public interface IDotNetAdder
    {
        void HelloDllMessage();
        string StringTest();
        bool CheckConnection(int timeout);
        void CheckProgramState(string ProductID, string SerialNumberIn,
            string RegSerialField, string ProductName, out Boolean AllowRun, out Boolean IsRegistered);
        bool CheckSerialNumberRegistry(string productName, string productSerialRegistry);
        void SaveInstalledDateRegistry();

        // Form show and hide methods
        void FormLoadingShow();
        void FormLoadingHide();
    }
}
