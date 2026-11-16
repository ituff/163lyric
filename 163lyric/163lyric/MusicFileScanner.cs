using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace _163lyric
{
    /// <summary>
    /// 扫描目录中的音乐文件：读取内嵌标签（歌名/歌手/专辑/时长），
    /// 标签缺失时回退解析文件名，并给出与搜索结果的匹配打分。
    /// </summary>
    class MusicFileScanner
    {
        // 支持读取标签的音乐文件扩展名（TagLib# 支持的范围）
        public static readonly string[] Extensions =
            { ".mp3", ".flac", ".m4a", ".aac", ".ogg", ".opus", ".wma", ".wav", ".ape" };

        /// <summary>
        /// 读取单个音乐文件的标签信息；读取失败时用文件名兜底（"歌手 - 歌名"）。
        /// 不会抛异常，总能得到一份可用于搜索的 SongInfo。
        /// </summary>
        public static SongInfo ReadFromFile(string sPath)
        {
            SongInfo song = new SongInfo();
            song.Title = System.IO.Path.GetFileNameWithoutExtension(sPath);

            try
            {
                using (TagLib.File tf = TagLib.File.Create(sPath))
                {
                    if (!string.IsNullOrEmpty(tf.Tag.Title))
                        song.Title = tf.Tag.Title.Trim();
                    string artist = tf.Tag.FirstPerformer;
                    if (string.IsNullOrEmpty(artist))
                        artist = tf.Tag.FirstAlbumArtist;
                    if (!string.IsNullOrEmpty(artist))
                        song.Artist = artist.Trim();
                    if (!string.IsNullOrEmpty(tf.Tag.Album))
                        song.Album = tf.Tag.Album.Trim();
                    if (tf.Properties != null)
                        song.Duration = (int)Math.Round(tf.Properties.Duration.TotalSeconds);
                }
            }
            catch (Exception)
            {
                // 标签读取失败（损坏/格式不支持），继续用文件名兜底
            }

            // 文件名兜底："歌手 - 歌名" 格式拆出歌手
            if (song.Artist.Length == 0)
            {
                int idx = song.Title.IndexOf(" - ");
                if (idx > 0)
                {
                    song.Artist = song.Title.Substring(0, idx).Trim();
                    song.Title = song.Title.Substring(idx + 3).Trim();
                }
            }
            return song;
        }

        /// <summary>
        /// 匹配打分：标题相等50（部分包含25）+ 歌手一致30（部分重叠20）+ 时长±3秒20（±8秒10，差太多倒扣）。
        /// </summary>
        public static int Score(SongInfo oTag, SongInfo oHit)
        {
            int score = 0;

            string t1 = Normalize(oTag.Title), t2 = Normalize(oHit.Title);
            if (t1.Length > 0 && t2.Length > 0)
            {
                if (t1 == t2)
                    score += 50;
                else if (t1.Contains(t2) || t2.Contains(t1))
                    score += 25;
            }

            string a1 = Normalize(oTag.Artist), a2 = Normalize(oHit.Artist);
            if (a1.Length > 0 && a2.Length > 0)
            {
                // 歌手可能带多位（a/b），逐个比较取最好结果
                int artist = 0;
                foreach (string s1 in a1.Split('/'))
                {
                    if (s1.Length == 0)
                        continue;
                    foreach (string s2 in a2.Split('/'))
                    {
                        if (s2.Length == 0)
                            continue;
                        if (s1 == s2)
                            artist = Math.Max(artist, 30);
                        else if (s1.Contains(s2) || s2.Contains(s1))
                            artist = Math.Max(artist, 20);
                    }
                }
                score += artist;
            }

            if (oTag.Duration > 0 && oHit.Duration > 0)
            {
                int diff = Math.Abs(oTag.Duration - oHit.Duration);
                if (diff <= 3)
                    score += 20;
                else if (diff <= 8)
                    score += 10;
                else
                    score -= 20;   // 时长对不上，压低可疑结果
            }

            return score;
        }

        /// <summary>
        /// 批量扫描时的最低保存分数：标题匹配（50）之外，还要求歌手或时长至少一项对得上。
        /// </summary>
        public const int MatchThreshold = 60;

        /// <summary>
        /// 归一化：转小写、全角转半角、去掉括号内容和所有空白，用于标题/歌手比对。
        /// </summary>
        public static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            s = s.ToLower();

            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c >= 0xFF01 && c <= 0xFF5E)
                    sb.Append((char)(c - 0xFEE0));      // 全角字母数字标点 → 半角
                else if (c == 0x3000)
                    sb.Append(' ');                      // 全角空格
                else
                    sb.Append(c);
            }
            s = sb.ToString();

            s = Regex.Replace(s, @"\([^)]*\)", "");      // 去括号内容（含 Live/Remix 等标注）
            s = Regex.Replace(s, @"\s", "");
            return s;
        }
    }
}
