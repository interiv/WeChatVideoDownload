
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

            videoDownloader=new VideoDownloader();

        }

        private void NetworkSniffer_DataKeyUpdated(object? sender, VideoKeyDataEventArgs e)
        {
            VideoManager.AddVideoKeyData(e.VideoKeyData);

        }

        private void InitializeListView()
        {
            listView1.View = View.Details; // 设置视图为详细信息模式

            listView1.Columns.Add("Description", "Description", 150);
            listView1.Columns.Add("Uploader", "Uploader", 150);
            listView1.Columns.Add("Decode Key", "Decode Key", 100);
            listView1.Columns.Add("Size", "Size", 100);
            listView1.Columns.Add("Downloaded", "Downloaded", 100);
            listView1.Columns.Add("URL", "URL", 200);
            listView1.Columns.Add("Key Bytes", "Key Bytes", 200);

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
                HttpUtility.ParseQueryString(new Uri(item.SubItems[5].Text).Query)["encfilekey"]))
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
                HttpUtility.ParseQueryString(new Uri(item.SubItems[5].Text).Query)["encfilekey"]);
                string hexString = "";
                if (video.KeyData!=null)
                {
                    hexString =  BitConverter.ToString(video.KeyData, 0, 10).Replace("-", " ");
                }
                if (item == null)
                {

                    // 如果项不存在，则创建新项。
                    item =  new ListViewItem(video.Description);  // 主文本设置为 Description
                    item.SubItems.Add(video.Uploader ?? "");   // 对应 "Uploader" 列
                    item.SubItems.Add(video.DecodeKey ?? "");  // 对应 "Decode Key" 列
                    item.SubItems.Add(video.Size.ToString());  // 对应 "Size" 列
                    item.SubItems.Add("");                    // 对应 "Downloaded" 列，初始为空
                    item.SubItems.Add(video.Url ?? "");        // 对应 "URL" 列
                    item.SubItems.Add(hexString??"");
                    item.Tag = video.KeyData;  // 重新设置或更新 Tag 中的 KeyData
                    listView1.Items.Add(item);
                }
                else
                {
                    if (item.SubItems[6].Text=="")
                    {
                        // 更新现有项的内容，如果它们有变化
                        item.Text = video.Description;
                        item.SubItems[1].Text = video.Uploader ?? "";
                        item.SubItems[2].Text = video.DecodeKey ?? "";
                        item.SubItems[3].Text = video.Size.ToString();
                        item.SubItems[4].Text="";
                        item.SubItems[5].Text = video.Url ?? "";
                        item.SubItems[6].Text=hexString;
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
        private SemaphoreSlim semaphore = new SemaphoreSlim(3);
        private CancellationTokenSource cancellationTokenSource = null;
        private List<Task> tasks = new List<Task>();
        private async void button1_Click(object sender, EventArgs e)
        {

            // 如果之前已经创建了 CancellationTokenSource，那么取消所有任务
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;

                await Task.WhenAll(tasks); // 等待所有已开始的任务完成
                tasks.Clear(); // 清空任务列表

                MessageBox.Show("All tasks have been cancelled or completed.");
                return;
            }

            // 创建新的 CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if ( item.SubItems[6].Text != "")
                {
                    var task = ProcessItemAsync(item, token);
                    tasks.Add(task);
                }
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Tasks were cancelled.");
            }
            finally
            {
                tasks.Clear(); // 完成后清空任务列表
            }

            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;


        }
        private async Task ProcessItemAsync(ListViewItem item, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                string url = item.SubItems[5].Text; // "URL" 列
                byte[] keyBytes = item.Tag as byte[]; // 从 Tag 获取密钥
                long fileSize = long.Parse(item.SubItems[3].Text); // "Size" 列，假设已经是字节为单位
                string filename = item.Text+"-"+fileSize; // 使用 Description 作为文件名
                                                          // 创建一个进度条更新实例
                IProgress<double> progress = new Progress<double>(percent =>
                {
                    this.Invoke(new Action(() =>
                    {
                        item.SubItems[4].Text = $"{(int)(percent * 100)}%"; // 更新下载进度到 "Downloaded" 列
                    }));
                });

                //bool r2= await videoDownloader.DownloadAndDecryptVideoAsync(url, keyBytes, filename);
                bool result = await videoDownloader.DownloadAndDecryptVideoAsync(url, keyBytes, filename, cancellationToken, progress);

                if (!cancellationToken.IsCancellationRequested)
                {
                    this.Invoke(new Action(() =>
                    {
                        item.SubItems[4].Text = result ? "Yes" : "No";
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    item.SubItems[4].Text = "Error"; // 在出错时更新状态
                }));
            }
            finally
            {
                semaphore.Release();
            }
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
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[5].Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            VideoManager.ClearVideos();
        }
    }
}

