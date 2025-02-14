using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Project
{
    internal class RecentList
    {
        private const int MaxItemCount = 30;
        public void ListData(string companyname,string time,string state,FlowLayoutPanel targetPanel)
        {
            //targetPanel.Controls.Clear(); // 기존 데이터 제거
            if (targetPanel.Controls.Count >= MaxItemCount)
            {
                targetPanel.Controls.RemoveAt(0); // 첫 번째 항목 제거
            }

            // 텍스트 형태로 데이터를 표시할 Label 추가
            Label lblData = new Label
            {
                
                Text = $"Company : {companyname}   Time : {time}   Result : {state}",
                AutoSize = false, // 크기를 고정
                Font = new Font("Arial", 12, FontStyle.Regular), // 글자 크기 및 스타일 조정
                TextAlign = ContentAlignment.MiddleLeft, // 텍스트 정렬
                Size = new Size(440, 30), // 라벨 크기 설정
                Margin = new Padding(1), // 라벨 간 간격
                ForeColor = Color.White // 글씨 색을 흰색으로 설정
            };

            // FlowLayoutPanel에 Label 추가
            targetPanel.Controls.Add(lblData);
        }
        public void ListData(DataTable dataTable, FlowLayoutPanel targetPanel)
        {
            //targetPanel.Controls.Clear(); // 기존 데이터 제거
            if (targetPanel.Controls.Count >= MaxItemCount)
            {
                targetPanel.Controls.RemoveAt(0); // 첫 번째 항목 제거
            }



            foreach (DataRow row in dataTable.Rows)
            {
                    // 텍스트 형태로 데이터를 표시할 Label 추가
                    Label lblData = new Label
                    {
                        Text = $"Company: {row["name"]}, " +
                               $"Time: {row["Date"]}, " +
                               $"Result: {row["total"]}",
                        AutoSize = false, // 크기를 고정
                        Font = new Font("Arial", 15, FontStyle.Regular), // 글자 크기 및 스타일 조정
                        TextAlign = ContentAlignment.MiddleLeft, // 텍스트 정렬
                        Size = new Size(440, 30), // 라벨 크기 설정
                        Margin = new Padding(1), // 라벨 간 간격
                        ForeColor = Color.White // 글씨 색을 흰색으로 설정
                    };

                    // FlowLayoutPanel에 Label 추가
                    targetPanel.Controls.Add(lblData);
            }
        }
    }
}
