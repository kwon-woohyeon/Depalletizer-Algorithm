using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form4 : Form
    {
        private database db;
        private System.Windows.Forms.Timer timer;
        private DataDisplayer dataDisplayer;
        public Form4()
        {
            InitializeComponent();
            InitializeDateTimeLabel();
            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            db = new database(connectionString);
            dataDisplayer = new DataDisplayer(db);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 라벨에 현재 날짜와 시간 업데이트
            //label2.Text = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");

        }
        private void InitializeDateTimeLabel()
        {
            // Timer 초기화
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1초 간격
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                string formattedDate = DateTime.Now.ToString("yyyyMMdd");
                DataTable data = db.LoadDataFromDatabase(formattedDate);
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected date.");
                }
                else
                {
                    dataDisplayer.DisplayData(data, tdflowLayoutPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
