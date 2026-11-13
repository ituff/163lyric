using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace _163lyric
{
    /// <summary>
    /// 歌词接口的返回结果。
    /// </summary>
    class LyricResult
    {
        public string Lyric = "";   // 原文歌词（LRC）
        public string TLyric = "";  // 翻译歌词（LRC，可能为空）
        public string Error = "";   // 错误信息，为空表示获取成功

        public bool HasError
        {
            get { return Error.Length > 0; }
        }

        /// <summary>
        /// 把翻译歌词按时间轴合并进原文歌词，翻译行紧跟在对应原文行后面，
        /// 与网易云音乐客户端的显示方式一致。没有翻译时直接返回原文。
        /// </summary>
        public override string ToString()
        {
            if (Lyric.Length == 0 || TLyric.Length == 0)
                return Lyric;

            // 翻译歌词按时间轴（[mm:ss.xxx] 中的内容）建立索引
            Dictionary<string, string> translations = new Dictionary<string, string>();
            foreach (string line in SplitLines(TLyric))
            {
                string[] parts = SplitTimestamp(line);
                if (parts != null)
                    translations[parts[0]] = parts[1];
            }

            // 遍历原文歌词，把时间轴相同的翻译行插到后面
            StringBuilder merged = new StringBuilder();
            foreach (string line in SplitLines(Lyric))
            {
                merged.AppendLine(line);
                string[] parts = SplitTimestamp(line);
                if (parts != null)
                {
                    string t;
                    if (translations.TryGetValue(parts[0], out t))
                        merged.AppendLine("[" + parts[0] + "]" + t);
                }
            }
            return merged.ToString();
        }

        private static string[] SplitLines(string s)
        {
            return s.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        // 提取行首时间轴："[mm:ss.xxx]内容" -> ["mm:ss.xxx", "内容"]，不是时间轴行返回 null
        private static string[] SplitTimestamp(string line)
        {
            if (!line.StartsWith("["))
                return null;
            int end = line.IndexOf(']');
            if (end <= 1)
                return null;
            return new string[] { line.Substring(1, end - 1), line.Substring(end + 1) };
        }
    }

    class NetEaseLyric
    {
        /// <summary>
        /// 从网易云音乐接口获取歌词（含翻译歌词）。
        /// </summary>
        public LyricResult getLyric(int iID)
        {
            LyricResult result = new LyricResult();

            HttpRequest hr = new HttpRequest();
            string sContent = hr.getContent("https://music.163.com/api/song/lyric?id=" + iID + "&lv=1&kv=1&tv=-1");

            if (sContent.StartsWith("ERR!"))
            {
                result.Error = "获取歌词失败！错误：\r\n" + sContent.Substring(4);
                return result;
            }

            try
            {
                // 反序列化JSON数据
                JObject o = (JObject)JsonConvert.DeserializeObject(sContent);

                JObject lrc = o["lrc"] as JObject;
                if (lrc != null)
                    result.Lyric = lrc["lyric"].ToString().Trim();

                JObject tlrc = o["tlyric"] as JObject;
                if (tlrc != null)
                    result.TLyric = tlrc["lyric"].ToString().Trim();

                if (result.Lyric.Length == 0)
                    result.Error = "该歌曲没有歌词。";
            }
            catch (Exception e)
            {
                result.Error = "解析歌词失败！错误：\r\n" + e.Message;
            }

            return result;
        }
    }
}
