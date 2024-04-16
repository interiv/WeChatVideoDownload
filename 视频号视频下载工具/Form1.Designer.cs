namespace 视频号视频下载工具
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
            components = new System.ComponentModel.Container();
            checkBox_开启监听 = new CheckBox();
            richTextBox1 = new RichTextBox();
            button1 = new Button();
            listView1 = new ListView();
            timer_更新界面 = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // checkBox_开启监听
            // 
            checkBox_开启监听.AutoSize = true;
            checkBox_开启监听.Location = new Point(36, 17);
            checkBox_开启监听.Margin = new Padding(4);
            checkBox_开启监听.Name = "checkBox_开启监听";
            checkBox_开启监听.Size = new Size(75, 21);
            checkBox_开启监听.TabIndex = 0;
            checkBox_开启监听.Text = "开启监听";
            checkBox_开启监听.UseVisualStyleBackColor = true;
            checkBox_开启监听.CheckedChanged += checkBox_开启监听_CheckedChanged;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(31, 317);
            richTextBox1.Margin = new Padding(4);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(861, 257);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // button1
            // 
            button1.Location = new Point(898, 44);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listView1
            // 
            listView1.Location = new Point(31, 44);
            listView1.Name = "listView1";
            listView1.Size = new Size(861, 266);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
            // 
            // timer_更新界面
            // 
            timer_更新界面.Enabled = true;
            timer_更新界面.Interval = 1000;
            timer_更新界面.Tick += timer_更新界面_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1092, 602);
            Controls.Add(listView1);
            Controls.Add(button1);
            Controls.Add(richTextBox1);
            Controls.Add(checkBox_开启监听);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_开启监听;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Timer timer_更新界面;
    }
}

