using System;
using System.Linq;

namespace 视频号视频下载工具
{
    public class VideoDataEventArgs : EventArgs
    {
        public VideoData VideoData { get; private set; }

        public VideoDataEventArgs(VideoData videoData)
        {
            VideoData = videoData;
        }
    }  
}
