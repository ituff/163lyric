using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace _163lyric
{
    class NetEaseLyric
    {
        public string getLyric(int iID) {

            string sLRC="";
            string sContent;

            HttpRequest hr = new HttpRequest();

            sContent = hr.getContent("http://music.163.com/api/song/media?id=" + iID);

            if (sContent.Substring(0, 4).Equals("ERR!")) return "Get lyric failed! \r\n EER: \r\n" + sContent.Substring(4);

            //反序列化JSON数据  
            JObject o = (JObject)JsonConvert.DeserializeObject(sContent);
            sLRC = o["lyric"].ToString().Replace("\"","").Replace("\\n", "\r\n");

            return sLRC;
        }
    }
}
