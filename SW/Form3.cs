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
    public partial class Form3 : Form
    {
        private DataDisplayer dataDisplayer;
        private System.Windows.Forms.Timer timer;
        private database db;
        public Form3()
        {
            InitializeComponent();
            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            db = new database(connectionString);
            dataDisplayer = new DataDisplayer(db);
            InitializeDateTimeLabel();

        }
        private void InitializeDateTimeLabel()
        {
            // Timer 초기화
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1초 간격
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 라벨에 현재 날짜와 시간 업데이트

            //clocklb.Text = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (companytextbox.Text == "")
            {
                try
                {
                    string formattedDate = dateTimePicker1.Value.ToString("yyyyMMdd");
                    DataTable data = db.LoadDataFromDatabase(formattedDate);
                    if (data.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found for the selected date.");
                    }
                    else
                    {
                        dataDisplayer.DisplayData(data, flowLayoutPanel3);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    string formattedDate = dateTimePicker1.Value.ToString("yyyyMMdd");

                    // 회사명 입력값 가져오기
                    string companyName = companytextbox.Text.Trim(); // 회사명 입력 TextBox 사용

                    if (string.IsNullOrWhiteSpace(companyName))
                    {
                        MessageBox.Show("Please enter a company name.");
                        return;
                    }

                    // 데이터 로드
                    DataTable data = db.LoadDcompanyanddate(formattedDate, companyName);
                    if (data.Rows.Count == 0)
                    {
                        MessageBox.Show($"No data found for the date: {formattedDate} and company: {companyName}");
                    }
                    else
                    {
                        dataDisplayer.DisplayData(data, flowLayoutPanel3);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

    }
}
