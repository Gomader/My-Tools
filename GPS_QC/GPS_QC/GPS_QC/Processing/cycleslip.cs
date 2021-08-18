using System;

namespace GPS_QC
{
	/// <summary>
	/// 
	/// </summary>
	public class cycleslip
	{
		int Type_Size, Size, i, j, intC1, intP2, intL1, intL2;


		public double[,,] cs(string[] type_observations, double[,,] obs, double[] obs_time, double[] delta_HEN)
		{
			// 
			// TODO: 여기에 생성자 논리를 추가합니다.
			//

			double v_light = 299792458;
			double f0 = 10.23 * 1000000;
			double f1 = 154 * f0;
			double f2 = 120 * f0;
			double lambda1 = v_light/f1;
			double lambda2 = v_light/f2;
			double alpha1 = f1*f1 / (f1*f1 - f2*f2);
			double alpha2 = 1 - alpha1;
			double[] PP = new double[32];
			double[] PPhi = new double[32];
			Size = obs_time.Length;
			double[,] P = new double[Size, 32];
			double[,] Phi = new double[Size, 32];
			double[,] deltaP = new double[Size, 32];
			double[,] deltaPhi = new double[Size, 32];
			double[,,] cycleslips = new double[Size-2, 32, 4];
			double[,] DP = new double[Size-2, 32];
			double[,] DPhi = new double[Size-2, 32];
			double[,] DDP = new double[Size-4, 32];
			double[,] DDPhi = new double[Size-4, 32];
			
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

			for (i=0; i<Size; i++)
			{
				for (j=0; j<32; j++)
				{
					if (obs[i,j,intL1] != 0)
					{
						if (PP[j] == 0)
						{
							P[i,j] = alpha1*obs[i,j,intC1] + alpha2*obs[i,j,intP2];
							Phi[i,j] = alpha1*lambda1*obs[i,j,intL1]+alpha2*lambda2*obs[i,j,intL2];

							PP[j] = P[i,j];
							PPhi[j] = Phi[i,j];
						}
						else
						{
							P[i,j] = alpha1*obs[i,j,intC1] + alpha2*obs[i,j,intP2];
							deltaP[i,j] = P[i,j] - PP[j];

							Phi[i,j] = alpha1*lambda1*obs[i,j,intL1] + alpha2*lambda2*obs[i,j,intL2];
							deltaPhi[i,j] = Phi[i,j] - PPhi[j];
						}
					}
				}
			}
		
			for (i=1; i<Size-1; i++)
			{
				for (j=0; j<32; j++)
				{
					cycleslips[i-1,j,0] = (deltaP[i-1,j]+deltaP[i+1,j]) - 2*deltaP[i,j];

					if (Math.Abs(cycleslips[i-1,j,0])>280000)
					{
						if (cycleslips[i-1,j,0]>0)
						{
							cycleslips[i-1,j,0] = cycleslips[i-1,j,0] - 299792.458;  //dp
						}
						else
						{
							cycleslips[i-1,j,0] = cycleslips[i-1,j,0] + 299792.458;   //dp
						}
					}

					cycleslips[i-1,j,1] = (deltaPhi[i-1,j]+deltaPhi[i+1,j]) - 2*deltaPhi[i,j];  //Dphi
				}
			}

			for (i=1; i<Size-3; i++)
			{
				for (j=0; j<32; j++)
				{
					cycleslips[i-1,j,2] = (cycleslips[i-1,j,0]+cycleslips[i+1,j,0]) - 2*cycleslips[i,j,0];  //DDP
					cycleslips[i-1,j,3] = (cycleslips[i-1,j,1]+cycleslips[i+1,j,1]) - 2*cycleslips[i,j,1];  //DDPhi
				}
			}







		
		return cycleslips;

		}



	}
}
