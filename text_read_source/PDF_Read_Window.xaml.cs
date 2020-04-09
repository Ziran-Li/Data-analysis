using System;
using System.Collections.Generic;
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
using MoonPdfLib.MuPdf;
using Microsoft.Win32;
namespace text_read_source
{
    /// <summary>
    /// PDF_Read_Window.xaml 的交互逻辑
    /// </summary>
    public partial class PDF_Read_Window
    {
        private bool _isLoaded = false;
        int language_select_flag = 0;
        public PDF_Read_Window(int language_select)
        {
            language_select_flag = language_select;
            InitializeComponent();
            if(language_select_flag ==0)
            {
                m_zairu.Content = "载入";
                m_fangda.Content = "放大";
                m_suoxiao.Content = "缩小";
                m_zhengye.Content = "整页";
                m_danye.Content = "单页";
                m_shuangye.Content = "双页";
            }
            else
            {
                m_zairu.Content = "Load";
                m_fangda.Content = "Zoom In";
                m_suoxiao.Content = "Zoom Out";
                m_zhengye.Content = "Full Page";
                m_danye.Content = "Single Page";
                m_shuangye.Content = "Two Page";
            }
        }
        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Function_Load();
        }
        string pdf_path_string;
        private void Function_Load()
        {
            try
            {
                if (language_select_flag==0)
                {
                    pdf_path_string = System.IO.Path.GetFullPath(@"dataanalysis.pdf");
                }
                else
                {
                    pdf_path_string = System.IO.Path.GetFullPath(@"2.pdf");
                }

                moonPdfPanel.OpenFile(pdf_path_string);
                _isLoaded = true;
            }
            catch (Exception)
            {
                _isLoaded = false;
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomIn();
            }
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomOut();
            }
        }

        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.Zoom(1.0);
            }
        }

        private void FitToHeightButton_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomToHeight();
        }

        private void FacingButton_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ViewType = MoonPdfLib.ViewType.Facing;
        }

        private void SinglePageButton_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ViewType = MoonPdfLib.ViewType.SinglePage;
        }
    }
}
