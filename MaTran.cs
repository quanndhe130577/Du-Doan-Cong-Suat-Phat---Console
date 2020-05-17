using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Du_Doan_Cong_Suat_Phat___Console
{
    public class MaTran
    {
        public int soDong { get; set; }
        public int soCot { get; set; }

        public double[,] matrix {get;set;}

        public MaTran(int d, int c)
        {
            soDong = d;
            soCot = c;
            matrix = new double[soDong, soCot];
        }

        public override string ToString()
        {
            string s = "";
            for(int i = 0; i < soDong; i++)
            {
                s += " | ";
                for(int j = 0; j < soCot; j++)
                {
                    s += matrix[i, j] + "    ";
                }
                s += " | \n";
            }
            return s;
        }

        public MaTran matrixChuyenVi()
        {
            //so dong cua matrixChuyenVi = soCot cua matrix
            int _soDong = soCot;
            //so cot cua matrixChuyenVi = soDong cua matrix
            int _soCot = soDong;
            MaTran matrixChuyenVi = new MaTran(_soDong, _soCot);

            for(int i = 0; i< _soDong; i++)
            {
                for(int j = 0; j < _soCot; j++)
                {
                    matrixChuyenVi.matrix[i, j] = matrix[j, i];
                }
            }
            return matrixChuyenVi;
        }

        public double[] GetRow(int i)
        {
            double[] row = new double[soCot];
            for(int j = 0; j < soCot; j++)
            {
                row[j] = matrix[i, j];
            }
            return row;
        }

        public double[] GetCol(int j)
        {
            double[] col = new double[soDong];
            for (int i = 0; i < soDong; i++)
            {
                col[i] = matrix[i, j];
            }
/*            Console.WriteLine(col[0] + " , " + col[1] + " , " + col[2]);*/
            return col;
        }
        public static MaTran Tich2MaTran(MaTran X, MaTran Y)
        {
            if (X.soCot != Y.soDong) return null;
            MaTran tichMaTran = new MaTran(X.soDong, Y.soCot);
            for(int i = 0; i < tichMaTran.soDong; i++)
            {
                for(int j = 0; j < tichMaTran.soCot; j++)
                {
                    tichMaTran.matrix[i, j] = MaTran.TichVoHuong2Vector(X.GetRow(i), Y.GetCol(j), X.soCot);
                }
            }
            return tichMaTran;
        }

        public static double TichVoHuong2Vector(double[] a, double[] b, int n)
        {
            double result = 0;
            for(int i = 0; i < n; i++)
            {
                //Console.WriteLine(a[i] + " , "+ b[i]);
                result += a[i] * b[i];
            }
            //Console.WriteLine(result);
            return result;
        }

        public static double DinhThucCuaMaTranVuong(MaTran x)
        {
            int number = x.soDong;
            double rs = 1;
            MaTran y = MaTran.BienDoiMaTran(x);
            for(int i = 0; i < number; i++)
            {
                rs *= y.matrix[i, i];
            }
            return rs;
        }

        public static MaTran BienDoiMaTran( MaTran x)
        {
            int soDong = x.soDong;
            int soCot = x.soCot;
            MaTran rs = MaTran.CopyPast(x);
            for(int i = 0; i < soDong; i++)
            {
                for(int j = i+1; j < soDong; j++)
                {
                    int count = 0;
                    double n = rs.matrix[j, i] / rs.matrix[i, i];
                    for (int u = 0; u < soCot; u++)
                    {
                        rs.matrix[j, u] = rs.matrix[j, u] - rs.matrix[i, u] * n ;
                        if (rs.matrix[j, u] == 0) count++;
                        if (count == soCot) return rs;
                    }
                }
            }
            return rs;
        }

        public static MaTran CopyPast(MaTran x)
        {
            int soDong = x.soDong;
            int soCot = x.soCot;
            MaTran y = new MaTran(soDong, soCot);
            for(int i = 0; i < soDong; i++)
            {
                for(int j = 0; j < soCot; j++)
                {
                    y.matrix[i, j] = x.matrix[i, j];
                }
            }
            return y;
        }

        public MaTran MaTranNghichDao()
        {
            double dichThuc = MaTran.DinhThucCuaMaTranVuong(this);
            if (dichThuc == 0)
            {
                return null;
            }
            MaTran x = MaTranPhuHop3x3();
            if (x == null) return null;
            MaTran y = new MaTran(soDong, soCot);
            for(int i = 0; i < soDong; i++)
            {
                for(int j = 0; j < soCot; j++)
                {
                    y.matrix[i, j] = 1 / dichThuc * x.matrix[i,j];
                }
            }
            return y;
        }

        public MaTran MaTranPhuHop3x3()
        {
            MaTran rs = new MaTran(3, 3);
            if (soDong == 3 && soDong == 3)
            {
                MaTran y = new MaTran(6, 6);
                for(int i = 0; i < 6; i++)
                {
                    for(int j = 0; j < 6; j++)
                    {
                        if (i < 3 && j < 3)
                        {
                            y.matrix[i, j] = matrix[i, j];
                        }else if(i < 3 && j >= 3)
                        {
                            y.matrix[i, j] = matrix[i, j-3];
                        }
                        else if(i >= 3 && j < 3)
                        {
                            y.matrix[i, j] = matrix[i-3, j];
                        }
                        else
                        {
                            y.matrix[i, j] = matrix[i - 3, j-3];
                        }
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        rs.matrix[j,i] = (y.matrix[i+1,j+1] * y.matrix[i+2,j+2] - y.matrix[i+2,j+1]*y.matrix[i+1,j+2]);
                    }
                }
            }
            return rs;
        }
    }
}
