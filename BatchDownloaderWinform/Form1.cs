using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using BatchDownloader;

namespace BatchDownloaderWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyWebHelper.StartDownload();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyWebHelper.DownloadType = 2;
            MyWebHelper.StartDownload();
        }
    }
}
