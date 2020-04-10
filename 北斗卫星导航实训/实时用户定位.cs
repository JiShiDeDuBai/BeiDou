using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections;
using System.Data.SQLite;

namespace 北斗卫星导航实训
{
    public partial class 实时用户定位 : Form
    {
        public string str = @"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @"GPS_data.db";
        public int n = 4;//轨迹编号
        SerialPort ComDevice = new SerialPort();    //实例化一个串口对象，
        public event EventHandler accept;//传递数据的事件


        public 实时用户定位()
        {
            InitializeComponent();
            cbbComList.Items.AddRange(SerialPort.GetPortNames());    //cbbList是选择端口的列表，将现有计算机上的端口号通过串口函数赋给他
            Map_show();
            this.init();                            //加载初始页面函数
            
        }

        


        //加载页面函数
        public void init ()
        {
            cbbComList.Items.AddRange(SerialPort.GetPortNames());    //将串口的名称加载到列表中
            if (cbbComList.Items.Count > 0)
            {
                cbbComList.SelectedIndex = 0;       //如果有端口，默认第一个是被选中的
            }
            cbbBaudRate.SelectedIndex = 5;
            cbbDataBits.SelectedIndex = 0;
            cbbParity.SelectedIndex = 0;
            cbbStopBits.SelectedIndex = 0;
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定接收数据事件
        }







        


        

        
        //接收数据
        private void Com_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
             byte[] ReDatas = new byte[ComDevice.BytesToRead];    //通过串口函数读数据到比特数组缓存中
             ComDevice.Read(ReDatas, 0, ReDatas.Length);         //从缓存中读数据
             AddData(ReDatas);//输出数据   
           

            
           
           
        }

        
        //....
      

