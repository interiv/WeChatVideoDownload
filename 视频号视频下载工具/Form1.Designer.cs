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
            listView1 = new ListView();
            timer_更新界面 = new System.Windows.Forms.Timer(components);
            button2 = new Button();
            button3 = new Button();
            checkBox_下载所有视频 = new CheckBox();
            button_清空微信浏览器缓存 = new Button();
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
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listView1.Location = new Point(31, 44);
            listView1.Name = "listView1";
            listView1.Size = new Size(861, 528);
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
            // button2
            // 
            button2.Location = new Point(899, 72);
            button2.Name = "button2";
            button2.Size = new Size(142, 23);
            button2.TabIndex = 2;
            button2.Text = "清除数据";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(899, 214);
            button3.Name = "button3";
            button3.Size = new Size(142, 23);
            button3.TabIndex = 2;
            button3.Text = "测试";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            button3.Click += button3_Click;
            // 
            // checkBox_下载所有视频
            // 
            checkBox_下载所有视频.AutoSize = true;
            checkBox_下载所有视频.Location = new Point(899, 44);
            checkBox_下载所有视频.Margin = new Padding(4);
            checkBox_下载所有视频.Name = "checkBox_下载所有视频";
            checkBox_下载所有视频.Size = new Size(99, 21);
            checkBox_下载所有视频.TabIndex = 0;
            checkBox_下载所有视频.Text = "下载所有视频";
            checkBox_下载所有视频.UseVisualStyleBackColor = true;
            checkBox_下载所有视频.CheckedChanged += checkBox_下载所有视频_CheckedChanged;
            // 
            // button_清空微信浏览器缓存
            // 
            button_清空微信浏览器缓存.Location = new Point(118, 15);
            button_清空微信浏览器缓存.Name = "button_清空微信浏览器缓存";
            button_清空微信浏览器缓存.Size = new Size(142, 23);
            button_清空微信浏览器缓存.TabIndex = 2;
            button_清空微信浏览器缓存.Text = "清空微信浏览器缓存";
            button_清空微信浏览器缓存.UseVisualStyleBackColor = true;
            button_清空微信浏览器缓存.Visible = false;
            button_清空微信浏览器缓存.Click += button_清空微信浏览器缓存_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1092, 602);
            Controls.Add(listView1);
            Controls.Add(button3);
            Controls.Add(button_清空微信浏览器缓存);
            Controls.Add(button2);
            Controls.Add(checkBox_下载所有视频);
            Controls.Add(checkBox_开启监听);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "视频号视频下载工具 2024年4月21日";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_开启监听;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Timer timer_更新界面;
        private Button button2;
        private Button button3;
        private CheckBox checkBox_下载所有视频;
        private Button button_清空微信浏览器缓存;
    }
}

