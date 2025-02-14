using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Project
{
    public class DataDisplayer
    {
        private database db;
        

        // 생성자: database 클래스 객체를 주입받음
        public DataDisplayer(database db)
        {
            this.db = db; // 외부에서 전달된 database 객체 사용
        }

        // 데이터를 표시하는 메서드
        public void DisplayData(DataTable dataTable, FlowLayoutPanel targetPanel)
        {
            targetPanel.Controls.Clear(); // 기존 데이터 제거

            foreach (DataRow row in dataTable.Rows)
            {
                // 각 데이터를 Panel로 표시
                Panel dataPanel = new Panel
                {
                    Size = new Size(1000, 75),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(10)
                };

                // 데이터 라벨 추가
                Label lblData = new Label
                {
                    Text = $"\nCompany: {row["name"]}    Address: {row["address"]}    Total: {row["total"]}",
                    AutoSize = false,
                    Font = new Font("Arial", 13, FontStyle.Bold),
                    Size = new Size(500, 100),
                    Location = new Point(5, 5),
                    ForeColor = Color.White
                };
               

                // Button 추가 (상세 보기)
                Button btnViewDetails = new Button
                {
                    Text = "View photo",
                    Size = new Size(240, 40),
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Location = new Point(720, 25),
                    Tag = row["id"], // id 저장
                    ForeColor = Color.White
                };
                btnViewDetails.Click += BtnViewDetails_Click;

                // Panel에 컨트롤 추가
                dataPanel.Controls.Add(lblData);
                dataPanel.Controls.Add(btnViewDetails);

                // FlowLayoutPanel에 Panel 추가
                targetPanel.Controls.Add(dataPanel);
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {

            try
            {
                Button clickedButton = sender as Button;

                if (clickedButton == null || clickedButton.Tag == null)
                {
                    MessageBox.Show("유효하지 않은 데이터입니다.");
                    return;
                }
                string route = @"C:\Users\user\Desktop\Project\picture";
                int dataId = Convert.ToInt32(clickedButton.Tag);
                string date = db.GetdateForId(dataId).ToString();
                string name = db.Getcompanyforid(dataId);
                
                string path1=Path.Combine(route, date);
                string dynamicPath = Path.Combine(path1, name);
                

                if (string.IsNullOrEmpty(route))
                {
                    MessageBox.Show($"ID {dataId}에 해당하는 경로(route)가 존재하지 않습니다.");
                    return;
                }

                if (!System.IO.Directory.Exists(dynamicPath))
                {
                    MessageBox.Show($"이미지 폴더가 존재하지 않습니다: {dynamicPath}");
                    return;
                }

                // 새 폼에 동적 경로 전달
                Form2 form2 = new Form2(dataId, dynamicPath);
                form2.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }

    }
}
