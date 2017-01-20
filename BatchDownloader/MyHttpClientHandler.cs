using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignHelper
{
    public class MyHttpClientHandler : HttpClientHandler
    {
        public string FileName="zjls";
        public string Dir = System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
        //public string Dir = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>    
        /// 将Cookie保存到磁盘    
        /// </summary>    
        public void SaveCookiesToDisk()
        {
            string cookieFile = Dir + "\\"+FileName+".cookie";
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Create);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formater.Serialize(fs, CookieContainer);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            
        }

        /// <summary>    
        /// 从磁盘加载Cookie    
        /// </summary>    
        public void LoadCookiesFromDisk()
        {
            string cookieFile = Dir + "\\" + FileName + ".cookie";
            if (!File.Exists(cookieFile))
                return;
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                CookieContainer = (CookieContainer)formater.Deserialize(fs);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }


        public void RemoveCookiesFromDisk()
        {
            string cookieFile = Dir + "\\" + FileName + ".cookie";
            if (!File.Exists(cookieFile))
                return;
            File.Delete(cookieFile);
        }

        public bool IsCookiesInDisd()
        {
            string cookieFile = Dir + "\\" + FileName + ".cookie";
            if (File.Exists(cookieFile))
                return true;
            else
            {
                return false;
            }
        }
    }
}
