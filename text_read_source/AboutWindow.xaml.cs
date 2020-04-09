using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace text_read_source
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(int language_select)
        {
            InitializeComponent();
            if(language_select==0)
            {
                m_gongsi.Content = "南京傲翼飞控智能科技有限公司";
                XMonitorVer.Content = "1. 数据分析软件 ";
                APFCVer_L.Content = "2. 版本1.0.0 ";
                m_close.Content = "关闭";
            }
            else
            {
                m_gongsi.Content = "Auto-wing Flight Control Tech";
                XMonitorVer.Content = "1. Data analysis software ";
                APFCVer_L.Content = "2. Version1.1.0 ";
                m_close.Content = "Close";
            }

            StreamReader sr = new StreamReader(System.IO.Path.GetFullPath(@"License.txt"), Encoding.GetEncoding("Shift_JIS"));
            AboutText.Text = sr.ReadToEnd();
            sr.Close();
        }
        private void ButAboutEnd(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ButAboutEnd2(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void AboutMiniSuvURL(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.aydrone.com/");
        }
        private void AboutACSLURL(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.aydrone.com/");
        }

        public string XMonitorVersion
        {
            get { return (string)XMonitorVer.Content; }
            set
            {
                string _ver = value;
                XMonitorVer.Content = "1. X-Monitor Ver " + _ver;
            }
        }
        public string APFCVersion_L
        {
            get { return (string)APFCVer_L.Content; }
            set
            {
                string _ver = value;
                APFCVer_L.Content = "2. APFC(L) Ver " + _ver;
            }
        }
        public string APFCVersion_H
        {
            get { return (string)APFCVer_H.Content; }
            set
            {
                string _ver = value;
                APFCVer_H.Content = "3. APFC(H) Ver " + _ver;
            }
        }
        private void Updata_Click(object sender, RoutedEventArgs e)
        {
            //MessageWindow Mwindow = new MessageWindow();
            //Mwindow.flag = true;
            //if (Mwindow.Initialize())
            //{
            //    Mwindow.ShowDialog();
            //}
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

    }
}
