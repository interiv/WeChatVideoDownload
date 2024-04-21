using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace 视频号视频下载工具
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

 



            Application.Run(new Form1());
        }
    }
}