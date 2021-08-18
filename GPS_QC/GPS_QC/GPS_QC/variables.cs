using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS_QC
{
    public class variables
    {
        public variables()
        {
            
        }

        // 파일 이름
        public string obs_filename;
        public string nav_filename;

        
        public double[,] eph;
        public double[] ion_alpha, ion_beta, deltaUTC;
        public int leap_seconds, obseph, intType, epoch_Size;
        public double[] approx_position, delta_HEN, obs_time;
        public string[] type_observations, time_original;
        public double[, ,] obs;
        public string process_name, obs_name, mark_name, mark_number, interval, time_first, time_last, rec_type, ant_type;
        public double[, ,] SkyDops;
        public double[, ,] ion_mp;
        public double[, ,] cycleslips;
        public int[] svss;
        public string sv;
        public double[] XYZ = new double[3];
        public List<string> lPrn = new List<string>();
        public string type_obs;
    }
}
