using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _163lyric
{
    /// <summary>
    /// 酷狗音乐歌词源：关键词搜索拿 hash，再经歌词候选接口（krcs）与下载接口（lyrics）取得 LRC。
    /// 全程无需登录或 cookie，接口来自 musicdl 等开源项目的公开用法。
    /// </summary>
    class KugouSource : ILyricSource
    {
        public string Name { get { return "酷狗"; } }

        public List<SongInfo> Search(string sKeyword)
        {
            HttpRequest hr = new HttpRequest();
            // 该 host 的 https 证书无效，走 http（音乐客户端同行做法）
            string sContent = hr.getContent("http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword="
                + Uri.EscapeDataString(sKeyword) + "&page=1&pagesize=20");

            if (sContent.StartsWith("ERR!"))
                throw new Exception("搜索请求失败：" + sContent.Substring(4));

            JObject o = ParseJson(sContent);

            List<SongInfo> songs = new List<SongInfo>();
            JObject data = o["data"] as JObject;
            JArray list = data == null ? null : data["info"] as JArray;
            if (list == null)
                return songs;

            foreach (JToken item in list)
            {
                SongInfo song = new SongInfo();
                song.Source = Name;
                song.Title = S(item["songname"]);
                song.Artist = S(item["singername"]);
                song.Album = S(item["album_name"]);
                song.Duration = I(item["duration"]);          // 酷狗返回秒
                song.KugouHash = S(item["hash"]);
                // 歌词候选接口同样要一份关键词，按"歌手 - 歌名"组织
                song.KugouKeyword = (song.Artist.Length > 0 ? song.Artist + " - " : "") + song.Title;
                songs.Add(song);
            }
            return songs;
        }

        public List<SongInfo> SearchDetail(string sTitle, string sArtist, string sAlbum)
        {
            StringBuilder kw = new StringBuilder(sTitle);
            if (sArtist.Length > 0)
                kw.Append(' ').Append(sArtist);
            if (sAlbum.Length > 0)
                kw.Append(' ').Append(sAlbum);
            return Search(kw.ToString());
        }

        public LyricResult GetLyric(SongInfo oSong)
        {
            LyricResult result = new LyricResult();

            if (string.IsNullOrEmpty(oSong.KugouHash))
            {
                result.Error = "缺少酷狗歌曲 hash。";
                return result;
            }

            HttpRequest hr = new HttpRequest();

            // 1) 歌词候选：返回 candidates 列表，每项带 id + accesskey
            string sCand = hr.getContent("https://krcs.kugou.com/search?ver=1&man=yes&client=mobi&keyword="
                + Uri.EscapeDataString(oSong.KugouKeyword) + "&duration=" + (oSong.Duration * 1000)
                + "&hash=" + oSong.KugouHash);
            if (sCand.StartsWith("ERR!"))
            {
                result.Error = "获取歌词失败！错误：\r\n" + sCand.Substring(4);
                return result;
            }

            JArray candidates;
            try
            {
                JObject oCand = ParseJson(sCand);
                candidates = oCand["candidates"] as JArray;
            }
            catch (Exception e)
            {
                result.Error = "解析歌词候选失败！错误：\r\n" + e.Message;
                return result;
            }

            if (candidates == null || candidates.Count == 0)
            {
                result.Error = "酷狗没有该歌曲的歌词。";
                return result;
            }

            // 有时长信息时挑最接近的候选，否则取第一个
            JToken sel = candidates[0];
            if (oSong.Duration > 0)
            {
                int bestDiff = int.MaxValue;
                foreach (JToken c in candidates)
                {
                    int dur = I(c["duration"]);           // 候选时长为毫秒
                    if (dur <= 0)
                        continue;
                    int diff = Math.Abs(dur - oSong.Duration * 1000);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        sel = c;
                    }
                }
            }

            // 2) 下载歌词：content 为 base64 编码的 LRC 文本
            string sDl = hr.getContent("https://lyrics.kugou.com/download?ver=1&client=pc&id="
                + S(sel["id"]) + "&accesskey=" + Uri.EscapeDataString(S(sel["accesskey"]))
                + "&fmt=lrc&charset=utf8");
            if (sDl.StartsWith("ERR!"))
            {
                result.Error = "下载歌词失败！错误：\r\n" + sDl.Substring(4);
                return result;
            }

            try
            {
                JObject oDl = ParseJson(sDl);
                string content = S(oDl["content"]);
                if (content.Length == 0)
                {
                    result.Error = "酷狗没有返回歌词内容。";
                    return result;
                }
                result.Lyric = Encoding.UTF8.GetString(Convert.FromBase64String(content)).Trim();
            }
            catch (Exception e)
            {
                result.Error = "歌词解码失败！错误：\r\n" + e.Message;
            }

            return result;
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
    }
}