        //添加数据
        public void AddData(byte[] data)
        {
            if (rbtnHex.Checked)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.AppendFormat("{0:x2}" + " ", data[i]);
                }
                AddContent(sb.ToString().ToUpper());
            }
            else if (rbtnASCII.Checked)
            {
                AddContent(new ASCIIEncoding().GetString(data));
            }
            else if (rbtnUTF8.Checked)
            {
                AddContent(new UTF8Encoding().GetString(data));
            }
            else if (rbtnUnicode.Checked)
            {
                AddContent(new UnicodeEncoding().GetString(data));
            }
            else
            { }

        }



        /// 输入到显示区域
        
        private void AddContent(string content)
        {
            BeginInvoke(new MethodInvoker(delegate                       //触发异步操作，一边显示，一边读取串口数据
            {
                
                txtShowData.AppendText(content);
               
            }));
        }


       
    


        private void cbbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        //提取并判断数据函数
        public void ReturnNE( )
        {

                
                string[] str=null;
                
                

                string Tdata = txtShowData.Text;    //
                
                str = Tdata.Split('$');           //先对传入数据进行第一次分割
               
                for(int i = 0; i < str.Length; i++)                //进入循环判断将每一段以$分割的信号在一个循环内以，再分割判断
            {
                string temp = str[i];
                string[] NE = null;                 //用来暂存经纬度和时间的数组
                NE = temp.Split(',');                //第二次分割，获取各个关键字段


                //进入判断流程
                if (NE[0] == "GNGGA")
                {
                    if (NE[6] == "0"||NE[6]=="3")
                    {
                        textBox2.Text = "暂无有效信号";

                    }
                    else
                    {
                        textBox2.Text = "正常定位中";


                        //经纬度转换
                        //纬度转换
                        double []Nn =  Array.ConvertAll(NE[2].Split('.'), double.Parse);                 //提取转化前的纬度,以点为分割存储
                        double ab = Nn[0]/ 100;
                        Nn[0] = Nn[0] % 100;
                        double cd = Nn[0];
                        double efgh = Nn[1];
                        double N = ab + (cd / 60) + (efgh/60000);                        //单位  （度）
                        

                        //经度转换
                        double []Ee= Array.ConvertAll(NE[4].Split('.'), double.Parse);          //提取转化前的纬度
                        double ABC = Ee[0] / 100;
                        Ee[0] = Ee[0] % 100;
                        double DE = Ee[0];
                        double FGHI = Ee[1];
                        double E = ABC + (DE / 60) + (FGHI / 60000);                //单位  （度）


                        double[] tmpNE = Gps84ToGcj02(N, E);
                        N = tmpNE[0];
                        E = tmpNE[1];
                        //将提取的数据存入数据库
                        databese(N, E);
                        textBox3.Text =N.ToString();
                        textBox4.Text = E.ToString();
                        textBox5.Text = NE[9] + "m";//海拔
                        textBox6.Text = NE[7] + "颗";
                    }
                }

            }
                
               
                
                

                    
                   
                
          



        }

        //WGS84转国内Gcj02坐标

        public static double pi = 3.1415926535897932384626;
        public static double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
        public static double aa = 6378245.0;
        public static double ee = 0.00669342162296594323;

        public static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y
                    + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        public static double TransformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1
                    * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0
                    * pi)) * 2.0 / 3.0;
            return ret;
        }

        public static double[] transform(double lat, double lon)
        {
            if (OutOfChina(lat, lon))
            {
                return new double[] { lat, lon };
            }
            double dLat = TransformLat(lon - 105.0, lat - 35.0);
            double dLon = TransformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double SqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((aa * (1 - ee)) / (magic * SqrtMagic) * pi);
            dLon = (dLon * 180.0) / (aa / SqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return new double[] { mgLat, mgLon };
        }

        public static bool OutOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        /** 
            * 84 to 火星坐标系 (GCJ-02) World Geodetic System ==> Mars Geodetic System 
            * 
            * @param lat 
            * @param lon 
            * @return 
            */
        public static double[] Gps84ToGcj02(double lat, double lon)
        {
            if (OutOfChina(lat, lon))
            {
                return new double[] { lat, lon };
            }
            double dLat = TransformLat(lon - 105.0, lat - 35.0);
            double dLon = TransformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double SqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((aa * (1 - ee)) / (magic * SqrtMagic) * pi);
            dLon = (dLon * 180.0) / (aa / SqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return new double[] { mgLat, mgLon };
        }

        //打开串口按键
        private void btnOpen_Click(object sender, EventArgs e)
        {

            if (cbbComList.Items.Count <= 0)
            {
                MessageBox.Show("没有发现串口,请检查芯片连接！");
                return;
            }

            if (ComDevice.IsOpen != true)
            {
                ComDevice.PortName = cbbComList.SelectedItem.ToString();
                ComDevice.BaudRate = Convert.ToInt32(cbbBaudRate.SelectedItem.ToString());
                ComDevice.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                ComDevice.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                ComDevice.StopBits = (StopBits)Convert.ToInt32(cbbStopBits.SelectedItem.ToString());
                try
                {
                    ComDevice.Open();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                btnOpen.Text = "关闭串口";

            }
            else
            {
                try
                {
                    ComDevice.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnOpen.Text = "打开串口";

            }
        }



        private void databese(double a,double b)
        {
            n++;
            string x = a.ToString();
            string y = b.ToString();
            SQLiteConnection conn = new SQLiteConnection(str);
            conn.Open();
            string sql = "insert into Point values('"+n+"','"+x+"','"+y+"')";
            SQLiteCommand sc = new SQLiteCommand(sql,conn);
            sc.ExecuteNonQuery();
            conn.Close();

        }


        
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }


        private void rbtnASCII_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void Map_show()//地图连接加载
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            string path = Application.StartupPath + "\\地图.html";
            webBrowser1.Navigate(path);
            
        }

        //定位按钮
        private void button1_Click(object sender, EventArgs e)
        {
           
            ReturnNE();
        }

        private void txtShowData_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ComDevice.Close();
                MessageBox.Show("已停止记录");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnOpen.Text = "打开串口";

        }

        

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            
            

        }



       

        private void button3_Click(object sender, EventArgs e)
        {
            
            webBrowser1.Document.InvokeScript("setLocation", new object[] { textBox4.Text.ToString(), textBox3.Text.ToString() });
           
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
