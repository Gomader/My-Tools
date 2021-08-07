using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVCT_Windows
{
    class SpeechRecognition
    {
        public static string TempPath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + "\\temp.wav";
        public static string Url = "http://asr.cloud.tencent.com/asr/v1/";
        

        private string API()
        {

        }
    }
}
