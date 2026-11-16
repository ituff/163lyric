using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace _163lyric
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // net48 默认协议跟随系统，这里显式补上 TLS 1.2，保证 https 歌词源（LRCLIB 等）可用
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
