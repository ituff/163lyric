using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _163lyric
{
    /// <summary>
    /// 关于窗口：版本信息、GitHub 项目主页、作者公众号（ONE生产力）二维码、一键检查更新。
    /// </summary>
    class AboutForm : Form
    {
        private PictureBox pbLogo;
        private Label lblName;
        private Label lblDesc;
        private LinkLabel lnkGithub;
        private Label lblGzh;
        private PictureBox pbQr;
        private Button btnUpdate;
        private Label lblStatus;
        private Button btnClose;

        public AboutForm()
        {
            InitUI();
        }

        private void InitUI()
        {
            Text = "关于 OneLyric";
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(430, 440);

            // 应用图标（与 exe 一致）
            pbLogo = new PictureBox();
            pbLogo.Location = new Point(24, 20);
            pbLogo.Size = new Size(32, 32);
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                pbLogo.Image = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();
            }
            catch { }
            Controls.Add(pbLogo);

            lblName = new Label();
            lblName.AutoSize = true;
            lblName.Font = new Font("宋体", 14F, FontStyle.Bold);
            lblName.Location = new Point(66, 24);
            lblName.Text = "OneLyric v" + Updater.CurrentVersion;
            Controls.Add(lblName);

            lblDesc = new Label();
            lblDesc.AutoSize = true;
            lblDesc.ForeColor = Color.DimGray;
            lblDesc.Location = new Point(68, 56);
            lblDesc.Text = "网易云音乐歌词下载器";
            Controls.Add(lblDesc);

            lnkGithub = new LinkLabel();
            lnkGithub.AutoSize = true;
            lnkGithub.Location = new Point(24, 92);
            lnkGithub.Text = "GitHub：https://github.com/ituff/163lyric";
            lnkGithub.LinkArea = new LinkArea(7, lnkGithub.Text.Length - 7);
            lnkGithub.LinkClicked += new LinkLabelLinkClickedEventHandler(LnkGithub_Click);
            Controls.Add(lnkGithub);

            lblGzh = new Label();
            lblGzh.AutoSize = true;
            lblGzh.Location = new Point(24, 124);
            lblGzh.Text = "微信公众号：ONE生产力（扫码关注）";
            Controls.Add(lblGzh);

            // 公众号二维码（嵌入 exe 的资源，来自 ShareOneList 项目）
            pbQr = new PictureBox();
            pbQr.Location = new Point(115, 152);
            pbQr.Size = new Size(200, 200);
            pbQr.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                using (Stream rs = typeof(AboutForm).Assembly.GetManifestResourceStream("_163lyric.wechat-qrcode.png"))
                {
                    if (rs != null)
                    {
                        // 复制进内存流，避免依赖已释放的资源流
                        MemoryStream ms = new MemoryStream();
                        rs.CopyTo(ms);
                        ms.Position = 0;
                        pbQr.Image = Image.FromStream(ms);
                    }
                }
            }
            catch { }
            Controls.Add(pbQr);

            btnUpdate = new Button();
            btnUpdate.Location = new Point(24, 372);
            btnUpdate.Size = new Size(110, 28);
            btnUpdate.Text = "检查更新";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            Controls.Add(btnUpdate);

            lblStatus = new Label();
            lblStatus.Location = new Point(144, 378);
            lblStatus.Size = new Size(280, 20);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.ForeColor = Color.DimGray;
            Controls.Add(lblStatus);

            btnClose = new Button();
            btnClose.Location = new Point(340, 402);
            btnClose.Size = new Size(66, 26);
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += new EventHandler(delegate { Close(); });
            Controls.Add(btnClose);

            AcceptButton = btnClose;
            CancelButton = btnClose;
        }

        private void LnkGithub_Click(object sender, LinkLabelLinkClickedEventArgs e)
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
    }
}
