using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;

namespace Project
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();

            chart1.Series["Series1"].Points.AddXY("", "80");
            chart1.Series["Series1"].Points.AddXY("", "20");

            chart2.Series["Series2"].Points.AddXY("", "20");
            chart2.Series["Series2"].Points.AddXY("", "80");

            chart3.Series["Series3"].Points.AddXY("", "40");
            chart3.Series["Series3"].Points.AddXY("", "30"); 
            chart3.Series["Series3"].Points.AddXY("", "30");

            chart1.Legends.Clear(); // 모든 범례를 제거
            chart2.Legends.Clear(); // 모든 범례를 제거

            // 도넛 그래프의 반지름 설정 (60은 안쪽 반지름 비율로, 100에서의 60%를 의미)
            chart1.Series["Series1"]["DoughnutRadius"] = "20";  // 60% 두께
            chart2.Series["Series2"]["DoughnutRadius"] = "20";  // 60% 두께
            chart3.Series["Series3"]["DoughnutRadius"] = "20";  // 60% 두께

            // 도넛의 각 섹션 색상 변경
            chart1.Series["Series1"].Points[0].Color = Color.FromArgb(180, 120, 150, 0);  // 퍼센트 // 투명 노란색
            chart1.Series["Series1"].Points[1].Color = Color.FromArgb(90, 90, 90);

            chart2.Series["Series2"].Points[0].Color = Color.FromArgb(55, 55, 55);  // 퍼센트
            chart2.Series["Series2"].Points[1].Color = Color.FromArgb(90, 90, 90);

            chart3.Series["Series3"].Points[0].Color = Color.FromArgb(207, 229, 134); // 왼쪽 색상
            chart3.Series["Series3"].Points[1].Color = Color.FromArgb(180, 195, 235); // 중앙 색상
            chart3.Series["Series3"].Points[2].Color = Color.FromArgb(135, 139, 135); // 오른쪽 색상

            // 레이블 색상 변경 (흰색으로 설정)
            chart1.Series["Series1"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정
            chart2.Series["Series2"].LabelForeColor = Color.White;  // 레이블 텍스트 색상 흰색으로 설정

            // 도넛 그래프의 Paint 이벤트에 텍스트 그리기 처리 추가
            chart1.Paint += chart1_Paint;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Color transparentColor = Color.FromArgb(128, Color.Black);
            //using (SolidBrush brush = new SolidBrush(transparentColor))
            //{
            //    e.Graphics.FillRectangle(brush, panel1.ClientRectangle);
            //}
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            timer1.Interval = 100; //타이머 간격 100ms
            timer1.Start();  //타이머 시작

            chart3.Paint += chart3_Paint;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label1.Text = DateTime.Now.ToString("HH:mm"); // label1에 현재날짜시간 표시, F:자세한 전체 날짜/시간
            //label2.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void cButton1_Click(object sender, EventArgs e)
        {
            // Form1을 생성하고 보여줍니다.
            //Form1 form1 = new Form1();
            //form1.Show();  // Form1을 표시

            //// 현재 Form5를 닫습니다.
            //this.Hide();   // Form5를 숨깁니다.
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel2.ClientRectangle);
            }
        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            // 차트의 첫 번째 데이터 포인트 값 (60% 값을 가져오기)
            var value = chart1.Series["Series1"].Points[0].YValues[0]; // 첫 번째 포인트의 값
            string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

            Font font = new Font("Arial", 16, FontStyle.Bold);
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

            Font font = new Font("Arial", 16, FontStyle.Bold);
            SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // 차트의 중앙 위치 계산
            float x = (chart1.Width - textSize.Width) / 2;
            float y = (chart1.Height - textSize.Height) / 2;

            // 텍스트 그리기 (검정색 텍스트)
            e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel3.ClientRectangle);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel4.ClientRectangle);
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel5.ClientRectangle);
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel6.ClientRectangle);
            }
        }

        private void chart3_Paint(object sender, PaintEventArgs e)
        {
            // 차트의 첫 번째 데이터 포인트 값(60 % 값을 가져오기)
            var value = chart3.Series["Series3"].Points[0].YValues[0]; // 첫 번째 포인트의 값
            //string centerText = $"{value}%"; // "60%" 형태로 텍스트 설정

            //Font font = new Font("Arial", 16, FontStyle.Bold);
            //SizeF textSize = e.Graphics.MeasureString(centerText, font);

            // 차트의 중앙 위치 계산
            //float x = (chart1.Width - textSize.Width) / 2;
            //float y = (chart1.Height - textSize.Height) / 2;

            // 텍스트 그리기 (검정색 텍스트)
            //e.Graphics.DrawString(centerText, font, Brushes.White, x, y);
        }
    }
}
