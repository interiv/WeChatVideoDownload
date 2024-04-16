
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace 视频号视频下载工具
{
    public class VideoDownloader
    {




        private IProgress<string> progress;
        private string cachePath = Path.Combine(Environment.CurrentDirectory, "cache");

        public VideoDownloader(IProgress<string> progress)
        {
            //this.progress = progress;
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
        }

        /// <summary>
        /// 过滤用, 用于记录已下载的视频链接, 避免重复下载
        /// </summary>
        public List<string> VideoUrls = new List<string>();
        public async Task DownloadAndDecryptVideoAsync(string videoUrl, byte[] decArray, string _filename)
        {



            // 使用 HttpUtility.ParseQueryString 来解析查询字符串
            string encfilekey = HttpUtility.ParseQueryString(new Uri(videoUrl).Query)["encfilekey"];
            if (!VideoUrls.Contains(encfilekey))
            {
                VideoUrls.Add(encfilekey);

                // Log the intercepted video download link
                // progress.Report($"拦截到视频下载链接: {videoUrl}");

                // Random file name
                string name = _filename;
                string filename = Path.Combine(cachePath, $"{name}_raw.dec");

                // Start downloading video
                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.UseProxy = false;
                        using (HttpClient client = new HttpClient(handler))
                        {

                            //client 不使用代理

                            var response = await client.GetAsync(videoUrl);
                            if (!response.IsSuccessStatusCode)
                            {
                                //  progress.Report($"下载错误, HTTP错误代码: {response.StatusCode}");
                                return;
                            }

                            byte[] videoData = await response.Content.ReadAsByteArrayAsync();
                            File.WriteAllBytes(filename, videoData);
                            //  progress.Report("下载视频完成，开始解密。");

                            // Decrypt video
                            byte[] decryptedVideo = DecryptVideo(videoData, decArray);
                            string videoFilename = Path.Combine(cachePath, $"{name}.mp4");
                            File.WriteAllBytes(videoFilename, decryptedVideo);
                            // progress.Report($"视频已下载，保存的路径: {videoFilename}");

                            // Delete cache
                            File.Delete(filename);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //  progress.Report($"下载或解密过程中出错: {ex.Message}");
                }
            }
        }
        public void DecodeFile()
        {

        }

        private byte[] DecryptVideo(byte[] videoData, byte[] key)
        {
            // Assuming key is the correct size and decryption method
            // This is a simple XOR for demonstration
            byte[] decryptedData = new byte[videoData.Length];
            for (int i = 0; i < videoData.Length; i++)
                decryptedData[i] = (byte)(videoData[i] ^ (i < key.Length ? key[i] : 0));

            return decryptedData;
        }
    }
}
