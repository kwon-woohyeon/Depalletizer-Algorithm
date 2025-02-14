using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Project
{
    public partial class Form2 : Form
    {
        private int dataId;
        private string dynamicPath;
        public string Textdate;
        public string result;
        public Form2()
        {
            InitializeComponent();

        }
        public Form2(int dataId, string dynamicPath)
        {
            InitializeComponent();
            this.dataId = dataId;
            this.dynamicPath = dynamicPath;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                // Check if the directory exists
                if (!Directory.Exists(dynamicPath))
                {
                    MessageBox.Show("이미지 폴더가 존재하지 않습니다.");
                    return;
                }

                // Get all image files in the folder
                var imageFiles = Directory.GetFiles(dynamicPath)
                    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                   file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (imageFiles.Length == 0)
                {
                    MessageBox.Show("이미지 파일이 없습니다.");
                    return;
                }

                // Display images dynamically
                DisplayImages(imageFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }
        private void DisplayImages(string[] imageFiles)
        {
            // Create a FlowLayoutPanel to hold the images
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true
            };

            foreach (var imageFile in imageFiles)
            {
                // Create a panel for each image
                Panel imagePanel = new Panel
                {
                    Width = 920,
                    Height = 500,
                    Margin = new Padding(10)
                };

                // Create a PictureBox for the image
                PictureBox pictureBox = new PictureBox
                {
                    ImageLocation = imageFile,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 920,
                    Height = 460,
                    Dock = DockStyle.Top
                };

                string Text = Path.GetFileName(imageFile);
                string[] text_1 = Text.Split('_');
                Textdate = Path.Combine(text_1[1] + text_1[2]+text_1[3]);
                result = text_1[4];
                // Create a Label for the file name
                Label label = new Label
                { 
                    Text = "시간  " + text_1[1]+":" + text_1[2] + ":" + text_1[3] + "   index : " +text_1[4] + "                      " + "결과 : " + text_1[5],
                    AutoSize = false,
                    Font = new Font("Arial", 20, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Bottom,
                    Height = 30
                };

                // Add PictureBox and Label to the panel
                imagePanel.Controls.Add(pictureBox);
                imagePanel.Controls.Add(label);

                // Add the panel to the FlowLayoutPanel
                flowLayoutPanel.Controls.Add(imagePanel);
            }

            // Add FlowLayoutPanel to the Form
            Controls.Add(flowLayoutPanel);
        }
    }
}

