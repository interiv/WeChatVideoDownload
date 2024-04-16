
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace 视频号视频下载工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeListView();
            IProgress<string> progress = new Progress<string>(message =>
                           {
                               // 确保在UI线程中更新RichTextBox
                               if (this.richTextBox1.InvokeRequired)
                               {
                                   this.richTextBox1.Invoke(new Action<string>(UpdateRichTextBox), message);
                               }
                               else
                               {
                                   UpdateRichTextBox(message);
                               }
                           });
            VideoManager=new VideoManager();
            networkSniffer.DataUpdated+=NetworkSniffer_DataUpdated;
            networkSniffer.DataKeyUpdated+=NetworkSniffer_DataKeyUpdated;

        }

        private void NetworkSniffer_DataKeyUpdated(object? sender, VideoKeyDataEventArgs e)
        {
            VideoManager.AddVideoKeyData(e.VideoKeyData);

        }

        private void InitializeListView()
        {
            listView1.View = View.Details; // 设置视图为详细信息模式

            listView1.Columns.Add("Description", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Uploader", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Decode Key", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Size", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("URL", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Key Bytes", 200, HorizontalAlignment.Left);
            listView1.FullRowSelect = true; // 选择整行

        }
        private void NetworkSniffer_DataUpdated(object? sender, VideoDataEventArgs e)
        {
            VideoManager.AddVideoData(e.VideoData);

            this.Invoke(new Action(() =>
            {
                UpdateUI();
            }));
        }

        private void UpdateUI()
        {


            // 获取当前在 ListView 中显示的视频 URL
            // var currentItems = listView1.Items.Cast<ListViewItem>().Select(item => item.SubItems[0].Text).ToList();
            var newVideos = VideoManager.GetVideos().Cast<VideoData>().ToList();

            //       listView1.BeginUpdate();
            // 移除不再存在的视频
            var list = listView1.Items.Cast<ListViewItem>().ToList();
            foreach (var item in list)
            {
                if (!newVideos.Any(v => HttpUtility.ParseQueryString(new Uri(v.Url).Query)["encfilekey"] ==
                HttpUtility.ParseQueryString(new Uri(item.SubItems[4].Text).Query)["encfilekey"]))
                {
                    listView1.Items.Remove(item);
                }
            }

            // 添加新的视频
            foreach (var video in newVideos)
            {
                // 查找是否已经有这个 URL 的视频
                ListViewItem item = listView1.Items.Cast<ListViewItem>().FirstOrDefault(
                    item => HttpUtility.ParseQueryString(new Uri(video.Url).Query)["encfilekey"] ==
                HttpUtility.ParseQueryString(new Uri(item.SubItems[4].Text).Query)["encfilekey"]);
                string hexString = "";
                if (video.KeyData!=null)
                {
                    hexString =  BitConverter.ToString(video.KeyData, 0, 10).Replace("-", " ");
                }
                if (item == null)
                {

                    // 如果项不存在，则创建新项。
                    item = new ListViewItem(video.Description);  // 主文本设置为 Description
                    item.SubItems.Add(video.Uploader ?? "");     // 添加子项 Uploader
                    item.SubItems.Add(video.DecodeKey ?? "");    // 添加子项 Decode Key
                    item.SubItems.Add(video.Size.ToString());    // 添加子项 Size
                    item.SubItems.Add(video.Url ?? "");          // 添加子项 URL
                    item.SubItems.Add(hexString);
                    item.Tag = video.KeyData;                    // 在 Tag 中存储 KeyData
                    listView1.Items.Add(item);
                }
                else
                {
                    if (item.SubItems[5].Text=="")
                    {
                        // 更新现有项的内容，如果它们有变化
                        item.Text = video.Description;
                        item.SubItems[1].Text = video.Uploader ?? "";
                        item.SubItems[2].Text = video.DecodeKey ?? "";
                        item.SubItems[3].Text = video.Size.ToString();
                        item.SubItems[4].Text = video.Url ?? "";
                        item.SubItems[5].Text=hexString;
                        item.Tag = video.KeyData;  // 重新设置或更新 Tag 中的 KeyData
                    }
                }
            }

            //listView1.EndUpdate();
        }

        NetworkSniffer networkSniffer = new NetworkSniffer();


        VideoManager VideoManager;
        VideoDownloader videoDownloader;

        private void UpdateRichTextBox(string message)
        {
            richTextBox1.AppendText(message + "\n");
            richTextBox1.ScrollToCaret(); // 确保滚动到最新内容
        }
        private void checkBox_开启监听_CheckedChanged(object sender, EventArgs e)
        {


            if (checkBox_开启监听.Checked)
            {
                networkSniffer.Start();
            }
            else
            {
                networkSniffer.Stop();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


           // VideoManager.ClearVideos();


            // 获取导出的函数
            //  var wasm_isaac_generate = instance.GetFunction("wasm_isaac_generate");


        }

        private void timer_更新界面_Tick(object sender, EventArgs e)
        {
            timer_更新界面.Enabled=false;
            UpdateUI();
            timer_更新界面.Enabled=true;
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count>0)
            {
                //剪贴板
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[4].Text);
            }
        }
    }
}

