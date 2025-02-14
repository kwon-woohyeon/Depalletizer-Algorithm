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
        private int lastOk = -1;  // 마지막 OK 값
        private int lastNg = -1;
        private bool _isRunning = false;
        private int frameCenterX; // 화면 중앙 좌표
        private int dataValue = 0; // DB의 data 값
        private RecentList ListData;
        private int statecount = 0;
        private int id1 = 0;
        public string state_flag;
        public int state_total;
        public string message = ""; 
        private VideoProcessor videoProcessor1;  // VideoProcessor 인스턴스를 필드로 선언
        private string currenttime;
        private int total_ok = 0;
        private int total_ng = 0;
        public Form1()
        {
            InitializeDateTimeLabel();
            InitializeComponent();

            panel6.Visible = false;
            // Timer 설정: 1초 간격으로 DB를 체크하고 차트를 업데이트합니다.
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1초마다 데이터 업데이트
            timer.Tick += UpdateTimer_Tick;
            timer.Start();

            string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
            db = new database(connectionString);
            videoProcessor1 = new VideoProcessor(this); // VideoProcessor 객체 생성

            ListData = new RecentList();
            if (db == null)
            {
                MessageBox.Show("DB 객체가 null입니다.");
            }
            if (videoProcessor1 == null)
            {
                MessageBox.Show("VideoProcessor 객체가 null입니다.");
            }
            if (ListData == null)
            {
                MessageBox.Show("ListData 객체가 null입니다.");
            }
        }

        // 데이터를 비동기적으로 수신
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

                        // 메시지 처리
                        if (receivedData == "capture")
                        {
                            videoProcessor1.capture_sign = "capture";
                        }
                        else if (receivedData == "complete")
                        {
                            videoProcessor1.complete_1 = "complete";
                            //string x = db.Get_Total_Today_NOW().ToString();

                            //byte[] dataToSend = Encoding.UTF8.GetBytes(x);
                            //await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송
                            //MessageBox.Show(x);
                            //receivedData = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 예외 발생 시 로그를 남기고 루프 종료
                    MessageBox.Show($"Error: {ex.Message}");
                    break;
                }
            }

            // 연결 종료 시 자원 해제
            client?.Close();
            stream?.Close();
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            int id = 1;  // 예시로 id 값을 지정, 실제로는 동적으로 설정
            int id1 = db.GetidToday(id);
            UpdateChart(id);
            DisplayCompanyData(id1);

        }

        private void SomeMethodWhereIdIsAvailable(int id)
        {
            int id1 = db.GetidToday(id);
            // 특정 id에 대해 불량률을 계산하고 차트를 갱신
            UpdateChart(id);
            DisplayCompanyData(id1);
        }


        private void UpdateChart(int id)
        {
            int ngCount = db.GetNGsum();
            int okCount = db.GetOKForId();

            // DB에서 값을 주기적으로 가져오고 이전 값과 비교하여 변경되었을 때만 차트를 갱신
            if (ngCount != lastNgCount || okCount != lastOkCount)
            {
                // 값이 변경되었을 때만 업데이트
                lastNgCount = ngCount;
                lastOkCount = okCount;
                // 전체 개수 (예시로 불량 개수 + 정상 개수로 설정)
                int totalCount = ngCount + okCount;

                double normalRate = Math.Round((double)okCount / totalCount * 100, 2);  // 정상률
                double defectRate = 100 - normalRate;  // 불량률
                defectRate = Math.Round(defectRate, 2);

                // 정상률 차트 업데이트 (100에서 나머지 값 계산하여 넣기)
                double normalOther = 100 - normalRate; // 100에서 정상률 빼기
                chart1.Series["Series1"].Points.Clear(); // 기존 데이터 제거
                chart1.Series["Series1"].Points.AddXY("", normalRate); // 정상률
                chart1.Series["Series1"].Points.AddXY("", normalOther); // 기타

                // 불량률 차트 업데이트 (100에서 나머지 값 계산하여 넣기)
                double defectOther = 100 - defectRate; // 100에서 불량률 빼기
                chart2.Series["Series2"].Points.Clear(); // 기존 데이터 제거
                chart2.Series["Series2"].Points.AddXY("", defectRate); // 불량률
                chart2.Series["Series2"].Points.AddXY("", defectOther); // 기타

                chart1.Legends.Clear(); // 모든 범례를 제거
                chart2.Legends.Clear(); // 모든 범례를 제거

                // 도넛 그래프의 반지름 설정 (60은 안쪽 반지름 비율로, 100에서의 60%를 의미)
                chart1.Series["Series1"]["DoughnutRadius"] = "20";  // 60% 두께
                chart2.Series["Series2"]["DoughnutRadius"] = "20";  // 60% 두께

                // 도넛의 각 섹션 색상 변경
                chart1.Series["Series1"].Points[0].Color = Color.FromArgb(180, 120, 150, 0);  // 퍼센트 // 투명 노란색
                chart1.Series["Series1"].Points[1].Color = Color.FromArgb(90, 90, 90);

                chart2.Series["Series2"].Points[0].Color = Color.FromArgb(180, 200, 50, 50);  // 퍼센트
                chart2.Series["Series2"].Points[1].Color = Color.FromArgb(90, 90, 90);

                // 레이블 색상 변경 (흰색으로 설정)
                chart1.Series["Series1"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정
                chart2.Series["Series2"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정

                // 도넛 그래프의 Paint 이벤트에 텍스트 그리기 처리 추가
                chart1.Paint += chart1_Paint;
                //}
            }
        }
        private async void startbtn_Click(object sender, EventArgs e)
        {

            if (!videoProcessor1._isRunning) // 이미 실행 중이지 않으면
            {
                videoProcessor1._isRunning = true; // 비디오 처리 상태를 '실행 중'으로 설정
                await videoProcessor1.ProcessVideoAsync(); // 비동기 방식으로 영상 처리 시작
            }
            byte[] dataToSend = Encoding.UTF8.GetBytes("start");
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            DataTable data = db.Loadcdt();

            int id = 1;  // 원하는 id를 사용
            UpdateLabel9();     // total
            SomeMethodWhereIdIsAvailable(id);

       

            // MySQL에서 회사 이름과 날짜 정보 가져오기
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
                    // 이미 디렉터리가 존재하면 넘어감
                    continue;
                }
            }

            // 폼 로드 시 ESP-01에 연결
            try
            {
                string serverIp = "192.168.0.40"; // 텍스트 박스에서 IP 입력
                int port = 8080; // 텍스트 박스에서 포트 입력

                client = new TcpClient();
                await client.ConnectAsync(serverIp, port); // TCP 연결

                stream = client.GetStream(); // 스트림 생성

                // 비동기 데이터 수신 시작
                ReceiveDataAsync(); // 수신을 별도 태스크로 실행
                MessageBox.Show("연결 성공");

            }
            catch (Exception ex)
            {
                // 예외 메시지와 스택 트레이스를 메시지 박스로 출력
                MessageBox.Show($"연결 실패: {ex.Message}\n{ex.ToString()}");
            }


        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 라벨에 현재 날짜와 시간 업데이트
            YMDlabel.Text = DateTime.Now.ToString("yyyy-MM-dd");
            weeklabel.Text = DateTime.Now.ToString("dddd", new CultureInfo("en-US"));
            timelabel.Text = DateTime.Now.ToString("HH:mm");

        }
        private void InitializeDateTimeLabel()
        {
            // Timer 초기화
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1초 간격
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void todaylistbtn_Click(object sender, EventArgs e)
        {
            //searchbtn.Parent.Focus();  // 버튼을 클릭한 후에도 테두리가 뜨지 않는 코드

            Form4 form4 = new Form4();
            form4.Show();

        }

        private void searchbtn_Click(object sender, EventArgs e)
        {
            //searchbtn.Parent.Focus();  // 버튼을 클릭한 후에도 테두리가 뜨지 않는 코드

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
            // 폼 종료
            Application.Exit();


        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            // 차트의 첫 번째 데이터 포인트 값 (60% 값을 가져오기)
            var value = chart1.Series["Series1"].Points[0].YValues[0]; // 첫 번째 포인트의 값
            string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

            Font font = new Font("Arial", 20, FontStyle.Bold);

            SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // 차트의 중앙 위치 계산
            float x = (chart1.Width - textSize.Width) / 2;
            float y = (chart1.Height - textSize.Height) / 2;

            // 텍스트 그리기 (검정색 텍스트)
            e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }
        private void chart2_Paint(object sender, PaintEventArgs e)
        {
            // 차트의 첫 번째 데이터 포인트 값(60 % 값을 가져오기)
            var value = chart2.Series["Series2"].Points[0].YValues[0]; // 첫 번째 포인트의 값
            string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

            Font font = new Font("Arial", 20, FontStyle.Bold);
            SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // 차트의 중앙 위치 계산
            float x = (chart1.Width - textSize.Width) / 2;
            float y = (chart1.Height - textSize.Height) / 2;

            // 텍스트 그리기 (검정색 텍스트)
            e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }

        public void UpdateLiveImage(Mat bitwiseFrame)
        {
            Invoke((Action)(() =>
            {
                Live.Image?.Dispose(); // Dispose를 사용해서 해지해야 메모리 누수 방지
                Live.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(bitwiseFrame); // 처리된 프레임을 PictureBox에 표시
            }));
        }

        public void UpdatePictureBox2(Mat frame)
        {
            Invoke((Action)(() =>
            {
                pictureBox2.Image?.Dispose(); // 이전 이미지를 해제하여 메모리 누수 방지
                pictureBox2.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame); // 새로운 프레임을 PictureBox에 표시
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



            // 디렉터리 생성
            if (!Directory.Exists(companyDirectory))
            {
                Directory.CreateDirectory(companyDirectory);
            }
            if (statecount == 1)
            // 저장 경로 설정
            {
                var savePath = Path.Combine(companyDirectory, $"{DateTime.Now:yyyyMMdd_HH_mm_ss}_{index}_NG_.jpg");
                Cv2.ImWrite(savePath, bitwiseFrame); // 파일 저장
            }
            else
            {
                var savePath = Path.Combine(companyDirectory, $"{DateTime.Now:yyyyMMdd_HH_mm_ss}_{index}_OK_.jpg");
                Cv2.ImWrite(savePath, bitwiseFrame); // 파일 저장
            }

        }

        private async void stopbtn_Click(object sender, EventArgs e)
        {
            videoProcessor1._isRunning = false; // 비디오 처리 상태를 '실행 중'으로 설정
            Live.Image = null;
            
            byte[] dataToSend = Encoding.UTF8.GetBytes("stop");
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length); // 데이터 전송

        }
        public void Currenttime(string current)
        {
            currenttime = current;
        }
        public void CheckCondition(int member_id)
        {

            if (member_id == 2)
            {
                button2.Visible = true; // 버튼 보이기
                button3.Visible = true;
            }
            else if (member_id == 3)
            {
                button2.Visible = false; // 버튼 숨기기
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
                var value = chart3.Series["Series3"].Points[0].YValues[0]; // 첫 번째 포인트의 값
                string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

                Font font = new Font("Arial", 20, FontStyle.Bold);
                SizeF textSize = e.Graphics.MeasureString(centerText, font);

                // 차트의 중앙 위치 계산
                float x = (chart3.Width - textSize.Width) / 2;
                float y = (chart3.Height - textSize.Height) / 2;

                // 텍스트 그리기 (검정색 텍스트)
                e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
            }
        }

        private void chart4_Paint(object sender, PaintEventArgs e)
        {
            // Series4의 데이터가 있는지 확인
            if (chart4.Series["Series4"].Points.Count > 0)
            {
                var value = chart4.Series["Series4"].Points[0].YValues[0]; // 첫 번째 포인트의 값
                string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

                Font font = new Font("Arial", 20, FontStyle.Bold);
                SizeF textSize = e.Graphics.MeasureString(centerText, font);

                // 차트의 중앙 위치 계산
                float x = (chart4.Width - textSize.Width) / 2;
                float y = (chart4.Height - textSize.Height) / 2;

                // 텍스트 그리기 (검정색 텍스트)
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

            // UI 스레드에서 label11를 업데이트
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

                // db가 null이 아닌지 확인하고 GetCompanyData() 호출
                if (db != null)
                {
                    var companyData = db.GetCompanyAllData(id1); // GetCompanyData 호출

                    // 값이 null이 아닌지, 정상적으로 데이터를 받아왔는지 확인
                    if (companyData.name != null && companyData.name != null)
                    {
                        // 데이터가 변경되었는지 확인
                        bool okChanged = companyData.ok != lastOk;
                        bool ngChanged = companyData.ng != lastNg;

                        // ok나 ng 값이 변경되었을 때만 갱신 처리
                        if (okChanged || ngChanged)
                        {
                            // 이전 값 갱신
                            if (okChanged)
                            {
                                lastOk = companyData.ok;
                                
                                this.Invoke(new Action(() =>
                                {
                                    label10.Text = total_ok.ToString(); // label10에 total_ok 표시
                                }));
                                total_ok++;  // ok 값이 변경되면 total_ok 증가
                            }

                            if (ngChanged)
                            {
                                lastNg = companyData.ng;
                                
                                this.Invoke(new Action(() =>
                                {
                                    label11.Text = total_ng.ToString(); // label11에 total_ng 표시
                                }));
                                total_ng++;  // ng 값이 변경되면 total_ng 증가
                            }

                            // 라벨 값 갱신
                            label20.Text = companyData.name;  // 회사 이름
                            label16.Text = companyData.total.ToString();  // total 값
                            label14.Text = companyData.ok.ToString();  // ok 값
                            label12.Text = companyData.ng.ToString();  // ng 값

                            // 정상률과 불량률 계산 (퍼센트)
                            double totalValue = companyData.ok + companyData.ng;
                            double okPercentage = (totalValue > 0) ? (double)companyData.ok / totalValue * 100 : 0;
                            double ngPercentage = (totalValue > 0) ? (double)companyData.ng / totalValue * 100 : 0;
                            okPercentage = Math.Round(okPercentage, 2);  // 소수점 둘째자리까지 반올림
                            ngPercentage = Math.Round(ngPercentage, 2);  // 소수점 둘째자리까지 반올림

                            double normalOther = 100 - okPercentage; // 100에서 정상률 빼기
                                                                     // Chart3 (정상률 차트) 설정 - 정상률만 표시
                            chart3.Series["Series3"].Points.Clear(); // 기존 데이터 지우기
                            chart3.Series["Series3"].Points.AddXY("", okPercentage); // 정상률
                            chart3.Series["Series3"].Points.AddXY("", normalOther); // 기타

                            double defectOther = 100 - ngPercentage; // 100에서 불량률 빼기
                                                                     // Chart4 (불량률 차트) 설정 - 불량률만 표시
                            chart4.Series["Series4"].Points.Clear(); // 기존 데이터 지우기
                            chart4.Series["Series4"].Points.AddXY("", ngPercentage); // 불량률
                            chart4.Series["Series4"].Points.AddXY("", defectOther); // 기타

                            chart3.Legends.Clear(); // 모든 범례를 제거
                            chart4.Legends.Clear(); // 모든 범례를 제거

                            // 도넛 그래프의 반지름 설정 (60은 안쪽 반지름 비율로, 100에서의 60%를 의미)
                            chart3.Series["Series3"]["DoughnutRadius"] = "20";  // 60% 두께
                            chart4.Series["Series4"]["DoughnutRadius"] = "20";  // 60% 두께

                            // 도넛의 각 섹션 색상 변경
                            chart3.Series["Series3"].Points[0].Color = Color.FromArgb(180, 120, 150, 0);  // 퍼센트 // 투명 노란색
                            chart3.Series["Series3"].Points[1].Color = Color.FromArgb(90, 90, 90);

                            chart4.Series["Series4"].Points[0].Color = Color.FromArgb(180, 200, 50, 50);  // 퍼센트
                            chart4.Series["Series4"].Points[1].Color = Color.FromArgb(90, 90, 90);

                            // 레이블 색상 변경 (흰색으로 설정)
                            chart3.Series["Series3"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정
                            chart4.Series["Series4"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정
                        }

                        // total과 ok 값이 같으면, 다음 id로 넘어가도록 처리
                        if (companyData.total == companyData.ok)
                        {
                            id1++;  // id 증가 (다음 회사로 넘어가기)
                            DisplayCompanyData(id1);  // 재귀 호출로 다음 회사 데이터 표시
                        }
                    }
                    else
                    {
                        MessageBox.Show($"id {id1}에 대한 데이터를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    MessageBox.Show("Database 객체가 초기화되지 않았습니다.");
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                MessageBox.Show($"오류 발생: {ex.Message}");
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
            // Graphics 객체 가져오기
            Graphics g = e.Graphics;

            // DPI를 고려한 3cm를 픽셀로 변환 (1cm = 37.795px)
            float cmToPixel = 37.795f;
            float lineLengthInPixels = 7 * cmToPixel;  // 3cm를 픽셀로 변환

            // 선을 그리기 위한 펜 객체 생성
            Pen pen = new Pen(Color.LightGray, 2);  // 빨간색, 두께 3인 펜

            // (370, 55) 위치에 3cm 길이의 선 그리기
            g.DrawLine(pen, 15, 45, 370 + lineLengthInPixels, 45);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Graphics 객체 가져오기
            Graphics g = e.Graphics;

            // DPI를 고려한 3cm를 픽셀로 변환 (1cm = 37.795px)
            float cmToPixel = 37.795f;
            float lineLengthInPixels = 7 * cmToPixel;  // 3cm를 픽셀로 변환

            // 선을 그리기 위한 펜 객체 생성
            Pen pen = new Pen(Color.LightGray, 2);  // 빨간색, 두께 3인 펜

            // (370, 55) 위치에 3cm 길이의 선 그리기
            g.DrawLine(pen, 15, 45, 370 + lineLengthInPixels, 45);
        }
    }
}
