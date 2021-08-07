using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SVCT_Windows
{
    internal class AudioRecorder
    {
        internal static void RecordToWav(string fileName)
        {
            using (WasapiCapture capture = new WasapiLoopbackCapture())
            {
                capture.Initialize();
                using (WaveWriter w = new WaveWriter(fileName, capture.WaveFormat))
                {
                    capture.DataAvailable += (s, e) =>
                    {
                        //save the recorded audio
                        w.Write(e.Data, e.Offset, e.ByteCount);
                    };

                    //start recording
                    
                    capture.Start();
                    Thread.Sleep(200);
                    //stop recording
                    capture.Stop();
                    
                }
            }
        }
    }
}
