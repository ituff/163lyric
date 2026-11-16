using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _163lyric
{
    /// <summary>
    /// 网易云音乐歌词源：关键词搜索 + 按 ID 取歌词（含翻译歌词）。
    /// </summary>
    class NetEaseSource : ILyricSource
    {
        private NetEaseLyric lyricApi = new NetEaseLyric();

        public string Name { get { return "网易"; } }

        public List<SongInfo> Search(string sKeyword)
        {
            HttpRequest hr = new HttpRequest();
            string sContent = hr.getContent("https://music.163.com/api/search/get/web?s="
                + Uri.EscapeDataString(sKeyword) + "&type=1&offset=0&total=true&limit=20");

            if (sContent.StartsWith("ERR!"))
                throw new Exception("搜索请求失败：" + sContent.Substring(4));

            JObject o = ParseJson(sContent);

            List<SongInfo> songs = new List<SongInfo>();
            JObject result = o["result"] as JObject;
            JArray list = result == null ? null : result["songs"] as JArray;
            if (list == null)
                return songs;

            foreach (JToken item in list)
            {
                SongInfo song = new SongInfo();
                song.Source = Name;
                song.Title = S(item["name"]);
                song.NetEaseId = L(item["id"]);
                song.Duration = I(item["duration"]) / 1000;   // 网易返回毫秒
                song.Artist = JoinNames(item["artists"] as JArray);
                JObject album = item["album"] as JObject;
                if (album != null)
                    song.Album = S(album["name"]);
                songs.Add(song);
            }
            return songs;
        }

        public List<SongInfo> SearchDetail(string sTitle, string sArtist, string sAlbum)
        {
            // 网易搜索只接受整句关键词，把已知字段拼起来
            StringBuilder kw = new StringBuilder(sTitle);
            if (sArtist.Length > 0)
                kw.Append(' ').Append(sArtist);
            if (sAlbum.Length > 0)
                kw.Append(' ').Append(sAlbum);
            return Search(kw.ToString());
        }

        public LyricResult GetLyric(SongInfo oSong)
        {
            if (oSong.NetEaseId <= 0)
            {
                LyricResult err = new LyricResult();
                err.Error = "缺少网易歌曲 ID。";
                return err;
            }
            return lyricApi.getLyric(oSong.NetEaseId);
        }

        // ---- JSON 小工具 ----

        private static JObject ParseJson(string s)
        {
            return (JObject)JsonConvert.DeserializeObject(s);
        }

        private static string S(JToken t)
        {
            return t == null ? "" : t.ToString().Trim();
        }

        private static int I(JToken t)
        {
            int v;
            return t != null && int.TryParse(t.ToString(), out v) ? v : 0;
        }

        private static long L(JToken t)
        {
            long v;
            return t != null && long.TryParse(t.ToString(), out v) ? v : 0;
        }

        private static string JoinNames(JArray arr)
        {
            if (arr == null)
                return "";
            List<string> names = new List<string>();
            foreach (JToken a in arr)
            {
                string s = S(a["name"]);
                if (s.Length > 0)
                    names.Add(s);
            }
            return string.Join("/", names.ToArray());
        }
    }
}
