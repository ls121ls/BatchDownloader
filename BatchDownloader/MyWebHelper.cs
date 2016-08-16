using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;

namespace BatchDownloader
{
    public static class MyWebHelper
    {
        //起始页
        static int strartIndex = 10000;
        //终止页
        static int endIndex = 30000;

        static HttpClient httpClient;
        private delegate bool MyDelegate(object i);

        //文件最小值
        static int minLength = 1;
        //线程数
        private static int count = 1;

        static MyDelegate myDelegate = new MyDelegate(DownloadFile);
        //当前下载最大Id
        private static int maxId = 1;

        private static List<string> strs = new List<string>(); 

        private static Object thisLock = new Object();
        private static Object thisLock1 = new Object();
        private static Object thisLock2 = new Object();

        //下载方式，1是直接下载，2是通过文件下载
        public static int DownloadType = 1;

        public static void StartDownload()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            if (DownloadType == 2)
            {

                strs = File.ReadAllLines("F://1/origion1.txt").ToList();
                endIndex = strs.Count;
            }

            maxId = strartIndex - 1;
            for (int i = 0; i < count; i++)
            {
                lock (thisLock2)
                {
                    maxId++;
                    IAsyncResult result1 = myDelegate.BeginInvoke(maxId, Completed, null);
                }
            }
        }

        /// <summary>
        /// 如果使用文件下载方式，把DownloadType设置为1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DownloadFile(object id)
        {
            string str = GetLinkStr(Convert.ToInt32(id));
            var st = httpClient.GetAsync(str);
            try
            {
                Log("正在下载" + id);
                var stemp = st.Result;
                //如果小于800，即没有文件
                if (stemp.Content.Headers.ContentLength < minLength)
                {
                    Log(id + "文件过小");
                    return true;
                }
                IEnumerable<String> sy;
                if (!stemp.Content.Headers.TryGetValues("content-disposition", out sy))
                {
                    Log(id + "没有文件");
                    return true;
                }
                string s = sy.FirstOrDefault();
                //将乱码转换为正确的字符串
                s = GetFileName(Encoding.GetEncoding("gbk").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(s)));
                File.WriteAllBytes("F://1/" + s, stemp.Content.ReadAsByteArrayAsync().Result);
                lock (thisLock)
                {
                    File.AppendAllText("F://1/[下载记录].txt", id + "\r\n");
                }
                Log(id + "下载完成"+s);
                return true;
            }
            catch (Exception e)
            {
                Log(id + "下载异常，重新下载");
                myDelegate.BeginInvoke(id, Completed, null);
                return false;
            }
        }

        public static string GetLinkStr(int id)
        {
            switch (DownloadType)
            {
                case 1:
                    return "http://www.wodexiaoshuo.com/txt/?txt-8689-" + id + "-14559-do.html";
                    break;
                case 2:
                    return strs[id];
                    break;
            }
            return "";
        }

        static void Completed(IAsyncResult result)
        {//获取委托对象，调用EndInvoke方法获取运行结果
            AsyncResult _result = (AsyncResult)result;
            MyDelegate myDelegaate = (MyDelegate)_result.AsyncDelegate;
            //获得参数
            bool data = myDelegaate.EndInvoke(_result);
            if (data)
            {
                lock (thisLock2)
                {
                    if (maxId <= endIndex)
                    {
                        maxId++;
                        myDelegate.BeginInvoke(maxId, Completed, null);
                    }
                }
            }
        }

        static void Log(string str)
        {
            Console.WriteLine(str);
            lock (thisLock1)
            {
                File.AppendAllText("F://1/[日志].txt", str + "\r\n");
            }
        }


        static string GetFileName(string str)
        {
            return Regex.Replace(str, ".*filename=", "");
        }
    }
}
