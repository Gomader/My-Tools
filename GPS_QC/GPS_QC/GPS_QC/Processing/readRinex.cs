using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GPS_QC
{
	/// <summary>
	/// rinex21에 대한 요약 설명입니다.
	/// </summary>
	public class readRinex
	{
		public readRinex()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

        public string GNSS_Type;
		public double[,] eph;
		public double[] ion_alpha, ion_beta, deltaUTC; 
		public int leap_seconds, obseph, intType, epoch_Size;
		public double[] approx_position, delta_HEN, obs_time;
		public string[] type_observations, time_original;
		public double[,,] obs;
		public string process_name,obs_name,mark_name,mark_number,interval,time_first,time_last,rec_type,ant_type,type_obs;
		public bool[] svss;
        public List<string> lPrn = new List<string>();
        
		public double[,,] observation(string obs_filename)
		{
			int i, j, l, k;
			int year, month, day, hour;
			double second, minute;
			string strLine;
			int head_lines = 1;
			approx_position = new double[3];
			delta_HEN = new double[3];
			svss = new bool[32];

			FileStream aFile = new FileStream(obs_filename, FileMode.Open);
			StreamReader sr = new StreamReader(aFile, System.Text.UnicodeEncoding.Default);

			strLine = sr.ReadLine();
			while(strLine != null)
			{
				head_lines ++;
                if (strLine.IndexOf("RINEX VERSION / TYPE") != -1)
                {
					GNSS_Type = strLine.Substring(40, 1);
                }

				if (strLine.IndexOf("PGM / RUN BY / DATE") != -1)
				{
					process_name = strLine.Substring(0, 55);
				}

				if (strLine.IndexOf("OBSERVER / AGENCY") != -1)
				{
					obs_name = strLine.Substring(0, 35);
					
				}

				if (strLine.IndexOf("MARKER NAME") != -1)
				{
					mark_name = strLine.Substring(0, 8);
				}

				if (strLine.IndexOf("MARKER NUMBER") != -1)
				{
					mark_number = strLine.Substring(0, 4);
				}

				if (strLine.IndexOf("INTERVAL") != -1)
				{
					interval = strLine.Substring(4, 6);
				}

				if (strLine.IndexOf("TIME OF FIRST OBS") != -1)
				{
					time_first = strLine.Substring(2, 41);
				}

				if (strLine.IndexOf("TIME OF LAST OBS") != -1)
				{
					time_last = strLine.Substring(2, 41);
				}

				if (strLine.IndexOf("REC # / TYPE / VERS") != -1)
				{
					rec_type = strLine.Substring(0, 55);
				}

				if (strLine.IndexOf("ANT # / TYPE") != -1)
				{
					ant_type = strLine.Substring(0, 55);
				}

				if (strLine.IndexOf("APPROX POSITION") != -1)
				{
					approx_position[0] = double.Parse(strLine.Substring(1,13));
					approx_position[1] = double.Parse(strLine.Substring(15,13));
					approx_position[2] = double.Parse(strLine.Substring(29,13));
				}

				if (strLine.IndexOf("DELTA H/E/N") != -1)
				{
					delta_HEN[0] = double.Parse(strLine.Substring(1,13));
					delta_HEN[1] = double.Parse(strLine.Substring(15,13));
					delta_HEN[2] = double.Parse(strLine.Substring(29,13));
				}

				if (strLine.IndexOf("TYPES OF OBSERV") != -1)
				{
                    type_obs = strLine;
					intType = int.Parse(strLine.Substring(4,2));
					type_observations = new string[intType];
					for (i=1; i<=intType; i++)
					{
						type_observations[i-1] = strLine.Substring(6*i+4,2);
					}
				}

                if (strLine.IndexOf("PRN / # OF OBS") != -1)
                {
                    lPrn.Add(strLine);
                }

				if (strLine.IndexOf("END OF HEADER") != -1)
				{
					break;
				}
				strLine = sr.ReadLine();
			}

            // 에포크 갯수, 데이터 에러 체크
            chech_epochNo(ref sr, ref obseph, intType);

            // 파일 리딩 포인터, 초기화
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            // 파일 리딩 포인터, 헤더 스킵
            for (i = 1; i < head_lines; i++)
            {
                strLine = sr.ReadLine();
            }

            // 에포크당 정보 저장
            EpochInformation(ref sr, obseph, intType, ref obs, ref time_original, GNSS_Type);

            sr.Close();
            aFile.Close();
			return obs;
		}

        private void chech_epochNo(ref StreamReader sr, ref int obseph, int intType)
        {
            try
            {
                obseph = 0;
                int svsnumber = 0;
                string strLine;

                while (true)
                {
                    readEpochHeader(ref sr, ref svsnumber);

                    if (svsnumber == 0)
                        return;

                    readEpochBody(ref sr, intType, svsnumber, ref obseph);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Wrong Data: " + ee.ToString());
            }
        }

        private void readEpochHeader(ref StreamReader sr, ref int svsnumber)
        {
            string strLine = sr.ReadLine();

            if (strLine == null)
            {
                svsnumber = 0;
                return;
            }
			Console.WriteLine(strLine);
            svsnumber = int.Parse(strLine.Substring(30, 2));

            if (svsnumber > 12)
                strLine = sr.ReadLine();
        }

        private void readEpochBody(ref StreamReader sr, int intType, int svsnumber, ref int obseph)
        {
            string strLine;

            if (intType > 5)
            {
                for (int i = 1; i <= svsnumber; i++)
                {
                    strLine = sr.ReadLine();
                    strLine = sr.ReadLine();
                }

                obseph++;
            }
            else
            {
                for (int i = 1; i <= svsnumber; i++)
                {
                    strLine = sr.ReadLine();
                }

                obseph++;
            }
        }

        private void EpochInformation(ref StreamReader sr, int obseph, int intType, ref double[, ,] obs, ref string[] time_original, string GNSS_Type)
        {
            obs_time = new double[obseph];
            obs = new double[obseph, 32, intType];
            double times;
            time_original = new string[obseph];

            for (int i = 1; i <= obseph; i++)
            {
                int[] svs = new int[20];
                bool[] epoch_GPS_YN = new bool[20];

                string strLine = sr.ReadLine();
                int year = int.Parse(strLine.Substring(1, 2));
                int month = int.Parse(strLine.Substring(4, 2));
                int day = int.Parse(strLine.Substring(7, 2));
                int hour = int.Parse(strLine.Substring(10, 2));
                double minute = double.Parse(strLine.Substring(13, 2));
                double second = double.Parse(strLine.Substring(15, 10));

                time_original[i - 1] = strLine.Substring(10, 2) + ":" + strLine.Substring(13, 2) + ":" + strLine.Substring(15, 10);

                times = hour + minute / 60 + second / 3600;
                obs_time[i - 1] = this.gps_time(year, month, day, times);

                int svsnumber = int.Parse(strLine.Substring(30, 2));

                // GPS, GLONASS, Galileo 정보 선별

                // GPS만 수신하는 경우
                if (GNSS_Type == "G")
                {
                    for (int j = 1; j <= svsnumber; j++)
                    {
                        svs[j] = int.Parse(strLine.Substring(3 * j + 30, 2));
                        epoch_GPS_YN[j] = true;

                        if (svss[svs[j] - 1] == false)
                        {
                            svss[svs[j] - 1] = true;
                        }
                    }

                    if (intType > 5)
                    {
                        for (int j = 1; j <= svsnumber; j++)
                        {
                            strLine = sr.ReadLine();
                            for (int l = 0; l < 5; l++)
                            {
                                string Temp = strLine.Substring(16*l, 16);
                                Temp = Temp.Trim();
                                obs[i - 1, svs[j] - 1, l] = Convert.ToDouble(Temp);
                            }
                            strLine = sr.ReadLine();
                            for (int l = 0; l < intType - 5; l++)
                            {
                                string Temp = strLine.Substring(16 * l, 16);
                                Temp = Temp.Trim();
                                obs[i - 1, svs[j] - 1, l + 5] = Convert.ToDouble(Temp);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 1; j <= svsnumber; j++)
                        {
                            strLine = sr.ReadLine();
                            for (int l = 0; l < intType; l++)
                            {
                                string Temp = strLine.Substring(16 * l, 16);
                                Temp = Temp.Trim();
                                obs[i - 1, svs[j] - 1, l] = Convert.ToDouble(Temp);
                            }
                        }
                    }
                }
                // GLONSS 등 섞여있는경우
                else
                {
                    // 에포크 헤더 리딩
                    if (svsnumber > 12)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            if (strLine.Substring(3 * j + 29, 1) == "G") // GPS 신호만 필터링
                            {
                                svs[j] = int.Parse(strLine.Substring(3 * j + 30, 2));
                                epoch_GPS_YN[j] = true;

                                if (svss[svs[j] - 1] == false)
                                {
                                    svss[svs[j] - 1] = true;
                                }
                            }
                        }

                        strLine = sr.ReadLine();

                        for (int j = 1; j <= (svsnumber - 12); j++)
                        {
                            if (strLine.Substring(3 * j + 29, 1) == "G")
                            {
                                svs[j + 12] = int.Parse(strLine.Substring(3 * j + 30, 2));
                                epoch_GPS_YN[j + 12] = true;

                                if (svss[svs[j + 12] - 1] == false)
                                {
                                    svss[svs[j + 12] - 1] = true;
                                }
                            }
                        }

                    }
                    else
                    {
                        for (int j = 1; j <= svsnumber; j++)
                        {
                            if (strLine.Substring(3 * j + 29, 1) == "G")
                            {
                                svs[j] = int.Parse(strLine.Substring(3 * j + 30, 2));
                                epoch_GPS_YN[j] = true;

                                if (svss[svs[j] - 1] == false)
                                {
                                    svss[svs[j] - 1] = true;
                                }
                            }
                        }
                    }

                    // 에포크 바디 리딩
                    for (int j = 1; j <= svsnumber; j++)
                    {
                        if (epoch_GPS_YN[j] == true)
                        {
                            if (intType > 5)
                            {
                                strLine = sr.ReadLine();
                                for (int l = 0; l < 5; l++)
                                {
                                    string Temp = strLine.Substring(16 * l, 16);
                                    Temp = Temp.Trim();
                                    obs[i - 1, svs[j] - 1, l] = Convert.ToDouble(Temp);
                                }
                                strLine = sr.ReadLine();
                                for (int l = 0; l < intType - 5; l++)
                                {
                                    string Temp = strLine.Substring(16 * l, 16);
                                    Temp = Temp.Trim();
                                    obs[i - 1, svs[j] - 1, l + 5] = Convert.ToDouble(Temp);
                                }
                            }
                            else
                            {
                                strLine = sr.ReadLine();
                                for (int l = 0; l < intType; l++)
                                {
                                    string Temp = strLine.Substring(16 * l, 16);
                                    Temp = Temp.Trim();
                                    
                                    if (Temp != "")
                                        obs[i - 1, svs[j] - 1, l] = Convert.ToDouble(Temp);
                                }
                            }
                        }
                        else
                        {
                            if (intType > 5)
                            {
                                strLine = sr.ReadLine();
                                strLine = sr.ReadLine();
                            }
                            else
                            {
                                strLine = sr.ReadLine();
                            }
                        }
                    }
                }
            }
        }

		public double[,] nav_eph(string nav_filename)
		{
			FileStream aFile = new FileStream(nav_filename, FileMode.Open);
			StreamReader sr = new StreamReader(aFile, System.Text.UnicodeEncoding.Default);
			

			string strLine;
			int head_lines = 1;
			
			ion_alpha = new double[4];
			ion_beta = new double[4];
			deltaUTC = new double[4];


			// 네비게이션 파일 중 헤더부분 리딩. 
			strLine = sr.ReadLine();
			while(strLine != null)
			{
				strLine = sr.ReadLine();
				head_lines ++;

				if (strLine.IndexOf("ION ALPHA") != -1)
				{
					ion_alpha[0] = this.DconE(strLine.Substring(4,10));
					ion_alpha[1] = this.DconE(strLine.Substring(16,10));
					ion_alpha[2] = this.DconE(strLine.Substring(28,10));
					ion_alpha[3] = this.DconE(strLine.Substring(40,10));
				}

				if (strLine.IndexOf("ION BETA") != -1)
				{
					ion_beta[0] = this.DconE(strLine.Substring(4,10));
					ion_beta[1] = this.DconE(strLine.Substring(16,10));
					ion_beta[2] = this.DconE(strLine.Substring(28,10));
					ion_beta[3] = this.DconE(strLine.Substring(40,10));
				}

				if (strLine.IndexOf("DELTA-UTC") != -1)
				{
					deltaUTC[0] = this.DconE(strLine.Substring(4,18));
					deltaUTC[1] = this.DconE(strLine.Substring(23,18));
					deltaUTC[2] = this.DconE(strLine.Substring(45,5));
					deltaUTC[3] = this.DconE(strLine.Substring(55,4));
					
				}

				if (strLine.IndexOf("LEAP SECONDS") != -1)
				{
					leap_seconds = int.Parse(strLine.Substring(4,6));
					
				}

				if (strLine.IndexOf("END OF HEADER") != -1)
				{
					break;
				}
			
			}

			int noeph = 1;

			strLine = sr.ReadLine();
			while(strLine != null)
			{
				strLine = sr.ReadLine();
				noeph++;

			}
			

			

			noeph = noeph / 8 ;      //네비게이션 파일 에포크 수, 정수형이 아닐경우 에러 처리 필요.
			

			sr.BaseStream.Seek(0, SeekOrigin.Begin);

			int i;
		
			for (i=1;i<=(head_lines);i++)
			{
				strLine = sr.ReadLine();
			}
			

			
			eph = new double[22, noeph];
			int year, month, day, hour, minute;
			double second;
		

			for (i=0;i<noeph;i++)
			{
				strLine = sr.ReadLine();
				eph[1,i] = double.Parse(strLine.Substring(0,2));
				year = int.Parse(strLine.Substring(3,2));
				month = int.Parse(strLine.Substring(6,2));
				day = int.Parse(strLine.Substring(9,2));
				hour = int.Parse(strLine.Substring(12,2));
				minute = int.Parse(strLine.Substring(15,2));
				second = double.Parse(strLine.Substring(18,4));
				
				eph[21,i] = this.gps_time(year, month, day, hour + minute/60 + second/3600);
				eph[19,i] = this.DconE(strLine.Substring(22,19));
				eph[20,i] = this.DconE(strLine.Substring(41,19));
				eph[2,i] = this.DconE(strLine.Substring(60,19));

				strLine = sr.ReadLine();
				eph[11,i] = this.DconE(strLine.Substring(22,19));
				eph[5,i] = this.DconE(strLine.Substring(41,19));
				eph[3,i] = this.DconE(strLine.Substring(60,19));

				strLine = sr.ReadLine();
				eph[8,i] = this.DconE(strLine.Substring(3,19));
				eph[6,i] = this.DconE(strLine.Substring(22,19));
				eph[9,i] = this.DconE(strLine.Substring(41,19));
				eph[4,i] = this.DconE(strLine.Substring(60,19));

				strLine = sr.ReadLine();
				eph[18,i] = this.DconE(strLine.Substring(3,19));
				eph[14,i] = this.DconE(strLine.Substring(22,19));
				eph[16,i] = this.DconE(strLine.Substring(41,19));
				eph[15,i] = this.DconE(strLine.Substring(60,19));

				strLine = sr.ReadLine();
				eph[12,i] = this.DconE(strLine.Substring(3,19));
				eph[10,i] = this.DconE(strLine.Substring(22,19));
				eph[7,i] = this.DconE(strLine.Substring(41,19));
				eph[17,i] = this.DconE(strLine.Substring(60,19));

				strLine = sr.ReadLine();
				eph[13,i] =this.DconE(strLine.Substring(3,19));
				
				strLine = sr.ReadLine();
				strLine = sr.ReadLine();

				
			}

			sr.Close();

			return eph;
		}
		
		private double gps_time(int year, int month, int day, double time)
		{
			string Year;
			if (year<50)
			{
				if (year<10)
				{
					Year = "200" + Convert.ToString(year);
					year = Convert.ToInt32(Year);
				}
				else
				{
					Year = "20" + Convert.ToString(year);
					year = Convert.ToInt32(Year);
				}
			}
			else
			{
				Year = "19" + Convert.ToString(year);
				year = Convert.ToInt32(Year);
			}


			
			
			
			if (month<=2)
			{
				year--;
				month += 12;
			}
			
			double jd;

			jd = Math.Floor(365.25*(year+4716))+Math.Floor(30.6001*(month+1))+day+time/24-1537.5;

			int a,b,c,e,f,dow;
			double d,sow;

			a = Convert.ToInt32(jd+0.5);
			b = a + 1537;
			c = Convert.ToInt32((b-122.1)/365.25);
			e = Convert.ToInt32(365.25*c);
			f = Convert.ToInt32((b-e)/30.6001);
			d = b-e-Convert.ToInt32(30.6001*f)+(jd+0.5)-Convert.ToInt32(jd+0.5);
			dow = Convert.ToInt32(jd+0.5) % 7;
			sow = (d - Convert.ToInt32(d) + dow + 1) * 86400;

			return sow;
		}

		private double DconE(string strLine)
		{
			strLine = strLine.Replace("D","e");
			double douLine;
			douLine = double.Parse(strLine);
			return douLine;
		}
	}
}
