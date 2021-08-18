using System;

namespace GPS_QC
{
	/// <summary>
	/// Matrix에 대한 요약 설명입니다.
	/// </summary>
	public class Matrix
	{
		public double[,] Data;
		public Matrix(int size)
		{
			this.Data = new double[size,size];			
		}
		public Matrix(int rows,int cols)
		{
			this.Data = new double[rows,cols];			
		}
		public Matrix(double[,] data)
		{			
			Data=data;			
		}
		
		
		public static Matrix operator+ (Matrix M1, Matrix M2)
		{
			int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
			int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
			if ((r1!=r2)||(c1!=c2))
			{
				throw new System.Exception("Matrix dimensions donot agree");  
			}
			double[,] res = new double[r1,c1]; 
			for (int i=0;i<r1;i++)
			{
				for (int j=0;j<c1;j++)
				{
					res[i,j]=M1.Data[i,j]+M2.Data[i,j];				
				}		
			}
			return new Matrix(res);	
		}
		public static Matrix operator- (Matrix M1, Matrix M2)
		{
			int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
			int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
			if ((r1!=r2)||(c1!=c2))
			{
				throw new System.Exception("Matrix dimensions donot agree");  
			}
			double[,] res = new double[r1,c1]; 
			for (int i=0;i<r1;i++)
			{
				for (int j=0;j<c1;j++)
				{
					res[i,j]=M1.Data[i,j]-M2.Data[i,j];				
				}		
			}
			return new Matrix(res);	
		}
		public static Matrix operator* (Matrix M1, Matrix M2)
		{
			int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
			int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
			if (c1!=r2)
			{
				throw new System.Exception("Matrix dimensions donot agree");  
			}
			double[,] res = new double[r1,c2]; 
			for (int i=0;i<r1;i++)
			{
				for(int j=0;j<c2;j++)
				{
					for(int k=0;k<r2;k++)						
					{
						res[i,j]=  res[i,j] + (M1.Data[i,k]*M2.Data[k,j]);
					}
				}			
			}
			return new Matrix(res);				
		}

		public static Matrix operator/ (double i,Matrix M)
		{
			return new Matrix(scalmul(i,INV(M.Data)));		
		}
		
		/*public static bool operator== (Matrix M1, Matrix M2)
		{
			bool B=true;
			int r1 = M1.Data.GetLength(0);int r2 = M2.Data.GetLength(0);  
			int c1 = M1.Data.GetLength(1);int c2 = M2.Data.GetLength(1);  
			if ((r1!=r2)||(c1!=c2))
			{
				return false;
			}
			else
			{
				for (int i=0;i<r1;i++)
				{
					for (int j=0;j<c1;j++)
					{
						if(M1.Data[i,j]!=M2.Data[i,j])
							B=false;
					}		
				}		
			}
			return B;
		}
		public static bool operator!= (Matrix M1, Matrix M2)
		{
			return !(M1==M2);		
		}
		public override bool Equals(object obj)
		{
			if (!(obj is Matrix))
			{
				return false;
			}
			return this==(Matrix)obj;
		}*/	

		public  double this[int i, int j]
		{
			get
			{
				return this.Data[i,j]; 
			}
			set
			{
				this.Data[i,j]=value;
			}
		}

		public void Display()
		{
			int r1 = this.Data.GetLength(0);int c1 = this.Data.GetLength(1);
			for (int i=0;i<r1;i++)
			{
				for (int j=0;j<c1;j++)
				{
					Console.Write(this.Data[i,j].ToString("N2")+"   " );				
				}
				Console.WriteLine(); 
			}
			Console.WriteLine(); 
		}
		public void Display(string format)
		{
			int r1 = this.Data.GetLength(0);int c1 = this.Data.GetLength(1);
			for (int i=0;i<r1;i++)
			{
				for (int j=0;j<c1;j++)
				{
					Console.Write(this.Data[i,j].ToString(format)+"   " );				
				}
				Console.WriteLine(); 
			}
			Console.WriteLine(); 
		}

		public Matrix Inverse ()
		{
			//if ((this.IsSquare)&&(!this.IsSingular))
			//{
				return new Matrix(INV(this.Data));
			//}
			//else
			//{
			//	throw new System.Exception (@"Cannot find inverse for non square /singular matrix"); 
			//}
		}
		
