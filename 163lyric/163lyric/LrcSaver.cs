using System;
using System.IO;
using System.Text;

namespace _163lyric
{
    /// <summary>
    /// 歌词文件保存：与音乐文件同名、扩展名 .lrc、UTF-8 带 BOM（避免老播放器中文乱码）。
    /// </summary>
    class LrcSaver
    {
        /// <summary>把歌词保存到音乐文件同目录的同名 .lrc，返回保存路径。</summary>
        public static string Save(string sMusicPath, string sContent)
        {
            string sLrcPath = Path.ChangeExtension(sMusicPath, ".lrc");
            File.WriteAllText(sLrcPath, sContent, new UTF8Encoding(true));
            return sLrcPath;
        }

        /// <summary>音乐文件旁边是否已存在同名 .lrc。</summary>
        public static bool Exists(string sMusicPath)
        {
            return File.Exists(Path.ChangeExtension(sMusicPath, ".lrc"));
        }
    }
}
