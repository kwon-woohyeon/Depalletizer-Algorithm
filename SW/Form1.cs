using System;
using System.Windows.Forms;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using OpenCvSharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Project
{
    public partial class Form1 : Form
    {
        private database db;
        private System.Windows.Forms.Timer timer;
        private TcpClient client;
        public NetworkStream stream;


        private int lastNgCount = -1;
        private int lastOkCount = -1;
        private int lastNgCount1 = -1;
        private int lastOkCount1 = -1;
        private int lastOk = -1;  // ������ OK ��
        private int lastNg = -1;
        private bool _isRunning = false;
        private int frameCenterX; // ȭ�� �߾� ��ǥ
        private int dataValue = 0; // DB�� data ��
        private RecentList ListData;
        private int statecount = 0;
        private int id1 = 0;
        public string state_flag;
        public int state_total;
        public string message = ""; 
        private VideoProcessor videoProcessor1;  // VideoProcessor �ν��Ͻ��� �ʵ�� ����
        private string currenttime;
        private int total_ok = 0;
        private int total_ng = 0;
        public Form1()
        {
            InitializeDateTimeLabel();
            InitializeComponent();

            panel6.Visible = false;
            // Timer ����: 1�� �������� DB�� üũ�ϰ� ��Ʈ�� ������Ʈ�մϴ�.
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1�ʸ��� ������ ������Ʈ
            timer.Tick += UpdateTimer_Tick;
            timer.Start();

            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            db = new database(connectionString);
            videoProcessor1 = new VideoProcessor(this); // VideoProcessor ��ü ����

            ListData = new RecentList();
            if (db == null)
            {
                MessageBox.Show("DB ��ü�� null�Դϴ�.");
            }
            if (videoProcessor1 == null)
            {
                MessageBox.Show("VideoProcessor ��ü�� null�Դϴ�.");
            }
            if (ListData == null)
            {
                MessageBox.Show("ListData ��ü�� null�Դϴ�.");
            }
        }

        // �����͸� �񵿱������� ����
        private async Task ReceiveDataAsync()
        {
            byte[] buffer = new byte[1024];
            while (client != null && client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        // �޽��� ó��
                        if (receivedData == "capture")
                        {
                            videoProcessor1.capture_sign = "capture";
                        }
                        else if (receivedData == "complete")
                        {
                            videoProcessor1.complete_1 = "complete";
                            //string x = db.Get_Total_Today_NOW().ToString();

                            //byte[] dataToSend = Encoding.UTF8.GetBytes(x);
                            //await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // ������ ����
                            //MessageBox.Show(x);
                            //receivedData = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // ���� �߻� �� �α׸� ����� ���� ����
                    MessageBox.Show($"Error: {ex.Message}");
                    break;
                }
            }

            // ���� ���� �� �ڿ� ����
            client?.Close();
            stream?.Close();
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            int id = 1;  // ���÷� id ���� ����, �����δ� �������� ����
            int id1 = db.GetidToday(id);
            UpdateChart(id);
            DisplayCompanyData(id1);

        }

        private void SomeMethodWhereIdIsAvailable(int id)
        {
            int id1 = db.GetidToday(id);
            // Ư�� id�� ���� �ҷ����� ����ϰ� ��Ʈ�� ����
            UpdateChart(id);
            DisplayCompanyData(id1);
        }


        private void UpdateChart(int id)
        {
            int ngCount = db.GetNGsum();
            int okCount = db.GetOKForId();

            // DB���� ���� �ֱ������� �������� ���� ���� ���Ͽ� ����Ǿ��� ���� ��Ʈ�� ����
            if (ngCount != lastNgCount || okCount != lastOkCount)
            {
                // ���� ����Ǿ��� ���� ������Ʈ
                lastNgCount = ngCount;
                lastOkCount = okCount;
                // ��ü ���� (���÷� �ҷ� ���� + ���� ������ ����)
                int totalCount = ngCount + okCount;

                double normalRate = Math.Round((double)okCount / totalCount * 100, 2);  // �����
                double defectRate = 100 - normalRate;  // �ҷ���
                defectRate = Math.Round(defectRate, 2);

                // ����� ��Ʈ ������Ʈ (100���� ������ �� ����Ͽ� �ֱ�)
                double normalOther = 100 - normalRate; // 100���� ����� ����
                chart1.Series["Series1"].Points.Clear(); // ���� ������ ����
                chart1.Series["Series1"].Points.AddXY("", normalRate); // �����
                chart1.Series["Series1"].Points.AddXY("", normalOther); // ��Ÿ

                // �ҷ��� ��Ʈ ������Ʈ (100���� ������ �� ����Ͽ� �ֱ�)
                double defectOther = 100 - defectRate; // 100���� �ҷ��� ����
                chart2.Series["Series2"].Points.Clear(); // ���� ������ ����
                chart2.Series["Series2"].Points.AddXY("", defectRate); // �ҷ���
                chart2.Series["Series2"].Points.AddXY("", defectOther); // ��Ÿ

                chart1.Legends.Clear(); // ��� ���ʸ� ����
                chart2.Legends.Clear(); // ��� ���ʸ� ����

                // ���� �׷����� ������ ���� (60�� ���� ������ ������, 100������ 60%�� �ǹ�)
                chart1.Series["Series1"]["DoughnutRadius"] = "20";  // 60% �β�
                chart2.Series["Series2"]["DoughnutRadius"] = "20";  // 60% �β�

                // ������ �� ���� ���� ����
                chart1.Series["Series1"].Points[0].Color = Color.FromArgb(180, 120, 150, 0);  // �ۼ�Ʈ // ���� �����
                chart1.Series["Series1"].Points[1].Color = Color.FromArgb(90, 90, 90);

                chart2.Series["Series2"].Points[0].Color = Color.FromArgb(180, 200, 50, 50);  // �ۼ�Ʈ
                chart2.Series["Series2"].Points[1].Color = Color.FromArgb(90, 90, 90);

                // ���̺� ���� ���� (������� ����)
                chart1.Series["Series1"].LabelForeColor = Color.White;  // ���̺� �ؽ�Ʈ ���� ������� ����
                chart2.Series["Series2"].LabelForeColor = Color.White;  // ���̺� �ؽ�Ʈ ���� ������� ����

                // ���� �׷����� Paint �̺�Ʈ�� �ؽ�Ʈ �׸��� ó�� �߰�
                chart1.Paint += chart1_Paint;
                //}
            }
        }
        private async void startbtn_Click(object sender, EventArgs e)
        {

            if (!videoProcessor1._isRunning) // �̹� ���� ������ ������
            {
                videoProcessor1._isRunning = true; // ���� ó�� ���¸� '���� ��'���� ����
                await videoProcessor1.ProcessVideoAsync(); // �񵿱� ������� ���� ó�� ����
            }
            byte[] dataToSend = Encoding.UTF8.GetBytes("start");
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // ������ ����
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            DataTable data = db.Loadcdt();

            int id = 1;  // ���ϴ� id�� ���
            UpdateLabel9();     // total
            SomeMethodWhereIdIsAvailable(id);

       

            // MySQL���� ȸ�� �̸��� ��¥ ���� ��������
            string query = "SELECT name, Date FROM test";
            MySqlCommand command = new MySqlCommand(query, db.Connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            string baseDirectory = @"C:\Users\user\Desktop\Project\picture";

            foreach (DataRow row in dataTable.Rows)
            {
                string companyName = row["name"].ToString();
                string date = row["Date"].ToString();

                string dateDirectory = Path.Combine(baseDirectory, date);
                string companyDirectory = Path.Combine(dateDirectory, companyName);

                if (!Directory.Exists(companyDirectory))
                {
                    Directory.CreateDirectory(companyDirectory);
                }
                else
                {
                    // �̹� ���͸��� �����ϸ� �Ѿ
                    continue;
                }
            }

            // �� �ε� �� ESP-01�� ����
            try
            {
                string serverIp = "192.168.0.40"; // �ؽ�Ʈ �ڽ����� IP �Է�
                int port = 8080; // �ؽ�Ʈ �ڽ����� ��Ʈ �Է�

                client = new TcpClient();
                await client.ConnectAsync(serverIp, port); // TCP ����

                stream = client.GetStream(); // ��Ʈ�� ����

                // �񵿱� ������ ���� ����
                ReceiveDataAsync(); // ������ ���� �½�ũ�� ����
                MessageBox.Show("���� ����");

            }
            catch (Exception ex)
            {
                // ���� �޽����� ���� Ʈ���̽��� �޽��� �ڽ��� ���
                MessageBox.Show($"���� ����: {ex.Message}\n{ex.ToString()}");
            }


        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // �󺧿� ���� ��¥�� �ð� ������Ʈ
            YMDlabel.Text = DateTime.Now.ToString("yyyy-MM-dd");
            weeklabel.Text = DateTime.Now.ToString("dddd", new CultureInfo("en-US"));
            timelabel.Text = DateTime.Now.ToString("HH:mm");

        }
        private void InitializeDateTimeLabel()
        {
            // Timer �ʱ�ȭ
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1�� ����
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void todaylistbtn_Click(object sender, EventArgs e)
        {
            //searchbtn.Parent.Focus();  // ��ư�� Ŭ���� �Ŀ��� �׵θ��� ���� �ʴ� �ڵ�

            Form4 form4 = new Form4();
            form4.Show();

        }

        private void searchbtn_Click(object sender, EventArgs e)
        {
            //searchbtn.Parent.Focus();  // ��ư�� Ŭ���� �Ŀ��� �׵θ��� ���� �ʴ� �ڵ�

            Form3 form3 = new Form3();
            form3.Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel2.ClientRectangle);
            }
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, flowLayoutPanel2.ClientRectangle);
            }
        }

        private void statusbtn_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            database db = new database("Server=localhost;Database=project;Uid=root;Pwd=1234;");
            string endDate = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
            db.Updateendtimeforid(currenttime, endDate);
            // �� ����
            Application.Exit();


        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            // ��Ʈ�� ù ��° ������ ����Ʈ �� (60% ���� ��������)
            var value = chart1.Series["Series1"].Points[0].YValues[0]; // ù ��° ����Ʈ�� ��
            string centerText = $"{value}%"; // "60%" ���·� �ؽ�Ʈ ����

            Font font = new Font("Arial", 20, FontStyle.Bold);

            SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // ��Ʈ�� �߾� ��ġ ���
            float x = (chart1.Width - textSize.Width) / 2;
            float y = (chart1.Height - textSize.Height) / 2;

            // �ؽ�Ʈ �׸��� (������ �ؽ�Ʈ)
            e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }
        private void chart2_Paint(object sender, PaintEventArgs e)
        {
            // ��Ʈ�� ù ��° ������ ����Ʈ ��(60 % ���� ��������)
            var value = chart2.Series["Series2"].Points[0].YValues[0]; // ù ��° ����Ʈ�� ��
            string centerText = $"{value}%"; // "60%" ���·� �ؽ�Ʈ ����

            Font font = new Font("Arial", 20, FontStyle.Bold);
            SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // ��Ʈ�� �߾� ��ġ ���
            float x = (chart1.Width - textSize.Width) / 2;
            float y = (chart1.Height - textSize.Height) / 2;

            // �ؽ�Ʈ �׸��� (������ �ؽ�Ʈ)
            e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }

        public void UpdateLiveImage(Mat bitwiseFrame)
        {
            Invoke((Action)(() =>
            {
                Live.Image?.Dispose(); // Dispose�� ����ؼ� �����ؾ� �޸� ���� ����
                Live.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(bitwiseFrame); // ó���� �������� PictureBox�� ǥ��
            }));
        }

        public void UpdatePictureBox2(Mat frame)
        {
            Invoke((Action)(() =>
            {
                pictureBox2.Image?.Dispose(); // ���� �̹����� �����Ͽ� �޸� ���� ����
                pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame); // ���ο� �������� PictureBox�� ǥ��
            }));
        }


        public void SaveCapturedImage(Mat bitwiseFrame, int statecount, int index)
        {

            int currentid = videoProcessor1.realid;
            string companyname = db.GetnameForId(currentid);
            string time = DateTime.Now.ToString("HH:mm:ss");
            string state = "";
            if (statecount == 1)
            {
                state = "NG";
            }
            else if (statecount == 2)
            {
                state = "OK";
            }


            string baseDirectory = @"C:\Users\user\Desktop\Project\picture";
            string dateDirectory = Path.Combine(baseDirectory, DateTime.Now.ToString("yyyyMMdd"));
            string companyDirectory = Path.Combine(dateDirectory, companyname);
            ListData.ListData(companyname, time, state, flowLayoutPanel2);



            // ���͸� ����
            if (!Directory.Exists(companyDirectory))
            {
                Directory.CreateDirectory(companyDirectory);
            }
            if (statecount == 1)
            // ���� ��� ����
            {
                var savePath = Path.Combine(companyDirectory, $"{DateTime.Now:yyyyMMdd_HH_mm_ss}_{index}_NG_.jpg");
                Cv2.ImWrite(savePath, bitwiseFrame); // ���� ����
            }
            else
            {
                var savePath = Path.Combine(companyDirectory, $"{DateTime.Now:yyyyMMdd_HH_mm_ss}_{index}_OK_.jpg");
                Cv2.ImWrite(savePath, bitwiseFrame); // ���� ����
            }

        }

        private async void stopbtn_Click(object sender, EventArgs e)
        {
            videoProcessor1._isRunning = false; // ���� ó�� ���¸� '���� ��'���� ����
            Live.Image = null;
            
            byte[] dataToSend = Encoding.UTF8.GetBytes("stop");
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // ������ ����

        }
        public void Currenttime(string current)
        {
            currenttime = current;
        }
        public void CheckCondition(int member_id)
        {

            if (member_id == 2)
            {
                button2.Visible = true; // ��ư ���̱�
                button3.Visible = true;
            }
            else if (member_id == 3)
            {
                button2.Visible = false; // ��ư �����
                button3.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void companyglbtn_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
        }

        private void chart3_Paint(object sender, PaintEventArgs e)
        {
            if (chart3.Series["Series3"].Points.Count > 0)
            {
                var value = chart3.Series["Series3"].Points[0].YValues[0]; // ù ��° ����Ʈ�� ��
                string centerText = $"{value}%"; // "60%" ���·� �ؽ�Ʈ ����

                Font font = new Font("Arial", 20, FontStyle.Bold);
                SizeF textSize = e.Graphics.MeasureString(centerText, font);

                // ��Ʈ�� �߾� ��ġ ���
                float x = (chart3.Width - textSize.Width) / 2;
                float y = (chart3.Height - textSize.Height) / 2;

                // �ؽ�Ʈ �׸��� (������ �ؽ�Ʈ)
                e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
            }
        }

        private void chart4_Paint(object sender, PaintEventArgs e)
        {
            // Series4�� �����Ͱ� �ִ��� Ȯ��
            if (chart4.Series["Series4"].Points.Count > 0)
            {
                var value = chart4.Series["Series4"].Points[0].YValues[0]; // ù ��° ����Ʈ�� ��
                string centerText = $"{value}%"; // "60%" ���·� �ؽ�Ʈ ����

                Font font = new Font("Arial", 20, FontStyle.Bold);
                SizeF textSize = e.Graphics.MeasureString(centerText, font);

                // ��Ʈ�� �߾� ��ġ ���
                float x = (chart4.Width - textSize.Width) / 2;
                float y = (chart4.Height - textSize.Height) / 2;

                // �ؽ�Ʈ �׸��� (������ �ؽ�Ʈ)
                e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
            }
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void fullghbtn_Click(object sender, EventArgs e)
        {
            panel6.Visible = false;
        }

        private void UpdateLabel9()
        {
            int total = db.GetTotalsum();

            // UI �����忡�� label11�� ������Ʈ
            this.Invoke(new Action(() =>
            {
                label9.Text = total.ToString();
            }));
        }

        private async void DisplayCompanyData(int id1)
        {
            try
            {
                string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
                db = new database(connectionString);

                // db�� null�� �ƴ��� Ȯ���ϰ� GetCompanyData() ȣ��
                if (db != null)
                {
                    var companyData = db.GetCompanyAllData(id1); // GetCompanyData ȣ��

                    // ���� null�� �ƴ���, ���������� �����͸� �޾ƿԴ��� Ȯ��
                    if (companyData.name != null && companyData.name != null)
                    {
                        // �����Ͱ� ����Ǿ����� Ȯ��
                        bool okChanged = companyData.ok != lastOk;
                        bool ngChanged = companyData.ng != lastNg;

                        // ok�� ng ���� ����Ǿ��� ���� ���� ó��
                        if (okChanged || ngChanged)
                        {
                            // ���� �� ����
                            if (okChanged)
                            {
                                lastOk = companyData.ok;
                                
                                this.Invoke(new Action(() =>
                                {
                                    label10.Text = total_ok.ToString(); // label10�� total_ok ǥ��
                                }));
                                total_ok++;  // ok ���� ����Ǹ� total_ok ����
                            }

                            if (ngChanged)
                            {
                                lastNg = companyData.ng;
                                
                                this.Invoke(new Action(() =>
                                {
                                    label11.Text = total_ng.ToString(); // label11�� total_ng ǥ��
                                }));
                                total_ng++;  // ng ���� ����Ǹ� total_ng ����
                            }

                            // �� �� ����
                            label20.Text = companyData.name;  // ȸ�� �̸�
                            label16.Text = companyData.total.ToString();  // total ��
                            label14.Text = companyData.ok.ToString();  // ok ��
                            label12.Text = companyData.ng.ToString();  // ng ��

                            // ������� �ҷ��� ��� (�ۼ�Ʈ)
                            double totalValue = companyData.ok + companyData.ng;
                            double okPercentage = (totalValue > 0) ? (double)companyData.ok / totalValue * 100 : 0;
                            double ngPercentage = (totalValue > 0) ? (double)companyData.ng / totalValue * 100 : 0;
                            okPercentage = Math.Round(okPercentage, 2);  // �Ҽ��� ��°�ڸ����� �ݿø�
                            ngPercentage = Math.Round(ngPercentage, 2);  // �Ҽ��� ��°�ڸ����� �ݿø�

                            double normalOther = 100 - okPercentage; // 100���� ����� ����
                                                                     // Chart3 (����� ��Ʈ) ���� - ������� ǥ��
                            chart3.Series["Series3"].Points.Clear(); // ���� ������ �����
                            chart3.Series["Series3"].Points.AddXY("", okPercentage); // �����
                            chart3.Series["Series3"].Points.AddXY("", normalOther); // ��Ÿ

                            double defectOther = 100 - ngPercentage; // 100���� �ҷ��� ����
                                                                     // Chart4 (�ҷ��� ��Ʈ) ���� - �ҷ����� ǥ��
                            chart4.Series["Series4"].Points.Clear(); // ���� ������ �����
                            chart4.Series["Series4"].Points.AddXY("", ngPercentage); // �ҷ���
                            chart4.Series["Series4"].Points.AddXY("", defectOther); // ��Ÿ

                            chart3.Legends.Clear(); // ��� ���ʸ� ����
                            chart4.Legends.Clear(); // ��� ���ʸ� ����

                            // ���� �׷����� ������ ���� (60�� ���� ������ ������, 100������ 60%�� �ǹ�)
                            chart3.Series["Series3"]["DoughnutRadius"] = "20";  // 60% �β�
                            chart4.Series["Series4"]["DoughnutRadius"] = "20";  // 60% �β�

                            // ������ �� ���� ���� ����
                            chart3.Series["Series3"].Points[0].Color = Color.FromArgb(180, 120, 150, 0);  // �ۼ�Ʈ // ���� �����
                            chart3.Series["Series3"].Points[1].Color = Color.FromArgb(90, 90, 90);

                            chart4.Series["Series4"].Points[0].Color = Color.FromArgb(180, 200, 50, 50);  // �ۼ�Ʈ
                            chart4.Series["Series4"].Points[1].Color = Color.FromArgb(90, 90, 90);

                            // ���̺� ���� ���� (������� ����)
                            chart3.Series["Series3"].LabelForeColor = Color.White;  // ���̺� �ؽ�Ʈ ���� ������� ����
                            chart4.Series["Series4"].LabelForeColor = Color.White;  // ���̺� �ؽ�Ʈ ���� ������� ����
                        }

                        // total�� ok ���� ������, ���� id�� �Ѿ���� ó��
                        if (companyData.total == companyData.ok)
                        {
                            id1++;  // id ���� (���� ȸ��� �Ѿ��)
                            DisplayCompanyData(id1);  // ��� ȣ��� ���� ȸ�� ������ ǥ��
                        }
                    }
                    else
                    {
                        MessageBox.Show($"id {id1}�� ���� �����͸� ã�� �� �����ϴ�.");
                    }
                }
                else
                {
                    MessageBox.Show("Database ��ü�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
                }
            }
            catch (Exception ex)
            {
                // ���� ó��
                MessageBox.Show($"���� �߻�: {ex.Message}");
            }
        }



        private void fullghbtn2_Click(object sender, EventArgs e)
        {
            panel6.Visible = false;
        }

        private void companyglbtn2_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {
            // Graphics ��ü ��������
            Graphics g = e.Graphics;

            // DPI�� ����� 3cm�� �ȼ��� ��ȯ (1cm = 37.795px)
            float cmToPixel = 37.795f;
            float lineLengthInPixels = 7 * cmToPixel;  // 3cm�� �ȼ��� ��ȯ

            // ���� �׸��� ���� �� ��ü ����
            Pen pen = new Pen(Color.LightGray, 2);  // ������, �β� 3�� ��

            // (370, 55) ��ġ�� 3cm ������ �� �׸���
            g.DrawLine(pen, 15, 45, 370 + lineLengthInPixels, 45);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Graphics ��ü ��������
            Graphics g = e.Graphics;

            // DPI�� ����� 3cm�� �ȼ��� ��ȯ (1cm = 37.795px)
            float cmToPixel = 37.795f;
            float lineLengthInPixels = 7 * cmToPixel;  // 3cm�� �ȼ��� ��ȯ

            // ���� �׸��� ���� �� ��ü ����
            Pen pen = new Pen(Color.LightGray, 2);  // ������, �β� 3�� ��

            // (370, 55) ��ġ�� 3cm ������ �� �׸���
            g.DrawLine(pen, 15, 45, 370 + lineLengthInPixels, 45);
        }
    }
}
