using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
namespace output
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 变量声明区




            public string str = "";//该变量保存INI文件所在的具体物理位置
            public string strOne = "";
            public string LineName = "";
            public static string TableName;      //表名
            public static string S1, Def, DefDate;
            public static string IP, DB, User, PWD;




        string S1sql = "";
        string SIndex = "";




        #endregion

        #region 读ini文件部分
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        public string ContentReader(string area, string key, string def)
        {
            StringBuilder stringBuilder = new StringBuilder(1024); 				//定义一个最大长度为1024的可变字符串
            GetPrivateProfileString(area, key, def, stringBuilder, 1024,str); 			//读取INI文件
            return stringBuilder.ToString();								//返回INI文件的内容
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string mpAppName,
            string mpKeyName,
            string mpDefault,
            string mpFileName);

        #endregion


 

        private void button1_Click(object sender, EventArgs e)
        {
            SIndex = comboBox1.SelectedIndex.ToString();
            Query();
        }
        #region 窗体加载部分
        private void Form1_Load(object sender, EventArgs e)
        {
            str = Application.StartupPath + "\\ConnectString.ini";						//INI文件的物理地址
            strOne = System.IO.Path.GetFileNameWithoutExtension(str); 				//获取INI文件的文件名
            if (File.Exists(str)) 											//判断是否存在该INI文件
            {
                LineName= ContentReader("Area", "LineName", "");
                TableName = ContentReader("Table", "T1", "");               //读取INI文件中数据库节点的内容
                S1 = ContentReader("STAT", "S1", "");
                Def = ContentReader("Default", "Def", "");
                DefDate = ContentReader("Default", "DefDate", "");
                IP = ContentReader("Address", "IP", "");
                DB = ContentReader("Address", "DB", "");
                User = ContentReader("Address", "User", "");
                PWD = ContentReader("Address", "PWD", "");
            }
            //this.StartPosition = FormStartPosition.CenterScreen;
            int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
            int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
            this.StartPosition = FormStartPosition.Manual;//窗体位置由Location属性决定
            this.Location = (Point)new Size(x,y);

            this.Text = LineName +"小时产量统计";

            dateTimePicker1.Value = DateTime.Now.AddDays(Convert.ToDouble(DefDate) );
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker1.Value = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 08, 00, 00);
            dateTimePicker2.Value = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, 23, 59, 59);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] str1 = new string[] { "小时产量", "车型小时产量", "颜色小时产量" };
            comboBox1.DataSource = str1;
            comboBox1.SelectedIndex = Convert.ToInt32(Def);
            SIndex = comboBox1.SelectedIndex.ToString();
            Query();
           
           

        }
        #endregion

        public void Query()
        {
           
            switch (SIndex)
            {
                case "0":
                    S1sql = "select to_char(INTIME,'yyyy-mm-dd HH24') as time,count(*) as count" +
                          " from " + TableName +
                          " WHERE STAT ='" + S1 +
                          "' AND INTIME >= to_date('" + dateTimePicker1.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                          " AND INTIME<= to_date('" + dateTimePicker2.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                          " GROUP BY to_char(INTIME, 'yyyy-mm-dd HH24')" +
                          " ORDER BY TIME ASC";
                    break;

                case "1":
                    S1sql = "select to_char(INTIME,'yyyy-mm-dd HH24') as time,BODY AS TYPE，count(*) as count" +
                                     " from " + TableName +
                                     " WHERE STAT ='" + S1 +
                                     "' AND INTIME >= to_date('" + dateTimePicker1.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                                     " AND INTIME<= to_date('" + dateTimePicker2.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                                     " GROUP BY to_char(INTIME, 'yyyy-mm-dd HH24'),BODY" +
                                     " ORDER BY TIME ASC";

                    break;

                case "2":
                    S1sql = "select to_char(INTIME,'yyyy-mm-dd HH24') as time,COLOR as Color,count(*) as count" +
                                     " from " + TableName +
                                     " WHERE STAT ='" + S1 +
                                     "' AND INTIME >= to_date('" + dateTimePicker1.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                                     " AND INTIME<= to_date('" + dateTimePicker2.Value + "','YYYY.MM.DD HH24:MI:SS')" +
                                     " GROUP BY to_char(INTIME, 'yyyy-mm-dd HH24'),COLOR" +
                                     " ORDER BY TIME ASC";

                    break;

            }

            #region  dataGridView
            
            DataSet S1ds = new DB().ReturnDataSet(S1sql, "Tab");
            dataGridView1.DataSource = null;
            
            dataGridView1.DataSource = S1ds.Tables["Tab"];//填充完的数据在datagridview中显示
            dataGridView1.DataSource = S1ds.Tables[0];//填充完的数据在datagridview中显示
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//选择时可以选择一整行
            dataGridView1.ReadOnly = true;//设置控件只能只读
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Green;//设置选中行显示绿色
            dataGridView1.Columns[0].Width = 100;

            #endregion

            // 产量和
            S1sql = "SELECT * FROM " + TableName +
               " WHERE STAT ='" + S1 +
               "' AND INTIME >= to_date('" + dateTimePicker1.Value + "','YYYY.MM.DD HH24:MI:SS')" +
               " AND INTIME<= to_date('" + dateTimePicker2.Value + "','YYYY.MM.DD HH24:MI:SS')";
              
            int SumCar1 = new DB().GetRecordCount(S1sql);
            label1.Text = LineName + " 产量合计：" + SumCar1.ToString() + " 台";


        }


    }
}
