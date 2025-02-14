using MySql.Data.MySqlClient;
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
    public partial class Form7 : Form
    {
        database db = new database("Server=localhost;Database=project;Uid=root;Pwd=1234;");
        private string connectionString = "Server=localhost;Database=project;User Id=root;Password=1234;";

        public Form7()
        {
            InitializeComponent();
            LoadUserData();

        }
        private void LoadUserData()
        {
            DataTable data = db.LoadUserData(); // LoadData 호출
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
        private void button1_Click(object sender, EventArgs e)
        {
            string query = "UPDATE project.login SET id = @id, password = @password, member = @member WHERE i = @i";

            try
            {
                // 선택된 행(Row) 가져오기
                if (dataGridView1.SelectedRows.Count > 0) // 선택된 행이 있는지 확인
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // 수정된 데이터 가져오기
                    string i = selectedRow.Cells["i"].Value.ToString(); // 수정된 i 값
                    string password = selectedRow.Cells["password"].Value.ToString(); // 수정된 비밀번호
                    string id = selectedRow.Cells["id"].Value.ToString(); // 선택된 행의 ID
                    string member = selectedRow.Cells["member"].Value.ToString(); // 수정된 멤버 값

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // 쿼리 파라미터 설정
                            cmd.Parameters.AddWithValue("@i", i);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@member", member);

                            // SQL 실행
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("데이터가 성공적으로 수정되었습니다.");
                    LoadUserData(); // 데이터 새로고침
                }
                else
                {
                    MessageBox.Show("수정할 행을 선택하세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("수정 중 오류 발생: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM  project.login WHERE i = @i";

            try
            {
                // 선택된 행(Row) 가져오기
                if (dataGridView1.SelectedRows.Count > 0) // 선택된 행이 있는지 확인
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // 삭제할 데이터 가져오기
                    string i = selectedRow.Cells["i"].Value.ToString(); // 삭제 기준 값

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // 쿼리 파라미터 설정
                            cmd.Parameters.AddWithValue("@i", i);

                            // SQL 실행
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("데이터가 성공적으로 삭제되었습니다.");
                            }
                            else
                            {
                                MessageBox.Show("삭제할 데이터를 찾을 수 없습니다.");
                            }
                        }
                    }

                    LoadUserData(); // 데이터 새로고침
                }
                else
                {
                    MessageBox.Show("삭제할 행을 선택하세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("삭제 중 오류 발생: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string insertQuery = "INSERT INTO  project.login (password, id, member) VALUES ( @password, @id, @member)";

            try
            {
                // DataGridView의 마지막 행 가져오기
                DataGridViewRow lastRow = dataGridView1.Rows[dataGridView1.Rows.Count - 2];

                if (!lastRow.IsNewRow) // 마지막 행이 새 행인지 확인
                {
                    // 새 행 데이터 가져오기

                    string newPassword = lastRow.Cells["password"].Value?.ToString() ?? string.Empty;
                    string newId = lastRow.Cells["id"].Value?.ToString() ?? string.Empty;
                    string newMember = lastRow.Cells["member"].Value?.ToString() ?? string.Empty;

                    // 필수 데이터가 비어 있는 경우 처리
                    if (string.IsNullOrWhiteSpace(newMember) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(newId))
                    {
                        MessageBox.Show("모든 필드를 채워주세요.");
                        return;
                    }

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                        {
                            // 쿼리 파라미터 설정

                            insertCmd.Parameters.AddWithValue("@password", newPassword);
                            insertCmd.Parameters.AddWithValue("@id", newId);
                            insertCmd.Parameters.AddWithValue("@member", newMember);

                            // SQL 실행
                            int insertRowsAffected = insertCmd.ExecuteNonQuery();

                            if (insertRowsAffected > 0)
                            {
                                MessageBox.Show("새로운 데이터가 성공적으로 추가되었습니다.");
                            }
                            else
                            {
                                MessageBox.Show("데이터 추가에 실패했습니다.");
                            }
                        }
                    }

                    LoadUserData(); // 데이터 새로고침
                }
                else
                {
                    MessageBox.Show("추가할 데이터가 없습니다. 마지막 행에 데이터를 입력하세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("추가 중 오류 발생: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void Logbtn_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.Show();
        }
    }
}
