using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _163lyric
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 标题栏/任务栏图标取自 exe 内嵌的 Win32 图标（app.ico）
            try
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            // 关于页：版本号与嵌入的图片资源（logo、公众号二维码）
            lblName.Text = "OneLyric v" + Updater.CurrentVersion;
            pbLogo.Image = LoadResourceImage("_163lyric.logo-256.png");
            pbQr.Image = LoadResourceImage("_163lyric.wechat-qrcode.png");
        }

        /// <summary>从 exe 嵌入资源加载图片（复制进内存流，避免依赖已释放的资源流）。</summary>
        private static Image LoadResourceImage(string sName)
        {
            try
            {
                using (Stream rs = typeof(Form1).Assembly.GetManifestResourceStream(sName))
                {
                    if (rs == null)
                        return null;
                    MemoryStream ms = new MemoryStream();
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        // ==================== 搜索歌词 ====================

        private List<SongInfo> m_SearchResults = null;   // 当前列表对应的搜索结果
        private string m_CurrentLyric = "";              // 当前预览的歌词文本
        private SongInfo m_CurrentSong = null;           // 当前预览歌词对应的歌曲

        private ILyricSource MakeSource()
        {
            if (SrcKugou.Checked)
                return new KugouSource();
            if (SrcLrclib.Checked)
                return new LrclibSource();
            return new NetEaseSource();
        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {
            string sKeyword = Keywordtb.Text.Trim();
            if (sKeyword.Length == 0)
            {
                MessageBox.Show("请输入歌名、歌手或专辑关键词！");
                return;
            }

            ILyricSource source = MakeSource();
            SetBusy(Searchbtn, true);
            Task.Run((Action)(() =>
            {
                string sError = "";
                List<SongInfo> results = null;
                try { results = source.Search(sKeyword); }
                catch (Exception ex) { sError = ex.Message; }

                BeginInvoke((Action)(() =>
                {
                    SetBusy(Searchbtn, false);
                    if (sError.Length > 0)
                    {
                        MessageBox.Show("搜索失败！错误：\r\n" + sError);
                        return;
                    }
                    ShowSearchResults(results);
                }));
            }));
        }

        private void ShowSearchResults(List<SongInfo> results)
        {
            m_SearchResults = results ?? new List<SongInfo>();
            ResultList.BeginUpdate();
            ResultList.Items.Clear();
            foreach (SongInfo song in m_SearchResults)
            {
                ListViewItem item = new ListViewItem(new string[] {
                    song.Title, song.Artist, song.Album, FormatDuration(song.Duration) });
                item.Tag = song;
                ResultList.Items.Add(item);
            }
            ResultList.EndUpdate();

            if (m_SearchResults.Count == 0)
                MessageBox.Show("没有搜到相关歌曲。");
        }

        private void ResultList_DoubleClick(object sender, EventArgs e)
        {
            DLbtn_Click(sender, e);
        }

        private void DLbtn_Click(object sender, EventArgs e)
        {
            if (ResultList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先在列表中选中一首歌曲。");
                return;
            }

            SongInfo song = (SongInfo)ResultList.SelectedItems[0].Tag;
            ILyricSource source = MakeSource();
            SetBusy(DLbtn, true);
            Task.Run((Action)(() =>
            {
                LyricResult result = source.GetLyric(song);
                BeginInvoke((Action)(() =>
                {
                    SetBusy(DLbtn, false);
                    if (result.HasError)
                    {
                        MessageBox.Show(result.Error);
                        return;
                    }
                    m_CurrentLyric = result.ToString();
                    m_CurrentSong = song;
                    SearchLyrictb.Text = m_CurrentLyric;
                }));
            }));
        }

        private void Save2btn_Click(object sender, EventArgs e)
        {
            if (m_CurrentLyric.Length == 0)
            {
                MessageBox.Show("请先获取歌词。");
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "LRC歌词文件 (*.lrc)|*.lrc|所有文件 (*.*)|*.*";
                dlg.FileName = DefaultLyricName(m_CurrentSong);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, m_CurrentLyric, new UTF8Encoding(true));
                    MessageBox.Show("已保存：\r\n" + dlg.FileName);
                }
            }
        }

        private void Copy2btn_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(m_CurrentLyric, true);
        }

        // 歌词默认保存名："歌手 - 歌名.lrc"
        private static string DefaultLyricName(SongInfo song)
        {
            string name = "lyrics";
            if (song != null && song.Title.Length > 0)
            {
                name = song.Title;
                if (song.Artist.Length > 0)
                    name = song.Artist + " - " + name;
                foreach (char c in Path.GetInvalidFileNameChars())
                    name = name.Replace(c, '_');
            }
            return name + ".lrc";
        }

        // ==================== 批量扫描 ====================

        private volatile bool m_StopScan = false;
        private bool m_Scanning = false;

        private void Browsebtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "选择要扫描的音乐目录";
                dlg.ShowNewFolderButton = false;
                if (Directory.Exists(Dirtb.Text.Trim()))
                    dlg.SelectedPath = Dirtb.Text.Trim();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    Dirtb.Text = dlg.SelectedPath;
            }
        }

        private void Scanbtn_Click(object sender, EventArgs e)
        {
            if (m_Scanning)
                return;

            string sDir = Dirtb.Text.Trim();
            if (sDir.Length == 0 || !Directory.Exists(sDir))
            {
                MessageBox.Show("请选择存在的目录！");
                return;
            }

            // 勾选顺序即优先级：网易 → 酷狗 → LRCLIB
            List<ILyricSource> sources = new List<ILyricSource>();
            if (PrioNetease.Checked)
                sources.Add(new NetEaseSource());
            if (PrioKugou.Checked)
                sources.Add(new KugouSource());
            if (PrioLrclib.Checked)
                sources.Add(new LrclibSource());
            if (sources.Count == 0)
            {
                MessageBox.Show("请至少勾选一个歌词源！");
                return;
            }

            List<string> files = new List<string>();
            try
            {
                foreach (string f in Directory.EnumerateFiles(sDir))
                {
                    string ext = Path.GetExtension(f).ToLower();
                    foreach (string e2 in MusicFileScanner.Extensions)
                    {
                        if (ext == e2)
                        {
                            files.Add(f);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取目录失败！错误：\r\n" + ex.Message);
                return;
            }

            if (files.Count == 0)
            {
                MessageBox.Show("目录中没有支持的音乐文件（mp3/flac/m4a/aac/ogg/opus/wma/wav/ape）。");
                return;
            }

            bool bSkipExisting = SkipExisting.Checked;
            m_StopScan = false;
            m_Scanning = true;
            Scanbtn.Enabled = false;
            Stopbtn.Enabled = true;
            ScanLogtb.Clear();
            ScanProgress.Value = 0;
            ScanProgress.Maximum = files.Count;
            ScanLogtb.AppendText(string.Format("共 {0} 个音乐文件，开始扫描...\r\n", files.Count));

            Task.Run((Action)(() =>
            {
                int done = 0;
                foreach (string sFile in files)
                {
                    if (m_StopScan)
                        break;
                    string sLine = ScanOneFile(sFile, sources, bSkipExisting);
                    done++;
                    BeginInvoke((Action)(() =>
                    {
                        ScanLogtb.AppendText(sLine + "\r\n");
                        ScanProgress.Value = Math.Min(done, ScanProgress.Maximum);
                    }));
                }
                BeginInvoke((Action)(() =>
                {
                    ScanLogtb.AppendText(m_StopScan ? "已停止。" : "扫描完成。");
                    ScanLogtb.AppendText("\r\n");
                    m_Scanning = false;
                    Scanbtn.Enabled = true;
                    Stopbtn.Enabled = false;
                }));
            }));
        }

        /// <summary>
        /// 处理单个音乐文件：读标签 → 按优先级逐个源搜索 → 打分取最优 → 保存同名 lrc。
        /// 返回一行日志。
        /// </summary>
        private string ScanOneFile(string sFile, List<ILyricSource> sources, bool bSkipExisting)
        {
            SongInfo tag = MusicFileScanner.ReadFromFile(sFile);
            string sName = Path.GetFileName(sFile);

            if (tag.Title.Length == 0)
                return "✘ " + sName + "：无法识别歌名，跳过";

            if (bSkipExisting && LrcSaver.Exists(sFile))
                return "－ " + tag.Title + "：已有歌词文件，跳过";

            string sFailReasons = "";
            foreach (ILyricSource source in sources)
            {
                if (m_StopScan)
                    break;

                List<SongInfo> hits;
                try
                {
                    hits = source.SearchDetail(tag.Title, tag.Artist, tag.Album);
                }
                catch (Exception ex)
                {
                    sFailReasons += "[" + source.Name + "搜索失败：" + ex.Message + "] ";
                    continue;
                }

                SongInfo best = null;
                int bestScore = 0;
                foreach (SongInfo hit in hits)
                {
                    int score = MusicFileScanner.Score(tag, hit);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = hit;
                    }
                }

                if (best == null || bestScore < MusicFileScanner.MatchThreshold)
                    continue;

                LyricResult result = source.GetLyric(best);
                if (!result.HasError && result.Lyric.Length > 0)
                {
                    LrcSaver.Save(sFile, result.ToString());
                    return "✔ " + tag.Title + (tag.Artist.Length > 0 ? " - " + tag.Artist : "")
                        + " ← [" + source.Name + "] " + best.Title + " - " + best.Artist
                        + "（匹配" + bestScore + "分）";
                }
                sFailReasons += "[" + source.Name + ":" + result.Error + "] ";
            }

            return "✘ " + tag.Title + (tag.Artist.Length > 0 ? " - " + tag.Artist : "")
                + "：未找到可靠匹配的歌词" + (sFailReasons.Length > 0 ? "；" + sFailReasons : "");
        }

        private void Stopbtn_Click(object sender, EventArgs e)
        {
            m_StopScan = true;
        }

        // ==================== 关于 ====================

        private void lnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("https://github.com/ituff/163lyric");
                psi.UseShellExecute = true;
                Process.Start(psi);
            }
            catch { }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            lblStatus.Text = "正在检查更新...";

            Task.Run((Action)(() =>
            {
                try
                {
                    Updater.ReleaseInfo rel = Updater.GetLatestRelease();
                    if (!Updater.IsNewer(rel.Tag))
                    {
                        SetStatus("已是最新版本（线上 " + rel.Tag + "）");
                        EnableUpdateBtn();
                        return;
                    }

                    SetStatus("发现新版本 " + rel.Tag + "，正在下载...");
                    string sScript = Updater.DownloadAndPrepareUpdate(rel.ZipUrl);
                    SetStatus("更新就绪，应用即将重启...");
                    Thread.Sleep(600);   // 给状态文本留出渲染时间
                    BeginInvoke((Action)(() => Updater.ApplyUpdate(sScript)));
                }
                catch (Exception ex)
                {
                    SetStatus("更新失败：" + Clamp(ex.Message));
                    EnableUpdateBtn();
                }
            }));
        }

        private void SetStatus(string s)
        {
            BeginInvoke((Action)(() => lblStatus.Text = s));
        }

        private void EnableUpdateBtn()
        {
            BeginInvoke((Action)(() => btnUpdate.Enabled = true));
        }

        private static string Clamp(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            s = s.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            return s.Length <= 38 ? s : s.Substring(0, 38) + "...";
        }

        // ==================== 小工具 ====================

        // 按钮进入/退出忙碌状态（文字存回 Tag）
        private void SetBusy(Button btn, bool busy)
        {
            if (busy)
            {
                btn.Tag = btn.Text;
                btn.Text = "请稍候...";
                btn.Enabled = false;
            }
            else
            {
                if (btn.Tag is string)
                    btn.Text = (string)btn.Tag;
                btn.Enabled = true;
            }
        }

        private static string FormatDuration(int seconds)
        {
            if (seconds <= 0)
                return "";
            return string.Format("{0}:{1:D2}", seconds / 60, seconds % 60);
        }
    }
}
