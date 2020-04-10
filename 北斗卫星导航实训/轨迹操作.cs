using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace 北斗卫星导航实训
{
    public partial class 轨迹操作 : Form
    {
        private string str = @"Data Source = " + AppDomain.CurrentDomain.BaseDirectory + @"GPS_Data.db";//数据库连接字符
        public event EventHandler accept;//传递数据的事件


        public 轨迹操作()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView_re();
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//选中整行
            if (this.dataGridView1.Rows.Count > 0)
            {
                textBox1.Text = this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
        }
        private void dataGridView_re()
        {
            SQLiteConnection conn = new SQLiteConnection(str);
            conn.Open();
            string sql = "select * from View";
            SQLiteDataAdapter sc = new SQLiteDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            sc.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }
        public string tool2Value
        {
            get
            {
                return this.textBox1.Text;
            }
            set
            {
                this.textBox1.Text = value;
            }
        }

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (accept != null)
            {
                accept(this, EventArgs.Empty); //当窗体触发事件，传递自身引用 
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void 轨迹记录_Enter(object sender, EventArgs e)
        {

        }
    }
}
