using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace _163lyric
{
    class HttpRequest
    {
        public string getContent(string sURL) {

            string sContent; //Content

            try
            {
                WebRequest wrGETURL = WebRequest.Create(sURL);
                using (Stream objStream = wrGETURL.GetResponse().GetResponseStream())
                using (StreamReader objReader = new StreamReader(objStream))
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
