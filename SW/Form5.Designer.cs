namespace Project
{
    partial class Form5
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            timer1 = new System.Windows.Forms.Timer(components);
            panel2 = new Panel();
            label4 = new Label();
            label3 = new Label();
            chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            panel3 = new Panel();
            label5 = new Label();
            label6 = new Label();
            panel4 = new Panel();
            panel5 = new Panel();
            label7 = new Label();
            panel6 = new Panel();
            chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            label8 = new Label();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chart2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chart3).BeginInit();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // panel2
            // 
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(chart2);
            panel2.Controls.Add(chart1);
            panel2.Location = new Point(35, 185);
            panel2.Name = "panel2";
            panel2.Size = new Size(422, 237);
            panel2.TabIndex = 3;
            panel2.Paint += panel2_Paint;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.ForeColor = Color.White;
            label4.Location = new Point(221, 15);
            label4.Name = "label4";
            label4.Size = new Size(100, 21);
            label4.TabIndex = 3;
            label4.Text = "Defect Rate";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.ForeColor = Color.White;
            label3.Location = new Point(3, 15);
            label3.Name = "label3";
            label3.Size = new Size(92, 21);
            label3.TabIndex = 2;
            label3.Text = "Good Rate";
            // 
            // chart2
            // 
            chart2.BackColor = Color.Transparent;
            chart2.BorderlineColor = Color.Transparent;
            chartArea1.BackColor = Color.Transparent;
            chartArea1.Name = "ChartArea1";
            chart2.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            chart2.Legends.Add(legend1);
            chart2.Location = new Point(221, 51);
            chart2.Name = "chart2";
            chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Grayscale;
            series1.BackSecondaryColor = Color.White;
            series1.BorderColor = Color.White;
            series1.BorderWidth = 0;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series1.Legend = "Legend1";
            series1.Name = "Series2";
            chart2.Series.Add(series1);
            chart2.Size = new Size(186, 175);
            chart2.TabIndex = 1;
            chart2.Text = "chart2";
            chart2.Paint += chart2_Paint;
            // 
            // chart1
            // 
            chart1.BackColor = Color.Transparent;
            chart1.BorderlineColor = Color.Transparent;
            chartArea2.BackColor = Color.Transparent;
            chartArea2.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea2);
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            chart1.Legends.Add(legend2);
            chart1.Location = new Point(3, 51);
            chart1.Name = "chart1";
            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Grayscale;
            series2.BackSecondaryColor = Color.White;
            series2.BorderColor = Color.White;
            series2.BorderWidth = 0;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            chart1.Series.Add(series2);
            chart1.Size = new Size(186, 175);
            chart1.TabIndex = 0;
            chart1.Text = "chart1";
            chart1.Paint += chart1_Paint;
            // 
            // panel3
            // 
            panel3.Controls.Add(label5);
            panel3.Location = new Point(35, 74);
            panel3.Name = "panel3";
            panel3.Size = new Size(200, 85);
            panel3.TabIndex = 4;
            panel3.Paint += panel3_Paint;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.ForeColor = SystemColors.ControlLightLight;
            label5.Location = new Point(3, 11);
            label5.Name = "label5";
            label5.Size = new Size(131, 21);
            label5.TabIndex = 5;
            label5.Text = "Target Quantity";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Transparent;
            label6.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.ForeColor = SystemColors.ControlLightLight;
            label6.Location = new Point(3, 11);
            label6.Name = "label6";
            label6.Size = new Size(124, 21);
            label6.TabIndex = 6;
            label6.Text = "Good Quantity";
            // 
            // panel4
            // 
            panel4.Controls.Add(label6);
            panel4.Location = new Point(270, 74);
            panel4.Name = "panel4";
            panel4.Size = new Size(200, 85);
            panel4.TabIndex = 7;
            panel4.Paint += panel4_Paint;
            // 
            // panel5
            // 
            panel5.Controls.Add(label7);
            panel5.Location = new Point(508, 74);
            panel5.Name = "panel5";
            panel5.Size = new Size(200, 85);
            panel5.TabIndex = 8;
            panel5.Paint += panel5_Paint;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label7.ForeColor = SystemColors.ControlLightLight;
            label7.Location = new Point(3, 11);
            label7.Name = "label7";
            label7.Size = new Size(154, 21);
            label7.TabIndex = 5;
            label7.Text = "Defective Quantity";
            // 
            // panel6
            // 
            panel6.Controls.Add(chart3);
            panel6.Controls.Add(label8);
            panel6.Location = new Point(479, 185);
            panel6.Name = "panel6";
            panel6.Size = new Size(229, 237);
            panel6.TabIndex = 9;
            panel6.Paint += panel6_Paint;
            // 
            // chart3
            // 
            chart3.BackColor = Color.Transparent;
            chart3.BorderlineColor = Color.Transparent;
            chartArea3.BackColor = Color.Transparent;
            chartArea3.Name = "ChartArea1";
            chart3.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            chart3.Legends.Add(legend3);
            chart3.Location = new Point(20, 48);
            chart3.Name = "chart3";
            chart3.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Grayscale;
            series3.BackSecondaryColor = Color.White;
            series3.BorderColor = Color.White;
            series3.BorderWidth = 0;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            chart3.Series.Add(series3);
            chart3.Size = new Size(186, 175);
            chart3.TabIndex = 10;
            chart3.Text = "chart3";
            chart3.Paint += chart3_Paint;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label8.ForeColor = SystemColors.ControlLightLight;
            label8.Location = new Point(3, 11);
            label8.Name = "label8";
            label8.Size = new Size(113, 21);
            label8.TabIndex = 5;
            label8.Text = "Defect Status";
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(742, 493);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Name = "Form5";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form5";
            Load += Form5_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chart2).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chart3).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private Panel panel2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private Label label4;
        private Label label3;
        private Panel panel3;
        private Label label5;
        private Label label6;
        private Panel panel4;
        private Panel panel5;
        private Label label7;
        private Panel panel6;
        private Label label8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
    }
}