

namespace 视频号视频下载工具
{
    public class VideoKeyDataEventArgs : EventArgs
    {
        public VideoKeyData VideoKeyData { get; private set; }

        public VideoKeyDataEventArgs(VideoKeyData videoKeyData)
        {
            VideoKeyData = videoKeyData;
        }
    }
}
