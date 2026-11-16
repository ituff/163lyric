using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _163lyric
{
    /// <summary>
    /// LRCLIB（lrclib.net）歌词源：开源歌词数据库，免 key，支持按歌名/歌手/专辑精确搜索。
    /// 搜索结果里直接带歌词，GetLyric 无需再请求。接口有限速（约90次/分），每次请求前稍作间隔。
    /// </summary>
    class LrclibSource : ILyricSource
    {
        public string Name { get { return "LRCLIB"; } }

        public List<SongInfo> Search(string sKeyword)
        {
            return SearchApi("q=" + Uri.EscapeDataString(sKeyword));
        }

        public List<SongInfo> SearchDetail(string sTitle, string sArtist, string sAlbum)
        {
            // LRCLIB 原生支持按字段搜索，有标签信息时走精确参数
            StringBuilder param = new StringBuilder();
            param.Append("track_name=").Append(Uri.EscapeDataString(sTitle));
            if (sArtist.Length > 0)
                param.Append("&artist_name=").Append(Uri.EscapeDataString(sArtist));
            if (sAlbum.Length > 0)
                param.Append("&album_name=").Append(Uri.EscapeDataString(sAlbum));
            return SearchApi(param.ToString());
        }

        public LyricResult GetLyric(SongInfo oSong)
        {
            LyricResult result = new LyricResult();
            result.Lyric = oSong.LrclibSynced.Length > 0 ? oSong.LrclibSynced : oSong.LrclibPlain;
            result.Lyric = result.Lyric.Trim();
            if (result.Lyric.Length == 0)
                result.Error = "LRCLIB没有该歌曲的歌词。";
            return result;
        }

        private List<SongInfo> SearchApi(string sParams)
        {
            HttpRequest hr = new HttpRequest();
            Thread.Sleep(400);   // LRCLIB 有限速，请求间保持间隔（含批量扫描场景）

            string sContent = hr.getContent("https://lrclib.net/api/search?" + sParams);
            if (sContent.StartsWith("ERR!"))
                throw new Exception("搜索请求失败：" + sContent.Substring(4));

            JArray list;
            try
            {
                list = (JArray)JsonConvert.DeserializeObject(sContent);
            }
            catch (Exception e)
            {
                throw new Exception("解析搜索结果失败：" + e.Message);
            }

            List<SongInfo> songs = new List<SongInfo>();
            if (list == null)
                return songs;

            foreach (JToken item in list)
            {
                SongInfo song = new SongInfo();
                song.Source = Name;
                song.Title = S(item["trackName"]);
                song.Artist = S(item["artistName"]);
                song.Album = S(item["albumName"]);
                song.Duration = I(item["duration"]);          // LRCLIB 返回秒
                song.LrclibSynced = S(item["syncedLyrics"]);  // 带时间轴歌词
                song.LrclibPlain = S(item["plainLyrics"]);    // 纯文本歌词
                songs.Add(song);
            }
            return songs;
        }

        // ---- JSON 小工具 ----

        private static string S(JToken t)
        {
            return t == null ? "" : t.ToString().Trim();
        }

        private static int I(JToken t)
        {
            int v;
            return t != null && int.TryParse(t.ToString(), out v) ? v : 0;
        }
    }
}
