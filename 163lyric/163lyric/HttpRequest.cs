using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace _163lyric
{
    class HttpRequest
    {
        /// <summary>
        /// GET 请求并读取全部文本。headers 可附加请求头（User-Agent、Referer 等按需覆盖）。
        /// 出错时返回以 "ERR!" 开头的字符串，调用方按此约定判断。
        /// </summary>
        public string getContent(string sURL, Dictionary<string, string> headers = null) {

            string sContent; //Content

            try
            {
                // 宿主不一定执行 Program.Main（如反射调用），这里也补一次 TLS 1.2
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                HttpWebRequest wrGETURL = (HttpWebRequest)WebRequest.Create(sURL);
                wrGETURL.Timeout = 10000;
                wrGETURL.ReadWriteTimeout = 10000;
                // LRCLIB 要求客户端表明身份，统一带上 User-Agent
                wrGETURL.UserAgent = "OneLyric/1.2 (https://github.com/ituff/163lyric)";
                wrGETURL.Accept = "*/*";

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> kv in headers)
                    {
                        if (string.Equals(kv.Key, "User-Agent", StringComparison.OrdinalIgnoreCase))
                            wrGETURL.UserAgent = kv.Value;
                        else if (string.Equals(kv.Key, "Referer", StringComparison.OrdinalIgnoreCase))
                            wrGETURL.Referer = kv.Value;
                        else
                            wrGETURL.Headers.Add(kv.Key, kv.Value);
                    }
                }

                using (WebResponse wrResponse = wrGETURL.GetResponse())
                using (Stream objStream = wrResponse.GetResponseStream())
                using (StreamReader objReader = new StreamReader(objStream, Encoding.UTF8))
                {
                    sContent = objReader.ReadToEnd();
                }
            }

            catch (Exception e) {
                sContent = "ERR!"+e.ToString();
            }

            return sContent;
        }
    }
}
