using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Du_Doan_Cong_Suat_Phat___Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create
        (string.Format("https://www.nldc.evn.vn/Renewable/Scada/GetScadaNhaMay?start=20200514000000&end=20200515000000&idNhaMay=362"));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            /*            if (WebResp.StatusCode.ToString() != "200")
                        {
                            Console.WriteLine("Can't get data !!!");
                        }*/

            Stream json = WebResp.GetResponseStream();
            StreamReader json_str = new StreamReader(json);
            string str = json_str.ReadToEnd();

            JavaScriptSerializer jss = new JavaScriptSerializer();
            SoLieu obj = jss.Deserialize<SoLieu>(str);
            Console.WriteLine("sucess : " + obj.success);

            

            for (int i = 0; i < obj.data.Count; i++)
            {
                if (obj.data[i].capacity == 0 || obj.data[i].ghi == 0)
                {
                    obj.data.RemoveAt(i--);
                }
                else
                {
                    Console.WriteLine(obj.data[i]);
                }
              //  Console.WriteLine(obj.data[i]);
            }
            Console.WriteLine("message : " + obj.message);
            int numberTraining = obj.data.Count / 10 * 8;
            int numberTest = obj.data.Count - numberTraining;

            int number = obj.data.Count;
            MaTran X = new MaTran(number, 3);
            /*X.matrix[0, 0] = 1;
            X.matrix[0, 1] = 2;
            X.matrix[0, 2] = 3;

            X.matrix[1, 0] = -2;
            X.matrix[1, 1] = 4;
            X.matrix[1, 2] = 0;

            X.matrix[2, 0] = 4;
            X.matrix[2, 1] = -5;
            X.matrix[2, 2] = 7;*/


            MaTran Y = new MaTran(number, 1);
            /*
            Y.matrix[0, 0] = 2;
            Y.matrix[1, 0] = 3;
            Y.matrix[2, 0] = 4;*/
            for (int i = 0; i < number; i++)
            {
                X.matrix[i, 0] = 1;
                X.matrix[i, 1] = obj.data[i].ghi;
                X.matrix[i, 2] = obj.data[i].envtemp;
            }
            for (int i = 0; i < number; i++)
            {
                Y.matrix[i, 0] = obj.data[i].capacity;
            }

            MaTran XChuyenVi = X.matrixChuyenVi();

            MaTran Tich2MaTran_1 = MaTran.Tich2MaTran(XChuyenVi, X);
            MaTran Tich2MaTran_2 = MaTran.Tich2MaTran(XChuyenVi, Y);
            MaTran NghichDao = Tich2MaTran_1.MaTranNghichDao();
            MaTran rs = MaTran.Tich2MaTran(NghichDao, Tich2MaTran_2);

            /*Console.WriteLine("Ma Tran X : \n" + X.ToString());
            Console.WriteLine("Ma Tran Y : \n" + Y.ToString());

            Console.WriteLine("Ma Tran Chuyen Vi Cua X : \n" + XChuyenVi.ToString());

            Console.WriteLine("Tich chuyen Vi cua X va X : \n" + Tich2MaTran_1.ToString());

            Console.WriteLine("Dinh thuc : \n" + MaTran.DinhThucCuaMaTranVuong(Tich2MaTran_1));

            Console.WriteLine("Tich chuyen Vi cua X Va Y: \n" + Tich2MaTran_2.ToString());

            Console.WriteLine("Nghich dao : : \n" + NghichDao.ToString());

            Console.WriteLine("Ket qua : : \n" + rs.ToString());*/
            Console.WriteLine("Number of data traing :" + numberTraining);
            Console.WriteLine("Number of data test :" + numberTest);

            double saisoMAE = 0;
            double saisoMSE = 0;
            double saisoRMSE = 0;
            double saisoMAPE = 0;

            int numberTestMAPE = numberTest;
            for(int i = numberTraining; i < obj.data.Count; i++)
            {
                double dudoan = obj.data[i].CapacityDuDoan(rs);
                double thucte = obj.data[i].capacity;
                saisoMSE += Math.Pow((dudoan-thucte), 2);
                
                if(thucte != 0)
                {
                    saisoMAPE += Math.Abs((dudoan - thucte) / thucte);
                }
                else
                {
                    numberTestMAPE--;
                }
                
                saisoMAE += Math.Abs(dudoan - thucte);
            }
            saisoMSE /= numberTest;
            saisoMAE /= numberTest;
            saisoMAPE /= numberTestMAPE ;
            saisoRMSE = Math.Sqrt(saisoMSE);
            Console.WriteLine("Sai so du doan theo MAE : " + saisoMAE );
            Console.WriteLine("Sai so du doan theo MSE : " + saisoMSE );
            Console.WriteLine("Sai so du doan theo MAPE : " + saisoMAPE *100 + " %");
            Console.WriteLine("Sai so du doan theo RMSE : " + saisoRMSE);
            Console.ReadLine();
        }
    }
}
