using System.Diagnostics;

namespace 视频号视频下载工具
{
    internal static class WeChatHelper
    {
        public static void ClearWeChatProfileDirectory()
        {
            // 获取当前用户的AppData路径
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // 构建目标路径
            string targetPath = Path.Combine(appDataPath, "Tencent", "WeChat", "radium", "web", "profiles");

            // 检查目标路径是否存在
            if (Directory.Exists(targetPath))
            {
                // 删除目标目录下的所有文件和子目录
                DeleteDirectory(targetPath);
            }
            else
            {
                throw new InvalidOperationException("指定的路径不存在。");
            }
        }

        static void DeleteDirectory(string path)
        {
            // 获取所有文件和子目录
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                // 删除文件
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                // 递归删除子目录
                DeleteDirectory(dir);
            }

            // 最后删除空目录
            Directory.Delete(path);
        }

        public static bool IsProcessRunning(string processName)
        {
            // 获取所有同名的进程
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
}