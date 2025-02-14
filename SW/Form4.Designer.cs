namespace Project
{
    partial class Form4
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
            tdflowLayoutPanel = new FlowLayoutPanel();
            label1 = new Label();
            SuspendLayout();
            // 
            // tdflowLayoutPanel
            // 
            tdflowLayoutPanel.AutoScroll = true;
            tdflowLayoutPanel.Location = new Point(44, 96);
            tdflowLayoutPanel.Name = "tdflowLayoutPanel";
            tdflowLayoutPanel.Size = new Size(1040, 575);
            tdflowLayoutPanel.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("맑은 고딕", 24F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.ForeColor = Color.White;
            label1.Location = new Point(44, 36);
            label1.Name = "label1";
            label1.Size = new Size(176, 45);
            label1.TabIndex = 3;
            label1.Text = "Today List";
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(1123, 712);
            Controls.Add(label1);
            Controls.Add(tdflowLayoutPanel);
            Name = "Form4";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "     ";
            Load += Form4_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel tdflowLayoutPanel;
        private Label label1;
    }
}