		public Matrix Transpose()
		{
			double[,] D = transpose(this.Data) ;
			return new Matrix (D);
		}
		public static Matrix Zeros(int size)
		{
			double[,] D = new double[size,size];
			return new Matrix(D);
		}
		public static Matrix Zeros(int rows, int cols)
		{
			double[,] D = new double[rows,cols];
			return new Matrix(D); 
		}
		public Matrix LinSolve(Matrix COF, Matrix CON)
		{
			return COF.Inverse()*CON;		
		}
		public double Det()
		{
			if(this.IsSquare)
			{
				return det(this.Data);
			}
			else
			{
				throw new System.Exception ("Cannot Determine the DET for a non square matrix"); 
			}
		}

		public bool IsSquare
		{
			get
			{
				return (this.Data.GetLength(0) == this.Data.GetLength(1));
			}
		}

		public bool IsSingular
		{
			get
			{
				return ((int)this.Det()==0);
			}
		}

		public int rows{get {return this.Data.GetLength(0);}}
		public int cols{get {return this.Data.GetLength(1);}}
					

		static double[,] INV (double[,] a )
		{
			int ro = a.GetLength(0);
			int co = a.GetLength(1);
			try
			{
				if (ro!=co)	{throw new System.Exception();}
				
			}
			catch{Console.WriteLine("Cannot find inverse for an non square matrix");}
			
			int q;double[,] b = new double[ro,co];double[,] I = eyes(ro);
			for(int p=0;p<ro;p++){for(q=0;q<co;q++){b[p,q]=a[p,q];}}			
			int i;double det=1;	
			if (a[0,0]==0)
			{
				i=1;
				while (i<ro)
				{
					if (a[i,0]!=0)
					{
						Matrix.interrow(a,0,i);		
						Matrix.interrow(I,0,i);
						det *= -1;
						break;
					}
					i++;
				}			
			}
			det*= a[0,0];
			Matrix.rowdiv(I,0,a[0,0]);
			Matrix.rowdiv(a,0,a[0,0]);
			for (int p=1;p<ro;p++)
			{
				q=0;
				while(q<p)
				{
					Matrix.rowsub(I,p,q,a[p,q]);
					Matrix.rowsub(a,p,q,a[p,q]);
					q++;
				}
				if(a[p,p]!=0)
				{
					det*=a[p,p];
					Matrix.rowdiv (I,p,a[p,p]); 
					Matrix.rowdiv (a,p,a[p,p]); 
					
				}
				if(a[p,p]==0)
				{
					for(int j=p+1;j<co;j++)
					{
						if(a[p,j]!=0)			// Column pivotting not supported
						{
							for(int p1=0;p1<ro;p1++)
							{
								for(q=0;q<co;q++)
								{
									a[p1,q]=b[p1,q];
								}
							}
							return inverse(b);
							  							
						}
					}
		
				}
			}
			for (int p=ro-1;p>0;p--)
			{
				for(q=p-1;q>=0;q--)
				{
					Matrix.rowsub (I,q,p,a[q,p]);
					Matrix.rowsub (a,q,p,a[q,p]);
				}
			}						
			for(int p=0;p<ro;p++)
			{
				for(q=0;q<co;q++)
				{
					a[p,q]=b[p,q];
				}
			}
			
			return(I);			
		}
		static double[,] inverse (double[,] a)
		{
			int ro = a.GetLength(0);
			int co = a.GetLength(1);
			double[,] ai = new double[ro,co];
			for (int p=0;p<ro;p++)
			{
				for (int q=0;q<co;q++)
				{
					ai[p,q]=0;
				}
			}
			try
			{
				if (ro!=co)
				{
					throw new System.Exception();

				}
			}
			catch
			{
				Console.WriteLine("Cannot find inverse for an non square matrix");		
				

			}
			double de = det(a);
			
			try
			{
				if (de==0)
				{
					System.Exception e1 = new Exception("Cannot Perform Inversion. Matrix Singular");
					
				}
			}
			catch(Exception e1)
			{
				Console.WriteLine ("Error:"+e1.Message );
			}
			
			
			for(int p=0;p<ro;p++)
			{
				for (int q=0;q<co;q++)
				{
					double [,] s = submat(a,p,q);
					double ds = det(s);
					ai[p,q]=Math.Pow(-1,p+q+2)*ds/de;
				
				}				
			}
			ai=transpose(ai);
			return(ai);
		}

		static void rowdiv(double[,] a,int r, double s )
		{
			int co=a.GetLength(1);
			for(int q=0;q<co;q++)
			{
				a[r,q]=a[r,q]/s;
			}
		}
		static void rowsub(double[,] a, int i, int j,double s)
		{
			int co=a.GetLength(1);
			for (int q=0;q<co;q++)
			{
				a[i,q]=a[i,q]-(s*a[j,q]);
			}
		}
		static  double[,] interrow (double[,]a ,int i , int j)
		{
			int ro = a.GetLength(0);
			int co = a.GetLength(1);
			double temp =0;
			for (int q=0;q<co;q++)
			{
				temp=a[i,q];
				a[i,q]=a[j,q];
				a[j,q]=temp;
			}
			return(a);
		}
		static double[,] eyes (int n)
		{
			double[,] a= new double[n,n];
			for (int p=0;p<n;p++)
			{
				for (int q=0;q<n;q++)
				{
					if(p==q)
					{
						a[p,q]=1;
					}
					else
					{
						a[p,q]=0;
					}
					
				}
			}
			return(a);
		}
		
