using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVCT_Windows
{
    class IniFunc
    {
        public static string ExPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + "\\log.ini";

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string defval,
            StringBuilder retval,
            int size,
            string filepath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section,
            string key,
            string value,
            string filePath);

        public static string ReadInit(string section, string key, string defValue)
        {
            StringBuilder retValue = new StringBuilder();
            GetPrivateProfileString(section, key, defValue, retValue, 256, ExPath).ToString();
            return retValue.ToString();
        }

        public static void WriteInit(string section, string key,string value)
        {
            WritePrivateProfileString(section, key, value, ExPath);
        }
    }
}
