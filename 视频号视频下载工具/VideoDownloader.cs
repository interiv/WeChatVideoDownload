
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




       
        private string cachePath = Path.Combine(Environment.CurrentDirectory, "cache");

        public VideoDownloader( )
        {
            //this.progress = progress;
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
        }

        /// <summary>
        /// 过滤用, 用于记录已下载的视频链接, 避免重复下载
        /// </summary>
        public List<string> VideoUrls = new List<string>();
        public async Task<bool> DownloadAndDecryptVideoAsync(string videoUrl, byte[] decArray, string _filename)
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

                if (!File.Exists(filename))
                {
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
                                    return false;
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
                        return false;
                    }

                }              
            } 
            return true;
        }

        public async Task<bool> DownloadAndDecryptVideoAsync(string videoUrl, byte[] decArray, string _filename, CancellationToken cancellationToken,  IProgress<double> progress)
        {
            // 使用 HttpUtility.ParseQueryString 来解析查询字符串
            string encfilekey = HttpUtility.ParseQueryString(new Uri(videoUrl).Query)["encfilekey"];
            if (!VideoUrls.Contains(encfilekey))
            {
                VideoUrls.Add(encfilekey);

                // 随机文件名
                string name = _filename;
                string videoFilename = Path.Combine(cachePath, $"{name}.mp4");

                if (!File.Exists(videoFilename))
                {
                    // 开始下载视频
                    try
                    {
                        using (HttpClientHandler handler = new HttpClientHandler())
                        {
                            handler.UseProxy = false; // client 不使用代理
                            using (HttpClient client = new HttpClient(handler))
                            {
                                // 检查是否已经请求取消
                                cancellationToken.ThrowIfCancellationRequested();

                                var response = await client.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                                if (!response.IsSuccessStatusCode)
                                {
                                    return false;
                                }

                                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                                var bytesRead = 0L;
                                var videoData = new List<byte>(); // Store downloaded data

                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {
                                    int read;
                                    var buffer = new byte[81920]; // Larger buffer for more efficient reading

                                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                                    {
                                        videoData.AddRange(buffer.Take(read));
                                        bytesRead += read;
                                        progress?.Report((double)bytesRead / totalBytes); // Report progress
                                    }
                                }

                                // Decrypt the first 131072 bytes (or less if the file is smaller)
                                int lengthToDecrypt = Math.Min(videoData.Count, 131072);
                                for (int i = 0; i < lengthToDecrypt; i++)
                                {
                                    videoData[i] ^= decArray[i]; // Apply XOR decryption
                                }

                                // Write the decrypted data to the file
                                await File.WriteAllBytesAsync(videoFilename, videoData.ToArray(), cancellationToken);

                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // 如果在下载或处理过程中发生取消，确保清理现场
                        if (File.Exists(videoFilename))
                        {
                            File.Delete(videoFilename);
                        }
                        throw; // 重新抛出取消异常以通知调用方操作已取消
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return true;
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
