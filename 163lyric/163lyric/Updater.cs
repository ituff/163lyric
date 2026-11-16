using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _163lyric
{
    /// <summary>
    /// 一键在线更新：访问 GitHub Releases API 检查最新版本，
    /// 下载 zip 资产并解压，再交给一个等待本进程退出的 cmd 脚本完成替换与重启。
    /// </summary>
    static class Updater
    {
        private const string RepoApiUrl = "https://api.github.com/repos/ituff/163lyric/releases/latest";
        private const string RepoHomePage = "https://github.com/ituff/163lyric";

        public static string CurrentVersion
        {
            get { return typeof(Updater).Assembly.GetName().Version.ToString(3); }   // "1.2.0"
        }

        public class ReleaseInfo
        {
            public string Tag = "";       // 如 "v1.2"
            public string ZipUrl = "";    // zip 资产的下载地址
        }

        /// <summary>
        /// 查询最新 Release。失败抛异常（网络/限流/JSON 异常），由调用方展示。
        /// </summary>
        public static ReleaseInfo GetLatestRelease()
        {
            // Updater 自带请求逻辑（不依赖 HttpRequest 的字符串返回），同样要补 TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(RepoApiUrl);
            req.UserAgent = "OneLyric/" + CurrentVersion + " (" + RepoHomePage + ")";   // GitHub API 强制要求 UA
            req.Accept = "application/vnd.github+json";
            req.Timeout = 15000;

            string sContent;
            using (WebResponse resp = req.GetResponse())
            using (Stream s = resp.GetResponseStream())
            using (StreamReader r = new StreamReader(s, Encoding.UTF8))
            {
                sContent = r.ReadToEnd();
            }

            JObject o = (JObject)JsonConvert.DeserializeObject(sContent);
            ReleaseInfo info = new ReleaseInfo();
            info.Tag = (o["tag_name"] ?? "").ToString().Trim();

            JArray assets = o["assets"] as JArray;
            if (assets != null)
            {
                foreach (JToken a in assets)
                {
                    string name = (a["name"] ?? "").ToString();
                    if (name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        info.ZipUrl = (a["browser_download_url"] ?? "").ToString();
                        break;
                    }
                }
            }

            if (info.Tag.Length == 0)
                throw new Exception("接口未返回版本号。");
            if (info.ZipUrl.Length == 0)
                throw new Exception("Release 中没有 zip 安装包。");
            return info;
        }

        /// <summary>tag（如 "v1.2" 或 "1.2.0"）是否比当前程序版本新。</summary>
        public static bool IsNewer(string sTag)
        {
            string s = sTag.Trim().TrimStart('v', 'V');
            Version vTag, vCur = typeof(Updater).Assembly.GetName().Version;
            if (!Version.TryParse(s, out vTag))
                return false;
            return vTag > vCur;
        }

        /// <summary>
        /// 下载 zip、解压、生成替换脚本，返回脚本路径。
        /// 脚本会等待当前进程退出后覆盖程序目录并重启应用。
        /// </summary>
        public static string DownloadAndPrepareUpdate(string sZipUrl)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // 下载内容放独立的新临时目录
            string sWorkDir = Path.Combine(Path.GetTempPath(),
                "OneLyric-update-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(sWorkDir);
            string sZipPath = Path.Combine(sWorkDir, "update.zip");

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sZipUrl);
            req.UserAgent = "OneLyric/" + CurrentVersion + " (" + RepoHomePage + ")";
            req.Timeout = 30000;
            req.ReadWriteTimeout = 60000;

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            using (Stream net = resp.GetResponseStream())
            using (FileStream fs = new FileStream(sZipPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buf = new byte[81920];
                int n;
                while ((n = net.Read(buf, 0, buf.Length)) > 0)
                    fs.Write(buf, 0, n);
            }

            // 解压
            string sNewDir = Path.Combine(sWorkDir, "new");
            ZipFile.ExtractToDirectory(sZipPath, sNewDir);
            // 兼容 zip 里带一层子目录的情况：只含一个目录时把内容上提一层
            string[] subDirs = Directory.GetDirectories(sNewDir);
            if (subDirs.Length == 1 && Directory.GetFiles(sNewDir).Length == 0)
            {
                string sSub = subDirs[0];
                foreach (string f in Directory.GetFiles(sSub))
                    File.Move(f, Path.Combine(sNewDir, Path.GetFileName(f)));
                foreach (string d in Directory.GetDirectories(sSub))
                    Directory.Move(d, Path.Combine(sNewDir, Path.GetFileName(d)));
                Directory.Delete(sSub);
            }

            // 生成替换脚本：等当前进程退出 → 覆盖程序目录 → 重启 → 清理
            string sExePath = Application.ExecutablePath;
            string sAppDir = Path.GetDirectoryName(sExePath);
            string sScript = Path.Combine(sWorkDir, "update.cmd");
            string sScriptBody =
                "@echo off\r\n" +
                ":waitloop\r\n" +
                "tasklist /FI \"PID eq " + Process.GetCurrentProcess().Id + "\" 2>nul | findstr /I \"OneLyric\" >nul\r\n" +
                "if not errorlevel 1 (\r\n" +
                "  timeout /t 1 /nobreak >nul\r\n" +
                "  goto waitloop\r\n" +
                ")\r\n" +
                // 清理更名前的旧版主程序
                "if exist \"" + Path.Combine(sAppDir, "163lyric.exe") + "\" del \"" + Path.Combine(sAppDir, "163lyric.exe") + "\"\r\n" +
                "xcopy /Y /E /I \"" + sNewDir + "\" \"" + sAppDir + "\" >nul\r\n" +
                "start \"\" \"" + sExePath + "\"\r\n" +
                "rd /s /q \"" + sWorkDir + "\"\r\n" +
                "del \"%~f0\"\r\n";
            File.WriteAllText(sScript, sScriptBody, Encoding.Default);
            return sScript;
        }

        /// <summary>启动替换脚本并退出当前应用（脚本会完成覆盖与重启）。</summary>
        public static void ApplyUpdate(string sScriptPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c \"" + sScriptPath + "\"");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
            Application.Exit();
        }
    }
}
