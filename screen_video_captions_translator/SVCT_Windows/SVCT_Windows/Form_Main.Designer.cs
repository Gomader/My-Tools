namespace SVCT_Windows
{
    partial class Form_Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.source = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.target = new System.Windows.Forms.ComboBox();
            this.start = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MV Boli", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(294, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 85);
            this.label1.TabIndex = 0;
            this.label1.Text = "SVCT";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "源语言";
            // 
            // source
            // 
            this.source.FormattingEnabled = true;
            this.source.Items.AddRange(new object[] {
            "韩语",
            "英文",
            "汉语"});
            this.source.Location = new System.Drawing.Point(165, 121);
            this.source.Name = "source";
            this.source.Size = new System.Drawing.Size(121, 32);
            this.source.TabIndex = 2;
            this.source.SelectedIndexChanged += new System.EventHandler(this.source_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "目标语言";
            // 
            // target
            // 
            this.target.FormattingEnabled = true;
            this.target.Items.AddRange(new object[] {
            "汉语",
            "英语",
            "韩语"});
            this.target.Location = new System.Drawing.Point(546, 120);
            this.target.Name = "target";
            this.target.Size = new System.Drawing.Size(121, 32);
            this.target.TabIndex = 4;
            this.target.SelectedIndexChanged += new System.EventHandler(this.target_SelectedIndexChanged);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(165, 378);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(150, 42);
            this.start.TabIndex = 5;
            this.start.Text = "开始翻译";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click_1);
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(495, 378);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(150, 42);
            this.exit.TabIndex = 6;
            this.exit.Text = "退出";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.start);
            this.Controls.Add(this.target);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.source);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox source;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox target;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button exit;
    }
}

