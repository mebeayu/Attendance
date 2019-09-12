namespace AttClient
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.text_Info = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // text_Info
            // 
            this.text_Info.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.text_Info.ForeColor = System.Drawing.Color.LimeGreen;
            this.text_Info.Location = new System.Drawing.Point(2, 2);
            this.text_Info.Multiline = true;
            this.text_Info.Name = "text_Info";
            this.text_Info.ReadOnly = true;
            this.text_Info.Size = new System.Drawing.Size(676, 469);
            this.text_Info.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 475);
            this.Controls.Add(this.text_Info);
            this.Name = "Form1";
            this.Text = "考勤数据监控";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_Info;
    }
}

