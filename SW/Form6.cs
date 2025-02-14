using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing.Drawing2D;

namespace Project
{
    
    public partial class Form6 : Form
    {
      
        public int member_id { get; set; } // member_id 속성 선언
        private string currentDate;
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 폼 종료
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Color transparentColor = Color.FromArgb(128, Color.Black);
            using (SolidBrush brush = new SolidBrush(transparentColor))
            {
                e.Graphics.FillRectangle(brush, panel1.ClientRectangle);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string userId = textBox1.Text.Trim(); // ID 입력값
            string password = textBox3.Text.Trim(); // 비밀번호 입력값

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("아이디와 비밀번호를 모두 입력하세요.");
                return;
            }

            // 데이터베이스 연결 및 사용자 확인
            database db = new database("Server=localhost;Database=project;Uid=root;Pwd=1234;");
            var (isValidUser, memberType) = db.ValidateUser(userId, password); // 튜플로 결과 받기

            if (isValidUser)
            {
                currentDate = DateTime.Now.ToString("yyyy-MM-dd  tt HH:mm:ss");
                db.Updatelog(userId, currentDate);
                // 멤버 타입에 따라 처리
                if (memberType == "관리자")
                {
                    member_id = 2; // 관리자
                }
                else if (memberType == "일반")
                {
                    member_id = 3; // 일반 사용자
                }

                // 로그인 성공 후 로직 추가 (예: 다음 화면으로 이동)

                Form1 form1 = new Form1();
                form1.Show();
                form1.CheckCondition(member_id);
                form1.Currenttime(currentDate);

            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 잘못되었습니다.");
            }
        }
    }
}
