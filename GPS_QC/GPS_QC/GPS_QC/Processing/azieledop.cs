using System;




namespace GPS_QC
{
	/// <summary>
	/// azieledop에 대한 요약 설명입니다.
	/// </summary>
	/// 
	
	public class azieledop
	{
		

		
		public double[,,] azieledops(double[] obs_time, double[,] eph, double[] approx_position, double[,,] obs)
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
			
			int intSize = obs_time.Length;
			int col, i, j, k;
			double[] sat_pos = new double[3];
			double[] sel_pos = new double[intSize];
			double[] AzEl = new double[2];
			double[] sel_eph = new double[21];
			double[,] sat_poss = new double[3,20];
			double[] Dops = new double[5];
			double[,,] SkyDops = new double[intSize, 37, 2];
			
			
			
			for (i=0; i<intSize; i++)
			{
				k = 0;
				for(j=0; j<=31; j++)
				{
					
					if (obs[i,j,0] != 0)
					{
						col = find_eph(eph, j+1, obs_time[i]);
						

						sat_pos = satpos(obs_time[i], eph, col);
						sat_poss[0,k] = sat_pos[0];
						sat_poss[1,k] = sat_pos[1];
						sat_poss[2,k] = sat_pos[2];
						k++;

						AzEl = azi_ele(sat_pos, approx_position);
						SkyDops[i, j, 0] = Math.Abs(AzEl[0] * (180/Math.PI));
						SkyDops[i, j, 1] = Math.Abs(AzEl[1] * (180/Math.PI));
						
					}

				}
				
				Dops = Dop(sat_poss, approx_position, k);
				SkyDops[i,32,0] = Dops[0];
				SkyDops[i,33,0] = Dops[1];
				SkyDops[i,34,0] = Dops[2];
				SkyDops[i,35,0] = Dops[3];
				SkyDops[i,36,0] = Dops[4];
			
			}

			
			return SkyDops;  
		}

		private double[] Dop(double[,] sat_poss, double[] approx_position, int k)
		{
			double[] r = new double[15];
			double[] Dx = new double[15];
			double[] Dy = new double[15];
			double[] Dz = new double[15];
			double[] Dt = new double[15];
			
	
			for (int l=0; l<k; l++)
			{
				r[l] = Math.Sqrt((Math.Pow((sat_poss[0,l]-approx_position[0]),2))+(Math.Pow((sat_poss[1,l]-approx_position[1]),2))+(Math.Pow((sat_poss[2,l]-approx_position[2]),2)));
				Dx[l] = (sat_poss[0,l]-approx_position[0])/r[l];
				Dy[l] = (sat_poss[1,l]-approx_position[1])/r[l];
				Dz[l] = (sat_poss[2,l]-approx_position[2])/r[l];
				Dt[l] = -1;
			}

			Matrix A = new Matrix(k, 4);

			//double[,] A = new double[k,4];
			for (int n=0; n<k; n++)
			{
				A[n,0] = Dx[n];
				A[n,1] = Dy[n];
				A[n,2] = Dz[n];
				A[n,3] = Dt[n];
			}

			Matrix B = new Matrix(4, k);
			B = A.Transpose();

			Matrix C = B * A;
			Matrix D = C.Inverse();
			
			

			/*double[,] B = new double[4,k];
			for (int n=0; n<k; n++)
			{
				for(int m=0; m<4; m++)
				{
					B[m,n] = A[n,m];
				}
			}*/

			

			/*double[,] C = new double[4,4];

			for (int i=1; i<=4; i++)
			{
				for (int j=1; j<=4; j++)
				{
					for (int o=1; o<=k; k++)
					{
						C[i-1,j-1] += B[i-1,o-1]*A[o-1,j-1];
					}
				}
			}

			double[,] D = new double[4,4];

			D = inv(C);*/

			double GDOP = Math.Sqrt(D[0,0]+D[1,1]+D[2,2]+D[3,3]);
			double PDOP = Math.Sqrt(D[0,0]+D[1,1]+D[2,2]);
			double HDOP = Math.Sqrt(D[0,0]+D[1,1]);
			double VDOP = Math.Sqrt(D[0,0]);
			double TDOP = Math.Sqrt(D[3,3]);
			
			double[] G = new double[5];
			G[0] = GDOP;
			G[1] = PDOP;
			G[2] = HDOP;
			G[3] = VDOP;
			G[4] = TDOP;
			
			return G;


		}

