using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Web;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace 视频号视频下载工具
{


    public partial class VideoManager
    {
        public static VideoData? ParseVideoDataFromJson(string jsonString)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                VideoData videoData = JsonSerializer.Deserialize<VideoData>(jsonString, options);
                return videoData;
            }
            catch
            {
                return null;
            }

        }

        private List<VideoData> videoDataList = new List<VideoData>();



 

  
        public VideoManager()
        {   
        }

        // 添加 VideoData 到列表，但前提是不存在相同的 Url
        public void AddVideoData(VideoData data)
        {
            // 检查列表中是否已存在具有相同 URL 的 VideoData
            var vurl= HttpUtility.ParseQueryString(new Uri(data.Url).Query)["encfilekey"];
            var existingVideo = videoDataList.FirstOrDefault(v => HttpUtility.ParseQueryString(new Uri(v.Url).Query)["encfilekey"] == vurl);

            if (existingVideo == null)
            {
                // 如果不存在，添加新的 VideoData
                videoDataList.Add(data);

            }
            else
            {
                // 如果存在，可以选择更新现有的数据或者简单地不添加/忽略
                // 更新示例：
                // UpdateVideoData(existingVideo, data);
            }
        }

        public async void AddVideoKeyData(VideoKeyData videoKey)
        {
            await Task.Delay(2000);

            var vurl2= HttpUtility.ParseQueryString(new Uri(videoKey.Url).Query)["encfilekey"];

            for (int i = videoDataList.Count - 1; i >= 0; i--)
            {
                var vurl = HttpUtility.ParseQueryString(new Uri(videoDataList[i].Url).Query)["encfilekey"];
             
                if (vurl2==vurl)
                {
                    videoDataList[i].KeyData =(byte[])videoKey.Key.Clone();
                 

                    return;  // Assuming URLs are unique, we can stop once we've found the match
                }
            } 



        }
        public void ClearVideos()
        {
            videoDataList.Clear();
        }
        public List<VideoData> GetVideos()
        {
            return videoDataList.ToList();
        }
    }

}
