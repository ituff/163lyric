using System;
using System.Collections.Generic;

namespace _163lyric
{
    /// <summary>
    /// 搜索到的一首歌曲信息，也是取歌词时传给歌词源的凭据。
    /// </summary>
    class SongInfo
    {
        public string Title = "";        // 歌名
        public string Artist = "";       // 歌手（多个用 / 分隔）
        public string Album = "";        // 专辑
        public int Duration = 0;         // 时长（秒），未知为 0
        public string Source = "";       // 来源名称（网易/酷狗/LRCLIB）

        // 各歌词源的取词凭据，按来源只填对应字段
        public long NetEaseId = 0;           // 网易：歌曲 ID（新歌超过 int 范围，用 long）
        public string KugouHash = "";        // 酷狗：歌曲 hash
        public string KugouKeyword = "";     // 酷狗：歌词候选接口的关键词
        public string LrclibSynced = "";     // LRCLIB：搜索时已带出的带时间轴歌词
        public string LrclibPlain = "";      // LRCLIB：搜索时已带出的无时间轴歌词
    }

    /// <summary>
    /// 歌词源统一接口：先按关键词搜索拿到候选列表，再对选中的歌曲取歌词。
    /// </summary>
    interface ILyricSource
    {
        /// <summary>来源显示名，如"网易"。</summary>
        string Name { get; }

        /// <summary>按关键词（歌名、歌手、专辑的任意组合）搜索，最多返回 20 条。</summary>
        List<SongInfo> Search(string sKeyword);

        /// <summary>
        /// 按字段精确搜索：已知歌名/歌手/专辑时优先调用（比整句关键词更准），
        /// 参数允许为空串，实现自行降级为关键词搜索。
        /// </summary>
        List<SongInfo> SearchDetail(string sTitle, string sArtist, string sAlbum);

        /// <summary>获取指定歌曲的歌词。</summary>
        LyricResult GetLyric(SongInfo oSong);
    }
}