		/*private double[,] inv(double[,] C)
		{

			double[,] E = new double[4,8];
			double a;

			for (int i=0; i<4; i++)
			{
				for (int j=0; j<8; j++)
				{
					if (j<4)
					{
						E[i,j] = C[i,j];
					}
					else
					{
						if (j==(3+i))
						{
							E[i,j] = 1;
						}
						else
						{
							E[i,j] = 0;
						}
					}
				}
			}
			
			
			for (int m=0; m<4; m++)
			{
				for (int i=0; i<4; i++)
				{
					if(m==i)
					{
						for (int j=0; j<8; j++)
						{
							E[i,i] -= E[m,j];
						}
					}
					else
					{
						a = E[i,m]/E[m,m];
					}
				}
			}

			for (int i=0; i<4; i++)
			{
				for (int j=4; j<8; j++)
				{
					E[i,j] /= E[i,i];
				}
			}

			double[,] D = new double[4,4];
			for (int i=0; i<4; i++)
			{
				for (int j=0; j<4; j++)
				{
					D[i,j] = E[i,j+4];
				}
			}

			

			return D;


		}*/




		public double[] azi_ele(double[] sat_pos, double[] approx_position)
		{

			double x,y,z,p,R;
			double[,] e = new double[3,3];
			x = approx_position[0];
			y = approx_position[1];
			z = approx_position[2];

			p = Math.Sqrt(x*x + y*y);
			R = Math.Sqrt(x*x + y*y + z*z);

			e[0,0] = -y/p;
			e[0,1] = x/p;
			e[0,2] = 0;
			e[1,0] = -x*z / (p*R);
			e[1,1] = -y*z / (p*R);
			e[1,2] = p/R;
			e[2,0] = x/R;
			e[2,1] = y/R;
			e[2,2] = z/R;
			
			double[] d = new double[3];

			for (int k=1; k<=3; k++)
			{
				d[k-1] = 0;
				for (int i=1; i<=3; i++)
				{
					d[k-1] = d[k-1] + (sat_pos[i-1]-approx_position[i-1])*e[k-1,i-1];
				}
			}

			double s = d[2] / Math.Sqrt(d[0]*d[0] + d[1]*d[1] + d[2]*d[2]);

			double El;

			if (s==1)
				El = 0.5 * Math.PI;
			else
				El = Math.Atan(s/Math.Sqrt(1-s*s));
			
			double Az;
			if (d[1] == 0 && d[0]>0)
				Az = 0.5 * Math.PI;
			else if (d[1] == 0 && d[0]<0)
				Az = 1.5 * Math.PI;
			else
			{
				Az = Math.Atan(d[0]/d[1]);

				if (d[1]<0)
					Az = Az + Math.PI;
				else if (d[1]>0 && d[0]<0)
					Az = Az + 2*Math.PI;
			}
			double[] AzEl = new double[2];
			AzEl[0] = Az;
			AzEl[1] = El;

			return AzEl;

		}

		
		private double[] satpos(double obs_time, double[,] eph, int col)
		{
			double GM = 3.986008e14;
			double Omegae_dot = 7.2921151467e-5;
			double svprn, af2, M0, roota, deltan, ecc, omega, cuc, cus, crc, crs, i0, idot, cic, cis, Omega0, Omegadot, toe, af0, af1, toc;

			svprn = eph[1, col];
			af2 = eph[2, col];
			M0 = eph[3, col];
			roota = eph[4, col];
			deltan = eph[5, col];
			ecc = eph[6, col];
			omega = eph[7, col];
			cuc = eph[8, col];
			cus = eph[9, col];
			crc = eph[10, col];
			crs = eph[11, col];
			i0 = eph[12, col];
			idot = eph[13, col];
			cic = eph[14, col];
			cis = eph[15, col];
			Omega0 = eph[16, col];
			Omegadot = eph[17, col];
			toe = eph[18, col];
			af0 = eph[19, col];
			af1 = eph[20, col];
			toc = eph[21, col];

			double A = roota*roota;
			double tk;
			tk = check_t(obs_time-toe);
			double n0 = Math.Sqrt(GM/(A*A*A));
			double n = n0+deltan;
			double M = M0+n*tk;
			M = (M+2*Math.PI) % (2*Math.PI);
			double E = M;
			
			double E_old, dE;
			for (int p=1; p<=10; p++)
			{
				E_old = E;
				E = M+ecc*Math.Sin(E);
				dE = (E-E_old)%(2*Math.PI);
				if (Math.Abs(dE) < 1e-12)
					break;
			}
			E = (E+2*Math.PI)%(2*Math.PI);
			double v = Math.Atan2(Math.Sqrt(1-ecc*ecc)*Math.Sin(E), Math.Cos(E)-ecc);
			double phi = v + omega;
			phi = phi % (2*Math.PI);
			double u = phi + cuc*Math.Cos(2*phi)+cus*Math.Sin(2*phi);
			double r = A * (1-ecc*Math.Cos(E)) + crc*Math.Cos(2*phi) + crs*Math.Sin(2*phi);
			double i = i0+idot*tk + cic*Math.Cos(2*phi)+cis*Math.Sin(2*phi);

			double Omega = Omega0+(Omegadot-Omegae_dot)*tk-Omegae_dot*toe;
			Omega = (Omega+2*Math.PI)%(2*Math.PI);
			double X1 = Math.Cos(u)*r;
			double Y1 = Math.Sin(u)*r;
			
			double[] satp = new double[3];

			satp[0] = X1*Math.Cos(Omega)-Y1*Math.Cos(i)*Math.Sin(Omega);
			satp[1] = X1*Math.Sin(Omega)+Y1*Math.Cos(i)*Math.Cos(Omega);
			satp[2] = Y1*Math.Sin(i);


			return satp;
		}


