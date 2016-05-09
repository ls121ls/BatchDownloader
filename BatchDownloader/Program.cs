﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BatchDownloader
{
    class Program
    {
        //起始页
        static int strartIndex = 74797;
        //终止页
        static int endIndex = 90000;

        static HttpClient httpClient;
        private delegate bool MyDelegate(object i);

        //文件最小值
        static int minLength = 1;
        //线程数
        private static int count = 20;

        static MyDelegate myDelegate = new MyDelegate(DownloadFile);
        //当前下载最大Id
        private static int maxId = 1;
        static void Main(string[] args)
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            maxId = strartIndex - 1;
            for (int i = 0; i < count; i++)
            {
                lock (thisLock2)
                {
                    maxId++;
                    IAsyncResult result1 = myDelegate.BeginInvoke(maxId, Completed, null);
                }
            }
            Console.ReadKey();
        }
        private static Object thisLock = new Object();
        private static Object thisLock1 = new Object();
        private static Object thisLock2 = new Object();
        public static bool DownloadFile(object id)
        {
            string str = "";
            str = "http://www.haxsk.com/modules/article/txtarticle.php?id=" + id;
            var st = httpClient.GetAsync(str);
            try
            {
                Log("正在下载" + id);
                var stemp = st.Result;
                //如果小于800，即没有文件
                if (stemp.Content.Headers.ContentLength < minLength)
                {
                    Log(id + "没有文件1");
                    return true;
                }
                IEnumerable<String> sy;
                if (!stemp.Content.Headers.TryGetValues("content-disposition", out sy))
                {
                    Log(id + "没有文件2");
                    return true;
                }
                string s = sy.FirstOrDefault();
                //将乱码转换为正确的字符串
                s = GetFileName(Encoding.GetEncoding("gbk").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(s)));

                File.WriteAllBytes("F://1/" + s, stemp.Content.ReadAsByteArrayAsync().Result);
                lock (thisLock)
                {
                    File.AppendAllText("F://1/10061.txt", id + "\r\n");
                }
                Log(id + "下载完成");
                return true;
            }
            catch (Exception e)
            {
                Log(id + "下载异常，重新下载");
                myDelegate.BeginInvoke(id, Completed, null);
                return false;
            }
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
                File.AppendAllText("F://1/log.txt", str + "\r\n");
            }
        }


        static string GetFileName(string str)
        {
            return Regex.Replace(str, ".*filename=", "");
        }
    }
}
