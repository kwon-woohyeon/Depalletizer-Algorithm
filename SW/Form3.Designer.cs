namespace Project
{
    partial class Form3
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
            flowLayoutPanel3 = new FlowLayoutPanel();
            button1 = new Button();
            companytextbox = new TextBox();
            dateTimePicker1 = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoScroll = true;
            flowLayoutPanel3.Location = new Point(40, 133);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(1040, 541);
            flowLayoutPanel3.TabIndex = 13;
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            button1.ForeColor = Color.White;
            button1.Location = new Point(916, 61);
            button1.Name = "button1";
            button1.Size = new Size(94, 27);
            button1.TabIndex = 12;
            button1.Text = "Search";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // companytextbox
            // 
            companytextbox.BackColor = Color.FromArgb(64, 64, 64);
            companytextbox.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            companytextbox.ForeColor = Color.White;
            companytextbox.Location = new Point(617, 61);
            companytextbox.Multiline = true;
            companytextbox.Name = "companytextbox";
            companytextbox.Size = new Size(224, 28);
            companytextbox.TabIndex = 9;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.CalendarFont = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            dateTimePicker1.CalendarMonthBackground = Color.White;
            dateTimePicker1.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 129);
            dateTimePicker1.Location = new Point(180, 61);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(222, 27);
            dateTimePicker1.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.ForeColor = Color.White;
            label1.Location = new Point(80, 54);
            label1.Name = "label1";
            label1.Size = new Size(72, 35);
            label1.TabIndex = 14;
            label1.Text = "Date";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.ForeColor = Color.White;
            label2.Location = new Point(475, 54);
            label2.Name = "label2";
            label2.Size = new Size(127, 35);
            label2.TabIndex = 14;
            label2.Text = "Company";
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(1123, 712);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(flowLayoutPanel3);
            Controls.Add(button1);
            Controls.Add(companytextbox);
            Controls.Add(dateTimePicker1);
            Name = "Form3";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel flowLayoutPanel3;
        private Button button1;
        private TextBox companytextbox;
        private DateTimePicker dateTimePicker1;
        private Label label1;
        private Label label2;
    }
}