		private double check_t(double t)
		{
			int half_week = 302400;
			double tt = t;
			if (t>half_week)
				tt = t-2*half_week;
			if (t<-half_week)
				tt = t+2*half_week;
			return tt;
		}


		private int find_eph(double[,] eph, int svs, double obs_time)
		{
			int isat;
			isat = eph.Length/22;
			int k=0;

			int[] intSam;
			intSam = new int[30];

			for (int i=1; i<=isat; i++)
			{
				if (eph[1,i-1] == svs)
				{
					intSam[k] = i-1;
					k++;
				
				}
			}

			isat = intSam[0];
			double dtmin = eph[18, intSam[0]] - obs_time;
			for (int l=0; l<k; l++)
			{
				double dt = eph[18,intSam[l]] - obs_time;
				if (dt<0)
				{
					if (Math.Abs(dt) < Math.Abs(dtmin))
					{
						isat = intSam[l];

					}
				}


			}
			

			return isat;

		}

		public double[] convertXYZtoGEO(double[] Xi)
		{
			double[] GEO = new double[3];
			double a = 6378137;
			double b = 6356752.31;
			double e1sqr = (a*a - b*b) / (a*a);
			double e2sqr = (a*a - b*b) / (b*b);

			double p = Math.Sqrt(Xi[0]*Xi[0] + Xi[1]*Xi[1]);
			double T = Math.Atan((Xi[2]*a)/(p*b));
			double sT = Math.Sin(T);
			double cT = Math.Cos(T);
			GEO[0] = Math.Atan((Xi[2] + e2sqr * b * sT * sT * sT) / (p - e1sqr * a * cT * cT * cT));
			
			double sig;
			if (Xi[1] != 0)
				sig = Xi[1] / Math.Abs(Xi[1]);
			else
				sig = 1;
			

			if (Xi[0] == 0)
				GEO[1] = sig*Math.PI/2;
			else
			{
				GEO[1] = Math.Atan(Xi[1]/Xi[0]);
    
				if (Xi[0]<0 & Xi[1] >= 0)
					GEO[1] = GEO[1] + Math.PI;
			
				if (Xi[0]<0 & Xi[1]<0)
					GEO[1] = GEO[1] - Math.PI;
			}
			
			double N = a/Math.Sqrt(1-e1sqr*Math.Sin(GEO[0])*Math.Sin(GEO[0]));
			GEO[2] = p/Math.Cos(GEO[0])-N;
			GEO[0] = GEO[0] * 180 / Math.PI;
			GEO[1] = GEO[1] * 180 / Math.PI;
			return GEO;
		}
	
	}
}
