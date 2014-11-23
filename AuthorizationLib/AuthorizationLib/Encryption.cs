//using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AuthorizationLib
{
    [ComVisible(false)]
    internal class Encryption
    {
        // Constants
        private const string CapitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SmallLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string ALL_CHARS = Digits + CapitalLetters + Digits + SmallLetters + Digits;

        // Optional seed is added to prevent same values generated one by one very fast
        internal static string Encrypt(string data, int seed = 0)
        {
            string dataOut, dataMixed, dataBinReversed;
            int sumEven = 0, sumOdd = 0, sumAll = 0;
            int strLenght, asciiValue;

            if ((data == null) || (data == ""))
                return "";
            strLenght = data.Length;
            if (strLenght > 9999)
                strLenght = 0000;

            for (int i = 0; i < data.Length; i++)
            {
                //Get value of ASCII code 
                asciiValue = data[i];
                //Sums of ASCII Code Values
                if (((i + 1) % 2) == 0)
                    sumEven += asciiValue; // Sum of Even digit
                if (i % 2 == 0)
                    sumOdd += asciiValue; // Sum of Odd digit                  
                sumAll += asciiValue; // Sum of All
            }

            //Control Sums and processed result
            sumEven = sumEven % 9;
            sumOdd = sumOdd % 9;
            sumAll = sumAll % 9;
            dataOut = String.Format("{0:D4}{1}{2}{3}{4}", strLenght, data, sumEven, sumOdd, sumAll);
            do
            {
                dataMixed = Mixing(dataOut, seed);
                dataBinReversed = BinaryReverse(dataMixed);
                seed += 10;
#if DEBUG
                if (seed > 1000)
                {
                    System.Windows.Forms.MessageBox.Show("Encryption failed! Backslash can't be prevented.");
                    break;
                }
#endif
            } while (dataBinReversed.Contains('\\'));
            return dataBinReversed;
        }

        internal static string Decrypt(string data)
        {
            string code, information;
            StringBuilder sb = new StringBuilder();
            int sumEvenCheck = 0, sumOddCheck = 0, sumAllCheck = 0;

            if ((data == null) || (data == string.Empty))
                return string.Empty;
            information = BinaryReverse(data);
            information = Demixing(information);
            try
            {
                //Get First 4 digit (Length of String)
                int length = int.Parse(information.Substring(0, 4));
                // Get Even Sum Number from (n - 2)th element 
                int sumEven = int.Parse(information[information.Length - 3].ToString());
                // Get Odd Sum Number from (n - 1)th element
                int sumOdd = int.Parse(information[information.Length - 2].ToString());
                // Get All Sum Number from last n-th element
                int sumAll = int.Parse(information[information.Length - 1].ToString());
                // Take the Code
                for (int i = 4; i < information.Length - 3; i++)
                    sb.Append(information[i]);
                code = sb.ToString();

                // Get Sum Values from Code
                for (int i = 0; i < code.Length; i++)
                {
                    // Get value of ASCII code   
                    int a = code[i];
                    // Sums of ASCII Code Values
                    if (((i + 1) % 2) == 0)
                        sumEvenCheck += a; // Even
                    if (i % 2 == 0)
                        sumOddCheck += a; // Odd                  
                    sumAllCheck += a; // All
                }
                //Control Sums from new Code
                sumEvenCheck = sumEvenCheck % 9;
                sumOddCheck = sumOddCheck % 9;
                sumAllCheck = sumAllCheck % 9;
                if (length == 0)
                    length = code.Length;
                // Verifying the Code
                if ((length == code.Length) && (sumEven == sumEvenCheck) &&
                    (sumOdd == sumOddCheck) && (sumAll == sumAllCheck))
                    return code;
                else
                    return string.Empty;
            }
            catch (Exception) // If wrong data is passed to the method an exception will be throwed
            {
                return string.Empty;
            }
        }

        private static string Mixing(string data, int seed)
        {
            StringBuilder buffer = new StringBuilder();
            string result;
            Random randomGenerator;
            if (seed > 0)
                randomGenerator = new Random((int)DateTime.Now.Ticks + seed);
            else
                randomGenerator = new Random();
            int randomIndex;

            for (int i = 0; i < data.Length; i++)
            {
                randomIndex = randomGenerator.Next(0, ALL_CHARS.Length - 1);
                buffer.Append(ALL_CHARS[randomIndex]); // Random symbol - char or number
                buffer.Append(data.Substring(i, 1));
            }
            result = buffer.ToString();
            return result;
        }

        private static string Demixing(string data)
        {
            // Return every 2nd symbol of the source
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                if ((i + 1) % 2 == 0)
                    result.Append(data[i]);
            return result.ToString();
        }

        private static string BinaryReverse(string data)
        {
            int HexFFF0 = 0xFFF0; // FFF0 Works with Unicode, not only with ASCII
            int Hex0F = 0x000F;
            int firstByte, secondByte, resultByteFirst, resultByteSecond;
            char resultCharFirst, resultCharSecond;
            StringBuilder res = new StringBuilder();

            for (int i = 0; i < data.Length - 1; i += 2)
            {
                // First and second char from a string, converted to their int representation
                firstByte = data[i];
                secondByte = data[i + 1];
                // 0000 ... 0000 {0000} and 1111 ... 1111 {1111} 
                // The last four bits are swaped between the chars
                resultByteFirst = (firstByte & HexFFF0) + (secondByte & Hex0F);
                resultByteSecond = (secondByte & HexFFF0) + (firstByte & Hex0F);
                // The new characters are added to new string
                resultCharFirst = (char)resultByteFirst;
                resultCharSecond = (char)resultByteSecond;
                res.Append(String.Format("{0}{1}", resultCharFirst, resultCharSecond));
            }
            return res.ToString();
        }
    }
}
