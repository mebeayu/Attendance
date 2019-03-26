using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Attendance.Common
{
    public class MD5
    {
        public static string GenerateRandomCode(int codeCount)
        {
            string str = string.Empty;
            int num2 = (int)DateTime.Now.Ticks;

            Random random = new Random(num2);
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            string res = s.ToString();
            return res;
        }
        public static string GetMD5HashFromFile(string filePath)
        {

            try
            {

                FileStream file = new FileStream(filePath, FileMode.Open);

                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] retVal = md5.ComputeHash(file);

                file.Close();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < retVal.Length; i++)
                {

                    sb.Append(retVal[i].ToString("x2"));

                }

                return sb.ToString().ToUpper();

            }

            catch (System.Exception ex)
            {

                throw new System.Exception("GetMD5HashFromFile() fail,error:" + ex.Message);

            }

        }
    }
}