using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BatchDownloader
{
    public static class MyHelper
    {
        /// <summary>
        /// 去除爬虫出来的错误链接
        /// </summary>
        public static void RemoveErrorLink()
        {
            string[] strs = File.ReadAllLines("F://1/1.txt");
            List<string> stemps = new List<string>();
            foreach (var str in strs)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    string temp = str.Split('-')[3];
                    if (!String.IsNullOrEmpty(temp))
                    {
                        stemps.Add(str);
                    }
                }
            }
            string path = "F://1/2.txt";
            File.WriteAllLines(path, stemps);
            Console.ReadKey();
        }

        public static int CalcCount()
        {
            return File.ReadAllLines("F://1/2.txt").Count();
        }
    }
}
