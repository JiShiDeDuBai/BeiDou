using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace 北斗卫星导航实训
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//为了能与JS代码交互的预先设定
   

    public partial class Form1 : Form
    {
        public 实时用户定位 tool1;
        public 轨迹操作 tool2;
        public 关于 tool3;
        public 地图文件加载 tool4;
        private string str = @"Data Source = " + AppDomain.CurrentDomain.BaseDirectory + @"GPS_data.db";//数据库连接字符
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        
        private void 工具栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;

            if ((sender as ToolStripMenuItem).Checked == false)//检查工具栏开关状态
            {
                toolStrip1.Visible = false;
            }
            else
            {
                toolStrip1.Visible = true;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (tool1 == null)
            {
                tool1 = new 实时用户定位();
                tool1.Show();
            }
            else if (tool1.IsDisposed)
            {
                tool1 = new 实时用户定位();
                tool1.Show();
            }
            else
            {
                if (tool1.WindowState == FormWindowState.Minimized)
                {
                    tool1.WindowState = FormWindowState.Normal;
                }
                tool1.Activate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void 实时经纬度获取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tool1 == null)
            {
                tool1 = new 实时用户定位();
                tool1.Show();
            }
            else if (tool1.IsDisposed)
            {
                tool1 = new 实时用户定位();
                tool1.Show();
            }
            else
            {
                if (tool1.WindowState == FormWindowState.Minimized)
                {
                    tool1.WindowState = FormWindowState.Normal;
                }
                tool1.Activate();
            }

            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (tool2 == null)
            {
                tool2 = new 轨迹操作();
                tool2.accept += new EventHandler(tool2_accept);
                tool2.Show();
            }
            else if (tool2.IsDisposed)
            {
                tool2 = new 轨迹操作();
                tool2.accept += new EventHandler(tool2_accept);
                tool2.Show();
            }
            else
            {
                if (tool2.WindowState == FormWindowState.Minimized)
                {
                    tool2.WindowState = FormWindowState.Normal;
                }
                tool2.Activate();
            }
            
        }
        void tool2_accept(object sender, EventArgs e)
        {
            //事件的接收者通过一个简单的类型转换得到tool2的引用 
            轨迹操作 tool2 = (轨迹操作)sender;
            //接收到tool2的textBox1.Text 
            this.textBox1.Text = tool2.tool2Value;
        }

        private void 轨迹操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tool2 == null)
            {
                tool2 = new 轨迹操作();
                tool2.accept += new EventHandler(tool2_accept);
                tool2.Show();
            }
            else if (tool2.IsDisposed)
            {
                tool2 = new 轨迹操作();
                tool2.accept += new EventHandler(tool2_accept);
                tool2.Show();
            }
            else
            {
                if (tool2.WindowState == FormWindowState.Minimized)
                {
                    tool2.WindowState = FormWindowState.Normal;
                }
                tool2.Activate();
            }

        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tool3 == null)
            {
                tool3 = new 关于();
                tool3.Show();
            }
            else if (tool3.IsDisposed)
            {
                tool3 = new 关于();
                tool3.Show();
            }
            else
            {
                if (tool3.WindowState == FormWindowState.Minimized)
                {
                    tool3.WindowState = FormWindowState.Normal;
                }
                tool3.Activate();
            }

            
        }

        private void 地图文件加载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tool4 == null)
            {
                tool4 = new 地图文件加载();
                tool4.Show();
            }
            else if (tool4.IsDisposed)
            {
                tool4 = new 地图文件加载();
                tool4.Show();
            }
            else
            {
                if (tool4.WindowState == FormWindowState.Minimized)
                {
                    tool4.WindowState = FormWindowState.Normal;
                }
                tool4.Activate();
            }
        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Map_show();
        }




        private void Map_show()//地图连接加载
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            string path = Application.StartupPath + "\\地图2.html";
            webBrowser1.Navigate(path);
            webBrowser1.ObjectForScripting = this;
        }
        private void Point_draw(string lng,string lat)//读取经纬度到html
        {
            webBrowser1.Document.InvokeScript("setLocation", new object[] { lng, lat });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text!=null)
            {
                
                SQLiteConnection conn = new SQLiteConnection(str);
                conn.Open();
                string sql = "select lng,lat from Point where no='"+textBox1.Text+"'";
                var sc = new SQLiteDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                sc.Fill(dt);
                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string lng = dt.Rows[i]["lng"].ToString();
                    string lat = dt.Rows[i]["lat"].ToString();
                    double[] tmpNE = Gps84ToGcj02(Convert.ToDouble(lat), Convert.ToDouble(lng));
                    double NNN = tmpNE[0];
                    double EEE = tmpNE[1];
                    lat = Convert.ToString(NNN);
                    lng = Convert.ToString(EEE);
                    webBrowser1.Document.InvokeScript("setLocation", new object[] { lng, lat });//操作动态函数绘制轨迹
                }

                conn.Close();
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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }

        
    }
}
