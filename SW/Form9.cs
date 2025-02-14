using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form9 : Form
    {
        database db = new database("Server=localhost;Database=project;Uid=root;Pwd=1234;");
        private string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";
        public Form9()
        {
            InitializeComponent();
            LoadlogData();
        }
        private void LoadlogData()
        {
            DataTable data = db.LoadlogData(); // LoadData 호출
            dataGridView1.DataSource = data; // LoadData 호출;
            dataGridView1.Columns[0].Visible = false; // 첫 번째 컬럼 숨기기

            // 기본 스타일 설정
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;     // 글씨 하얀색

            // 배경색
            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);

            // 셀 선택 시 배경 및 글자 색
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // 컬럼 헤더 배경색 변경
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(55, 55, 55);

            // 컬럼 헤더 텍스트 색상 변경
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // 행 헤더 배경색 변경
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(55, 55, 55);

            // 행 헤더 텍스트 색상 변경
            dataGridView1.RowHeadersDefaultCellStyle.ForeColor = Color.White;

            dataGridView1.RowTemplate.Height = 32; // 행 높이
            dataGridView1.Columns[0].Width = 150;  // 첫 번째 열의 너비

            // 셀 텍스트 폰트 크기 설정
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12); // Arial, 12포인트 폰트
            // 컬럼 헤더 텍스트 크기
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 13);

        }
    }
}