		static double[,] scalmul(double scalar,double[,] A)
		{
			int ro = A.GetLength(0);
			int co = A.GetLength(1);
			double[,] B = new double[ro,co];
			for(int p=0;p<ro;p++)
			{
				for(int q=0;q<co;q++)
				{
					B[p,q]= scalar*A[p,q];
				}
			}
			return(B);	
		}
		
		static double det (double[,] a )//new det
		{
			int q=0;
			int ro = a.GetLength(0);
			int co = a.GetLength(1);
			double[,] b = new double[ro,co];
			for(int p=0;p<ro;p++)
			{
				for(q=0;q<co;q++)
				{
					b[p,q]=a[p,q];
				}
			}
			int i=0;
			double det=1;
			try
			{
				if (ro!=co)
				{
					System.Exception E1 = new Exception("Error: Matrix Not Square");
					throw E1;
				}
			}
			catch(Exception E1)
			{
				Console.WriteLine (E1.Message );
			}
			try
			{
				if(ro==0)
				{
					System.Exception E2 = new Exception("Dimesion of the Matrix 0X0");
					throw E2;
				}
			}
			catch(Exception E2)
			{
				Console.WriteLine(E2.Message );
			}
			
			if(ro==2)
			{
				return( (a[0,0]*a[1,1]) - (a[0,1]*a[1,0]) ); 
			}
		
			if (a[0,0]==0)
			{
				i=1;
				while (i<ro)
				{
					if (a[i,0]!=0)
					{
						Matrix.interrow(a,0,i);		//Interchange of rows changes. determinent = determinent * -1
						det *= -1;
						break;
					}
					i++;
				}
			
			}
			if(a[0,0]==0)
			{
				return(0);							//If all the elements in a row or column of matrix are 0, determient is equal to 0
			}
			det*= a[0,0];
			Matrix.rowdiv(a,0,a[0,0]);
			for (int p=1;p<ro;p++)
			{
				q=0;
				while(q<p)							//preparring an upper triangular matrix
				{
					Matrix.rowsub(a,p,q,a[p,q]);
					q++;
				}
				if(a[p,p]!=0)
				{
					det*=a[p,p];					//Dividing the entire row with non zero diagonal element. Multiplying det with that factor.	
					Matrix.rowdiv (a,p,a[p,p]); 
				}
				if(a[p,p]==0)						// Chcek if the diagonal elements are zeros
				{
					for(int j=p+1;j<co;j++)
					{
						if(a[p,j]!=0)
						{
							Matrix.colsub(a,p,j,-1);//Adding of columns donot change the determinent
							
							det *= a[p,p];
							Matrix.rowdiv (a,p,a[p,p]);
							break;
						}
					}
		
				}
				if(a[p,p]==0)						//if diagonal element is still zero, Determinent is zero.
				{
					return(0);
				}
			
				
			}
			
			for(int p=0;p<ro;p++)
			{
				for(q=0;q<co;q++)
				{
					a[p,q]=b[p,q];
				}
			}
			return(det);
		}

		static double[,] submat(double [,] a, int ro, int co)
		{
			int n=a.GetLength(0);
			double [,] c = new double[n-1,n-1];
			int i=0;
			for(int p=0;p<n;p++)
			{				
				int j=0;
				if (p!= ro)
				{
					for(int q=0;q<n;q++)
					{
						if(q!=co)
						{
							c[i,j]=a[p,q];
							j+=1;
						}
					}
					i+=1;
				}
			}
	 		
			return(c);
		}
		
		static double[,] transpose(double[,] a)
		{
			
			int ro= a.GetLength(0);
			int co=a.GetLength (1);
			double[,] c = new double[co,ro];
			for(int p=0;p<ro;p++)
			{
				for(int q=0;q<co;q++)
				{
					c[q,p]=a[p,q];
				}
			
			}
					
			return(c);						
		
		}
		static void colsub(double[,]a,int i,int j,double s)
		{
			int ro = a.GetLength(0);
			int co = a.GetLength(1);
			for(int p=0;p<ro;p++)
			{
				a[p,i]=a[p,i]-(s*a[p,j]);
			}
 	
		}

	}
}
