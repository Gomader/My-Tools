using System;

namespace GPS_QC
{
	/// <summary>
	/// 
	/// </summary>
	public class ionmp
	{
		int Type_Size, i, j, k, l, intC1, intP2, intL1, intL2;
		int safety = 6;

		public double[,,] mp(string[] type_observations, double[,,] obs, double[] obs_time)
		{
			// 
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
			
			double L1m, L2m;
			double[,,] ion_mp;
			double f1 = 1.57542e9;
			double f2 = 1.2276e9;
			double c = 0.299792458e9;
			double l1 = c / f1;
			double l2 = c / f2;
			double a = Math.Pow((f1 / f2), 2);
			int Size = obs_time.Length;
			double[,] MP1 = new double[Size,32];
			double[,] MP2 = new double[Size,32];
			double[,] MP1_mean = new double[Size,32];
			double[,] MP2_mean = new double[Size,32];
			double[] ini_svs = new double[32];
			double ave;


			Type_Size = type_observations.Length;

			for (i=0; i<Type_Size; i++)
			{
				switch (type_observations[i])
				{
					case "C1":
						intC1 = i;
						break;
					case "P1":
						intC1 = i;
						break;
					case "C2":
						intP2 = i;
						break;
					case "P2":
						intP2 = i;
						break;
					case "L1":
						intL1 = i;
						break;
					case "L2":
						intL2 = i;
						break;
				}
			}

			ion_mp = new double[Size, 32, 4];
			
			for (i=0; i<Size; i++)
			{
				for (j=0; j<32; j++)
				{
					if (obs[i,j,intL1] != 0)
					{
						if(ini_svs[j] == 0)
						{					
							L1m = obs[i,j,intL1] * l1;
							L2m = obs[i,j,intL2] * l2;
							ion_mp[i,j,0] = (a/(a-1))*(L1m-L2m); //ion
							ini_svs[j] = ion_mp[i,j,0];
							ion_mp[i,j,0] = 0;
						}
						else
						{
							L1m = obs[i,j,intL1] * l1;
							L2m = obs[i,j,intL2] * l2;
							ion_mp[i,j,0] = (a/(a-1))*(L1m-L2m); //ion
							ion_mp[i,j,0] = ini_svs[j] - ion_mp[i,j,0];

							if (Math.Abs(ion_mp[i,j,0])>10*safety)// && i<=(num_ion[j]+(int)(Size*0.05)))
							{
								ini_svs[j] = (a/(a-1))*(L1m-L2m); //ion
								
							}

							ion_mp[i,j,1] = ((ion_mp[i, j, 0] - ion_mp[i-1, j , 0]) / (obs_time[i] - obs_time[i-1])) * 60;
							
						}

						MP1[i,j] = obs[i,j,intC1] - L1m * ((2/(a-1))+1) + L2m * (2/(a-1));
						MP2[i,j] = obs[i,j,intP2] - L1m * ((2*a)/(a-1)) + L2m * (((2*a)/(a-1))-1);

						//mp1sum[j,0] = mp1sum[j,0] + MP1[i,j];
						//mp1sum[j,1]++;

						//mp2sum[j,0] = mp2sum[j,0] + MP2[i,j];
						//mp2sum[j,1]++;
							

					}
				}
			}

			for (j=0; j<32; j++)
			{
				i = 0;

				while (i <= Size-1 && i != Size-1)
				{
					if (MP1[i,j] == 0)
					{
						i++;
					}
					else if (Math.Abs(MP1[i,j] - MP1[i+1,j]) < 2*safety)
					{
						l=i;
						ave = 0;

						do
						{
							ave = ave + MP1[i,j];
							i++;
							if (i >= (Size-1))
							{
								break;
							}
						}
						while(Math.Abs(MP1[i,j] - MP1[i+1,j]) < 2*safety);

						ave = ave / (i-l);

						for (k=l; k<=i; k++)
						{
							if (k != Size)
							{
								MP1_mean[k,j] = ave;
							}
						}

					}
					else
					{
						MP1_mean[i,j] = 0;
						i++;
					}

				}
			}


			for (j=0; j<32; j++)
			{
				i = 0;

				while (i <= Size-1 && i != Size-1)
				{
					if (MP2[i,j] == 0)
					{
						i++;
					}
					else if (Math.Abs(MP2[i,j] - MP2[i+1,j]) < 2*safety)
					{
						l=i;
						ave = 0;

						do
						{
							ave = ave + MP2[i,j];
							i++;
							if (i >= (Size-1))
							{
								break;
							}
						}
						while(Math.Abs(MP2[i,j] - MP2[i+1,j]) < 2*safety);

						ave = ave / (i-l);

						for (k=l; k<=i; k++)
						{
							//if (k != Size)
							//{
								MP2_mean[k,j] = ave;
							//}
						}
					}
					else
					{
						MP2_mean[i,j] = 0;
						i++;
					}

				}
			}


			for (i=0; i<Size; i++)
			{
				for (j=0; j<32; j++)
				{
					if (MP1[i,j] != 0)
					{
						ion_mp[i,j,2] = MP1[i,j] - MP1_mean[i,j];
						ion_mp[i,j,3] = MP2[i,j] - MP2_mean[i,j];
					}
				}
			}

			

			/*for (i=0; i<32; i++)
			{
				aveMP[i,0] = mp1sum[i,0] / (mp1sum[i,1]);
				aveMP[i,1] = mp2sum[i,0] / (mp2sum[i,1]);
			}*/


			/*for (i=0; i<Size; i++)
			{
				for (j=0; j<32; j++)
				{
					if (obs[i,j,intL1] != 0)
					{

						if (Math.Abs(ion_mp[i,j,2]) > 2*safety)
						{
							int k=i;
							double dou_sum = MP1[k,j];
							if (i == (Size-1))
							{
							}
							else
							{
								while (Math.Abs(MP1[k+1,j] - MP1[k,j]) < 2*safety)
								{
									k++;
									dou_sum = dou_sum + MP1[k,j];
									if (k == (Size - 1))
									{
										break;
									}
								}
								aveMP[j,0] = dou_sum / (k-j+1);
							}
						}

						if (Math.Abs(ion_mp[i,j,3]) > 2*safety)
						{
							int k=i;
							double dou_sum = MP2[k,j];
							if (i == (Size-1))
							{
							}
							else
							{
								while (Math.Abs(MP2[k+1,j] - MP2[k,j]) < 2*safety)
								{
									k++;
									dou_sum = dou_sum + MP2[k,j];
									if (k == (Size - 1))
									{
										break;
									}
								}
								aveMP[j,1] = dou_sum / (k-j+1);
							}
						}

						int k=i;
						double dou_sum = MP1[k,j];
						while (Math.Abs(MP1[k+1,j] - MP1[k,j]) < 2*safety)
						{
							k++;
							dou_sum = dou_sum + MP1[k,j];
							if (k == (Size - 1))
							{
								break;
							}
						}

						aveMP[j,0] = dou_sum / (k-j+1);

						ion_mp[i,j,2] = MP1[i,j] - aveMP[j,0];
						ion_mp[i,j,3] = MP2[i,j] - aveMP[j,1];

					}
				}
			}*/
				
			/*for (j=0; j<32; j++)
			{
				i=0;
				while (i != Size)
				{	
					while (Math.Abs(MP1[i,j] - MP1[i+1,j]) < (2 * safety) && obs[i,j,intL1] != 0)
					{
						l = i;
						dou_sum = dou_sum + MP1[i,j];
						i++;
					}
					aveMP[j,0] = dou_sum / (l-i+1);
					for (int m=l; m<i; m++)
					{
						ion_mp[m,j,2] = MP1[m,j] - aveMP[j,0];
					}
				}
			}*/

			return ion_mp;
		}
	}
}
