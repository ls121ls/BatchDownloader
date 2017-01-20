using System;
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
    class Program2
    {
        //起始页
        static int strartIndex = 1;
        //终止页
        static int endIndex = 50000;

        private delegate bool MyDelegate(object i);

        //文件最小值
        static int minLength = 1;
        //线程数
        private static int count = 1;

        static MyDelegate myDelegate = new MyDelegate(DownloadFile);
        //当前下载最大Id
        private static int maxId = 1;
        static void Main(string[] args)
        {

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
            str = "http://www.11bz.org/modules/article/txtarticle.php?id=" + id;
            try
            {
                Log("正在下载" + id);
                WebRequest request = WebRequest.Create(str);
                var temp=request.GetResponse().Headers;
                if (temp.GetValues("content-disposition")==null)
                {
                    Log(id + "没有文件2");
                    return true;
                }
                string fileName = GetFileName(Encoding.GetEncoding("gbk").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(temp.GetValues("content-disposition").FirstOrDefault())));

                StreamToFile(request.GetResponse().GetResponseStream(), "F://1/" + fileName);
                lock (thisLock)
                {
                    File.AppendAllText("F://1/111下载.txt", id + "\r\n");
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
        /// <summary>   
        /// 将 Stream 写入文件   
        /// </summary>   
        public static void StreamToFile(Stream responseStream, string fileName)
        {
            //创建本地文件写入流
            Stream stream = new FileStream(fileName, FileMode.Create);

            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();

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
                File.AppendAllText("F://1/111日志.txt", str + "\r\n");
            }
        }


        static string GetFileName(string str)
        {
            return Regex.Replace(str, ".*filename=", "");
        }
    }
}
