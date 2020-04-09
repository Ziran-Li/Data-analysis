using GMap.NET;                         //GMap.NET Core
using GMap.NET.MapProviders;            //GMap.NET 地图提供商
using GMap.NET.WindowsPresentation;
using wpf_test_gamp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Steema.TeeChart.WPF;
using System.Threading;
using Steema.TeeChart.WPF.Styles;
using System.Data;
using Steema.TeeChart.WPF.Tools;

namespace text_read_source
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int Flight_Style_Num = 0;//机型选择编码1开始的
        int YKQType_Num = 0;//遥控器类型0开始的
        int Battery_Num = 0;//电池类型0开始的
        int IfReback_Num = 0;//是否回中0开始的
        int Imu_Check_Acc = 0;
        int Imu_Check_Gyro = 0;
        int Imu_Check_Mag = 0;
        DataTable newdtb = new DataTable();
        int start_play_flag = 0;
        double m_dMeterPerDeg_Latitude = 111569.58;//每纬度对应的距离
        double m_dMeterPerDeg_Longitude = 90717.31;//每经度对应的距离
        int track_count = 0;
        int track_play_count = 0;
        int maxtomin = 0;
        bool If_Load_Data_Success = false;

        int whitch_line = 0;//各个线说明如下
        // 0 m_roll; 1 m_pitch; 2 m_yaw; 3 m_telecontroller_roll;4 m_telecontroller_pitch; 5 m_telecontroller_yaw; 6 m_telecontroller_throttle 7 m_gyro_roll 8 m_gyro_pitch; 9 m_gyro_yaw;
        //10 m_accelerate_x; 11 m_accelerate_y;12 m_accelerate_z; 13 m_mag_x; 14 m_mag_y 15 m_mag_z; 16 m_telecontroller_A; 17 m_telecontroller_B; 18 m_telecontroller_C;19 m_telecontroller_D; 
        //20 m_motor_1; 21 m_motor_2; 22 m_motor_3; 23 m_motor_4; 24 m_motor_5; 25 m_motor_6;26 m_gps_northvel; 27 m_gps_eastvel; 28 m_gps_updownvel 29 m_gps_lon; 
        // 30 m_gps_lat; 31 m_gps_height; 32 m_gps_yaw; 33 m_gps_starnum;34 m_gps_dop; 35 m_barometer_height; 36 m_state_volt 37 m_state_8highbyte; 38 m_state_8lowbyte; 39 m_state_IMUstate 
        // 40 m_state_posnum; 41 m_state_RTK; 42 m_frametype_index; 43 m_group;44 m_size; 45 m_num; 46 m_attitude_control_roll 47 m_stabil_roll; 48 m_origin_pitch; 49 m_stabil_pitch 
        // 50 m_yaw_control_yaw; 51 m_yaw_control_stabyaw; 52 m_yaw_control_fbyaw; 53 m_position_control_lon;54 m_position_control_lat;
        //55 m_position_control_index; 56 m_position_control_northvel 57 m_position_control_eastvel; 58 m_height_control_height; 59 m_height_control_updownvel 
        // 60 m_height_control_fbvel; 61 m_height_control_stabthrottle; 62 m_motor_control_roll; 63 m_motor_control_pitch;64 m_motor_control_yaw; 65 m_state_noise; 66 m_version_info_equipment 67 m_trackreplay; 
        /*=====================================版本信息申明空间=====================================*/
        Version_Information All_Equipment_Info = new Version_Information();
        //////////////////////////////等待效果创建/////////////////////////////
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch WaitTime = new System.Diagnostics.Stopwatch();
        WaitProgressWindow Wait_Dlg;
        int wait_count = 0;
        int wait_flag = 0;
        int error_message = 0;
        RowAndClumn GetData1_RowAndColumn_Value = new RowAndClumn();
        RowAndClumn GetData2_RowAndColumn_Value = new RowAndClumn();
        RowAndClumn GetData3_RowAndColumn_Value = new RowAndClumn();
        RowAndClumn GetData4_RowAndColumn_Value = new RowAndClumn();
        Model_Flight_Time Get_AllModel_Timer = new Model_Flight_Time();
        double[,] AutoWing_UsedData1_Array;
        double[,] AutoWing_UsedData2_Array;
        double[,] AutoWing_UsedData3_Array;
        double[,] AutoWing_UsedData4_Array;
        double[] AutoWing_UsedTracklat_Array;
        double[] AutoWing_UsedTracklng_Array;

        double[] AutoWing_UsedTrackRoll_Array;
        double[] AutoWing_UsedTrackPitch_Array;
        double[] AutoWing_UsedTrackYaw_Array;

        double data1_timer_frequency = 0.04;
        double data2_timer_frequency = 0.0666666667;
        int Whitch_DataBag_Style = 0;
        //     double data3_timer_frequency = 0.01;

        GMapMarker UAVMarker;                           // 当前无人机标志
        PointLatLng pList_points = new PointLatLng();
        GMapRoute UAVRoute;                             // UAV轨迹
        CursorTool cursortool;// = new CursorTool();
       
        public MainWindow()
        {
            InitializeComponent();
            newdtb.Columns.Add("Id", typeof(int));
            newdtb.Columns.Add("ProName", typeof(string));
            newdtb.Columns.Add("name_y", typeof(decimal));
            newdtb.Columns.Add("name_x", typeof(string));
            newdtb.Columns["Id"].AutoIncrement = true;

            m_language_choose.Items.Add("中文简体");
            m_language_choose.Items.Add("English");
            m_language_choose.SelectedIndex = 0;
            sw.Start();
            //Work_Thread();
            TChart1.Aspect.View3D = false;//控件3D效果
            TChart1.Header.Text = "Roll";//Tchart窗体标题
            TChart1.Axes.Bottom.Title.Text = "Timer(s)";//底部标题
            TChart1.Legend.Visible = false;//直线标题集合是否显示

            

            Steema.TeeChart.WPF.Styles.Line line000 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line000.Color = System.Windows.Media.Color.FromRgb(255, 255, 25);//直线颜色
            TChart1.Series.Add(line000);//添加直线

            Steema.TeeChart.WPF.Styles.Line line001 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line001.Color = System.Windows.Media.Color.FromRgb(255, 25, 255);//直线颜色
            TChart1.Series.Add(line001);//添加直线
            Steema.TeeChart.WPF.Styles.Line line002 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line002.Color = System.Windows.Media.Color.FromRgb(25, 255, 255);//直线颜色
            TChart1.Series.Add(line002);//添加直线
            Steema.TeeChart.WPF.Styles.Line line003 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line003.Color = System.Windows.Media.Color.FromRgb(255, 200, 50);//直线颜色
            TChart1.Series.Add(line003);//添加直线
            Steema.TeeChart.WPF.Styles.Line line004 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line004.Color = System.Windows.Media.Color.FromRgb(255,50, 200);//直线颜色
            TChart1.Series.Add(line004);//添加直线
            Steema.TeeChart.WPF.Styles.Line line005 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line005.Color = System.Windows.Media.Color.FromRgb(50, 255, 200);//直线颜色
            TChart1.Series.Add(line005);//添加直线
            Steema.TeeChart.WPF.Styles.Line line006 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line006.Color = System.Windows.Media.Color.FromRgb(20, 105, 05);//直线颜色
            TChart1.Series.Add(line006);//添加直线
            Steema.TeeChart.WPF.Styles.Line line007 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line007.Color = System.Windows.Media.Color.FromRgb(25, 5, 105);//直线颜色
            TChart1.Series.Add(line007);//添加直线
            Steema.TeeChart.WPF.Styles.Line line008 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line008.Color = System.Windows.Media.Color.FromRgb(25, 105, 25);//直线颜色
            TChart1.Series.Add(line008);//添加直线
            Steema.TeeChart.WPF.Styles.Line line009 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line009.Color = System.Windows.Media.Color.FromRgb(105, 25, 25);//直线颜色
            TChart1.Series.Add(line009);//添加直线
            Steema.TeeChart.WPF.Styles.Line line010 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line010.Color = System.Windows.Media.Color.FromRgb(25, 25, 105);//直线颜色
            TChart1.Series.Add(line010);//添加直线
            Steema.TeeChart.WPF.Styles.Line line011 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line011.Color = System.Windows.Media.Color.FromRgb(255, 105, 25);//直线颜色
            TChart1.Series.Add(line011);//添加直线
            Steema.TeeChart.WPF.Styles.Line line012 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line012.Color = System.Windows.Media.Color.FromRgb(105, 255, 25);//直线颜色
            TChart1.Series.Add(line012);//添加直线
            Steema.TeeChart.WPF.Styles.Line line013 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line013.Color = System.Windows.Media.Color.FromRgb(105, 25,255);//直线颜色
            TChart1.Series.Add(line013);//添加直线
            Steema.TeeChart.WPF.Styles.Line line014 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line014.Color = System.Windows.Media.Color.FromRgb(75, 225, 25);//直线颜色
            TChart1.Series.Add(line014);//添加直线
            Steema.TeeChart.WPF.Styles.Line line015 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line015.Color = System.Windows.Media.Color.FromRgb(225, 75, 25);//直线颜色
            TChart1.Series.Add(line015);//添加直线
            Steema.TeeChart.WPF.Styles.Line line016 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line016.Color = System.Windows.Media.Color.FromRgb(75, 25, 225);//直线颜色
            TChart1.Series.Add(line016);//添加直线
            Steema.TeeChart.WPF.Styles.Line line017 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line017.Color = System.Windows.Media.Color.FromRgb(225, 25, 75);//直线颜色
            TChart1.Series.Add(line017);//添加直线
            Steema.TeeChart.WPF.Styles.Line line018 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line018.Color = System.Windows.Media.Color.FromRgb(90, 125, 45);//直线颜色
            TChart1.Series.Add(line018);//添加直线
            Steema.TeeChart.WPF.Styles.Line line019 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line019.Color = System.Windows.Media.Color.FromRgb(90, 45, 125);//直线颜色
            TChart1.Series.Add(line019);//添加直线
            Steema.TeeChart.WPF.Styles.Line line020 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line020.Color = System.Windows.Media.Color.FromRgb(45, 90, 125);//直线颜色
            TChart1.Series.Add(line020);//添加直线
            Steema.TeeChart.WPF.Styles.Line line021 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line021.Color = System.Windows.Media.Color.FromRgb(45, 125, 90);//直线颜色
            TChart1.Series.Add(line021);//添加直线
            Steema.TeeChart.WPF.Styles.Line line022 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line022.Color = System.Windows.Media.Color.FromRgb(125, 45, 90);//直线颜色
            TChart1.Series.Add(line022);//添加直线
            Steema.TeeChart.WPF.Styles.Line line023 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line023.Color = System.Windows.Media.Color.FromRgb(125, 90, 45);//直线颜色
            TChart1.Series.Add(line023);//添加直线
            Steema.TeeChart.WPF.Styles.Line line024 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line024.Color = System.Windows.Media.Color.FromRgb(150, 180, 125);//直线颜色
            TChart1.Series.Add(line024);//添加直线
            Steema.TeeChart.WPF.Styles.Line line025 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line025.Color = System.Windows.Media.Color.FromRgb(150, 125, 180);//直线颜色
            TChart1.Series.Add(line025);//添加直线
            Steema.TeeChart.WPF.Styles.Line line026 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line026.Color = System.Windows.Media.Color.FromRgb(125, 150, 180);//直线颜色
            TChart1.Series.Add(line026);//添加直线
            Steema.TeeChart.WPF.Styles.Line line027 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line027.Color = System.Windows.Media.Color.FromRgb(125, 180, 150);//直线颜色
            TChart1.Series.Add(line027);//添加直线
            Steema.TeeChart.WPF.Styles.Line line028 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line028.Color = System.Windows.Media.Color.FromRgb(180, 125, 150);//直线颜色
            TChart1.Series.Add(line028);//添加直线
            Steema.TeeChart.WPF.Styles.Line line029 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line029.Color = System.Windows.Media.Color.FromRgb(180, 150, 125);//直线颜色
            TChart1.Series.Add(line029);//添加直线
            Steema.TeeChart.WPF.Styles.Line line030 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line030.Color = System.Windows.Media.Color.FromRgb(220, 120, 20);//直线颜色
            TChart1.Series.Add(line030);//添加直线
            Steema.TeeChart.WPF.Styles.Line line031 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line031.Color = System.Windows.Media.Color.FromRgb(220, 20, 120);//直线颜色
            TChart1.Series.Add(line031);//添加直线
            Steema.TeeChart.WPF.Styles.Line line032 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line032.Color = System.Windows.Media.Color.FromRgb(20, 225, 25);//直线颜色
            TChart1.Series.Add(line032);//添加直线
            Steema.TeeChart.WPF.Styles.Line line033 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line033.Color = System.Windows.Media.Color.FromRgb(20, 25, 225);//直线颜色
            TChart1.Series.Add(line033);//添加直线
            Steema.TeeChart.WPF.Styles.Line line034 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line034.Color = System.Windows.Media.Color.FromRgb(25, 225, 20);//直线颜色
            TChart1.Series.Add(line034);//添加直线
            Steema.TeeChart.WPF.Styles.Line line035 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line035.Color = System.Windows.Media.Color.FromRgb(25, 20, 225);//直线颜色
            TChart1.Series.Add(line035);//添加直线
            Steema.TeeChart.WPF.Styles.Line line036 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line036.Color = System.Windows.Media.Color.FromRgb(225, 20, 25);//直线颜色
            TChart1.Series.Add(line036);//添加直线
            Steema.TeeChart.WPF.Styles.Line line037 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line037.Color = System.Windows.Media.Color.FromRgb(225, 25, 20);//直线颜色
            TChart1.Series.Add(line037);//添加直线
            Steema.TeeChart.WPF.Styles.Line line038 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line038.Color = System.Windows.Media.Color.FromRgb(25, 225, 25);//直线颜色
            TChart1.Series.Add(line038);//添加直线
            Steema.TeeChart.WPF.Styles.Line line039 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line039.Color = System.Windows.Media.Color.FromRgb(99, 88, 66);//直线颜色
            TChart1.Series.Add(line039);//添加直线
            Steema.TeeChart.WPF.Styles.Line line040 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line040.Color = System.Windows.Media.Color.FromRgb(99, 66, 88);//直线颜色
            TChart1.Series.Add(line040);//添加直线
            Steema.TeeChart.WPF.Styles.Line line041 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line041.Color = System.Windows.Media.Color.FromRgb(88, 66, 99);//直线颜色
            TChart1.Series.Add(line041);//添加直线
            Steema.TeeChart.WPF.Styles.Line line042 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line042.Color = System.Windows.Media.Color.FromRgb(88, 99, 66);//直线颜色
            TChart1.Series.Add(line042);//添加直线
            Steema.TeeChart.WPF.Styles.Line line043 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line043.Color = System.Windows.Media.Color.FromRgb(66, 88, 99);//直线颜色
            TChart1.Series.Add(line043);//添加直线
            Steema.TeeChart.WPF.Styles.Line line044 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line044.Color = System.Windows.Media.Color.FromRgb(66, 99, 88);//直线颜色
            TChart1.Series.Add(line044);//添加直线
            Steema.TeeChart.WPF.Styles.Line line045 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line045.Color = System.Windows.Media.Color.FromRgb(66, 123, 234);//直线颜色
            TChart1.Series.Add(line045);//添加直线
            Steema.TeeChart.WPF.Styles.Line line046 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line046.Color = System.Windows.Media.Color.FromRgb(60, 234, 123);//直线颜色
            TChart1.Series.Add(line046);//添加直线
            Steema.TeeChart.WPF.Styles.Line line047 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line047.Color = System.Windows.Media.Color.FromRgb(234, 123, 60);//直线颜色
            TChart1.Series.Add(line047);//添加直线
            Steema.TeeChart.WPF.Styles.Line line048 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line048.Color = System.Windows.Media.Color.FromRgb(234, 60, 123);//直线颜色
            TChart1.Series.Add(line048);//添加直线
            Steema.TeeChart.WPF.Styles.Line line049 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line049.Color = System.Windows.Media.Color.FromRgb(234, 60, 123);//直线颜色
            TChart1.Series.Add(line049);//添加直线
            Steema.TeeChart.WPF.Styles.Line line050 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line050.Color = System.Windows.Media.Color.FromRgb(123, 234, 123);//直线颜色
            TChart1.Series.Add(line050);//添加直线
            Steema.TeeChart.WPF.Styles.Line line051 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line051.Color = System.Windows.Media.Color.FromRgb(123, 60, 234);//直线颜色
            TChart1.Series.Add(line051);//添加直线
            Steema.TeeChart.WPF.Styles.Line line052 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line052.Color = System.Windows.Media.Color.FromRgb(80, 140, 180);//直线颜色
            TChart1.Series.Add(line052);//添加直线
            Steema.TeeChart.WPF.Styles.Line line053 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line053.Color = System.Windows.Media.Color.FromRgb(80, 180, 140);//直线颜色
            TChart1.Series.Add(line053);//添加直线
            Steema.TeeChart.WPF.Styles.Line line054 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line054.Color = System.Windows.Media.Color.FromRgb(180, 80, 140);//直线颜色
            TChart1.Series.Add(line054);//添加直线
            Steema.TeeChart.WPF.Styles.Line line055 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line055.Color = System.Windows.Media.Color.FromRgb(180, 140, 80);//直线颜色
            TChart1.Series.Add(line055);//添加直线
            Steema.TeeChart.WPF.Styles.Line line056 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line056.Color = System.Windows.Media.Color.FromRgb(140, 80, 180);//直线颜色
            TChart1.Series.Add(line056);//添加直线
            //Steema.TeeChart.WPF.Styles.Line line057 = new Steema.TeeChart.WPF.Styles.Line();//直线
            //line057.Color = System.Windows.Media.Color.FromRgb(140, 180, 80);//直线颜色
            //TChart1.Series.Add(line057);//添加直线
            //Steema.TeeChart.WPF.Styles.Line line058 = new Steema.TeeChart.WPF.Styles.Line();//直线
            //line058.Color = System.Windows.Media.Color.FromRgb(128, 0, 180);//直线颜色
            //TChart1.Series.Add(line058);//添加直线
            //Steema.TeeChart.WPF.Styles.Line line059 = new Steema.TeeChart.WPF.Styles.Line();//直线
            //line059.Color = System.Windows.Media.Color.FromRgb(180, 25, 128);//直线颜色
            //TChart1.Series.Add(line059);//添加直线
            /******************************nature************************/
            Steema.TeeChart.WPF.Styles.Line line057 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line057.Color = System.Windows.Media.Color.FromRgb(140, 80, 30);//直线颜色
            TChart1.Series.Add(line057);//添加直线
            Steema.TeeChart.WPF.Styles.Line line058 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line058.Color = System.Windows.Media.Color.FromRgb(140,30, 180);//直线颜色
            TChart1.Series.Add(line058);//添加直线
            Steema.TeeChart.WPF.Styles.Line line059 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line059.Color = System.Windows.Media.Color.FromRgb(30, 80, 180);//直线颜色
            TChart1.Series.Add(line059);//添加直线
            Steema.TeeChart.WPF.Styles.Line line060 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line060.Color = System.Windows.Media.Color.FromRgb(140, 50, 180);//直线颜色
            TChart1.Series.Add(line060);//添加直线
            Steema.TeeChart.WPF.Styles.Line line061 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line061.Color = System.Windows.Media.Color.FromRgb(140, 80, 50);//直线颜色
            TChart1.Series.Add(line061);//添加直线
            Steema.TeeChart.WPF.Styles.Line line062 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line062.Color = System.Windows.Media.Color.FromRgb(50, 80, 180);//直线颜色
            TChart1.Series.Add(line062);//添加直线
            Steema.TeeChart.WPF.Styles.Line line063 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line063.Color = System.Windows.Media.Color.FromRgb(140, 80, 70);//直线颜色
            TChart1.Series.Add(line063);//添加直线
            Steema.TeeChart.WPF.Styles.Line line064 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line064.Color = System.Windows.Media.Color.FromRgb(140, 70, 180);//直线颜色
            TChart1.Series.Add(line064);//添加直线
            Steema.TeeChart.WPF.Styles.Line line065 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line065.Color = System.Windows.Media.Color.FromRgb(70, 80, 180);//直线颜色
            TChart1.Series.Add(line065);//添加直线
            Steema.TeeChart.WPF.Styles.Line line066 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line066.Color = System.Windows.Media.Color.FromRgb(140, 80, 90);//直线颜色
            TChart1.Series.Add(line066);//添加直线
            Steema.TeeChart.WPF.Styles.Line line067 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line067.Color = System.Windows.Media.Color.FromRgb(140, 90, 180);//直线颜色
            TChart1.Series.Add(line067);//添加直线
            Steema.TeeChart.WPF.Styles.Line line068 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line068.Color = System.Windows.Media.Color.FromRgb(90, 80, 180);//直线颜色
            TChart1.Series.Add(line068);//添加直线
            Steema.TeeChart.WPF.Styles.Line line069 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line069.Color = System.Windows.Media.Color.FromRgb(140, 80, 110);//直线颜色
            TChart1.Series.Add(line069);//添加直线
            Steema.TeeChart.WPF.Styles.Line line070 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line070.Color = System.Windows.Media.Color.FromRgb(140, 110, 180);//直线颜色
            TChart1.Series.Add(line070);//添加直线
            Steema.TeeChart.WPF.Styles.Line line071 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line071.Color = System.Windows.Media.Color.FromRgb(150, 80, 180);//直线颜色
            TChart1.Series.Add(line071);//添加直线
            Steema.TeeChart.WPF.Styles.Line line072 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line072.Color = System.Windows.Media.Color.FromRgb(140, 150, 180);//直线颜色
            TChart1.Series.Add(line072);//添加直线
            Steema.TeeChart.WPF.Styles.Line line073 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line073.Color = System.Windows.Media.Color.FromRgb(140, 80, 150);//直线颜色
            TChart1.Series.Add(line073);//添加直线
            Steema.TeeChart.WPF.Styles.Line line074 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line074.Color = System.Windows.Media.Color.FromRgb(160, 80, 180);//直线颜色
            TChart1.Series.Add(line074);//添加直线
            Steema.TeeChart.WPF.Styles.Line line075 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line075.Color = System.Windows.Media.Color.FromRgb(140, 160, 180);//直线颜色
            TChart1.Series.Add(line075);//添加直线
            Steema.TeeChart.WPF.Styles.Line line076 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line076.Color = System.Windows.Media.Color.FromRgb(140, 80, 160);//直线颜色
            TChart1.Series.Add(line076);//添加直线
            Steema.TeeChart.WPF.Styles.Line line077 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line077.Color = System.Windows.Media.Color.FromRgb(210, 70, 180);//直线颜色
            TChart1.Series.Add(line077);//添加直线
            Steema.TeeChart.WPF.Styles.Line line078 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line078.Color = System.Windows.Media.Color.FromRgb(230, 80, 180);//直线颜色
            TChart1.Series.Add(line078);//添加直线
            Steema.TeeChart.WPF.Styles.Line line079 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line079.Color = System.Windows.Media.Color.FromRgb(140, 80, 170);//直线颜色
            TChart1.Series.Add(line079);//添加直线
            Steema.TeeChart.WPF.Styles.Line line080 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line080.Color = System.Windows.Media.Color.FromRgb(140, 170, 180);//直线颜色
            TChart1.Series.Add(line080);//添加直线
                    


            TChart2.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line line2 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line2.Color = System.Windows.Media.Color.FromRgb(255, 255, 255);//直线颜色
            TChart2.Header.Text = "Frame_Type";//Tchart窗体标题
            TChart2.Legend.Visible = false;//直线标题集合是否显示
            TChart2.Series.Add(line2);//添加直线
            TChart2.Axes.Bottom.Title.Text = "Timer(s)";//底部标题


            TChart3.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line line3 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line3.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            Steema.TeeChart.WPF.Styles.Line line4 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line4.Color = System.Windows.Media.Color.FromRgb(255, 255,255);//直线颜色
            TChart3.Header.Text = "Version";//Tchart窗体标题
            TChart3.Legend.Visible = false;//直线标题集合是否显示
            TChart3.Series.Add(line3);//添加直线
            TChart3.Series.Add(line4);//添加直线
            TChart3.Axes.Bottom.Title.Text = "Timer(s)";//底部标题  


            TChart4.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line line5 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line5.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            TChart4.Header.Text = "Flight Model";//Tchart窗体标题
            TChart4.Legend.Visible = false;//直线标题集合是否显示
            TChart4.Series.Add(line5);//添加直线
            TChart4.Axes.Bottom.Title.Text = "Timer(s)";//底部标题


            TChart5.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line line6 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line6.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//直线颜色
            TChart5.Header.Text = "IMU_State";//Tchart窗体标题
            TChart5.Legend.Visible = false;//直线标题集合是否显示
            TChart5.Series.Add(line6);//添加直线
            TChart5.Axes.Bottom.Title.Text = "Timer(s)";//底部标题


            TChart6.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line totorline0 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline0.Color = System.Windows.Media.Color.FromRgb(0, 0, 0);//1号电机直线颜色黑色
            totorline0.Title = "Motor1";//标题
            Steema.TeeChart.WPF.Styles.Line totorline1 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline1.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//2号电机直线颜色红色
            totorline1.Title = "Motor2";//标题
            Steema.TeeChart.WPF.Styles.Line totorline2 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline2.Color = System.Windows.Media.Color.FromRgb(0, 255, 0);//3号电机直线颜色绿色
            totorline2.Title = "Motor3";//标题
            Steema.TeeChart.WPF.Styles.Line totorline3 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline3.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//4号电机直线颜色蓝色
            totorline3.Title = "Motor4";//标题
            Steema.TeeChart.WPF.Styles.Line totorline4 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline4.Color = System.Windows.Media.Color.FromRgb(0, 255, 255);//5号电机直线颜色黄色
            totorline4.Title = "Motor5";//标题
            Steema.TeeChart.WPF.Styles.Line totorline5 = new Steema.TeeChart.WPF.Styles.Line();//直线
            totorline5.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);//6号电机直线颜色灰色
            totorline5.Title = "Motor6";//标题
            TChart6.Legend.CheckBoxes = true;//是否需要勾选
            TChart6.Legend.Visible = true;//直线标题集合是否显示
            TChart6.Legend.Alignment = LegendAlignments.Top;//直接标题顶部显示
            TChart6.Header.Text = "Motors";//Tchart窗体标题
            TChart6.Series.Add(totorline0);//添加直线
            TChart6.Series.Add(totorline1);//添加直线
            TChart6.Series.Add(totorline2);//添加直线
            TChart6.Series.Add(totorline3);//添加直线
            TChart6.Series.Add(totorline4);//添加直线
            TChart6.Series.Add(totorline5);//添加直线
            TChart6.Axes.Bottom.Title.Text = "Timer(s)";//底部标题

            TChart7.Aspect.View3D = false;//控件3D效果
            Steema.TeeChart.WPF.Styles.Line line081 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line081.Color = System.Windows.Media.Color.FromRgb(170, 80, 180);//直线颜色
            TChart7.Series.Add(line081);//添加直线
            Steema.TeeChart.WPF.Styles.Line line082 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line082.Color = System.Windows.Media.Color.FromRgb(140, 80, 190);//直线颜色
            TChart7.Series.Add(line082);//添加直线
            Steema.TeeChart.WPF.Styles.Line line083 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line083.Color = System.Windows.Media.Color.FromRgb(140, 190, 180);//直线颜色
            TChart7.Series.Add(line083);//添加直线
            Steema.TeeChart.WPF.Styles.Line line084 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line084.Color = System.Windows.Media.Color.FromRgb(190, 80, 180);//直线颜色
            TChart7.Series.Add(line084);//添加直线
            Steema.TeeChart.WPF.Styles.Line line085 = new Steema.TeeChart.WPF.Styles.Line();//直线
            line085.Color = System.Windows.Media.Color.FromRgb(230, 80, 180);//直线颜色
            TChart7.Series.Add(line085);//添加直线

            TChart7.Header.Text = "Frame_Type";//Tchart窗体标题
            TChart7.Legend.Visible = false;//直线标题集合是否显示

            TChart7.Axes.Bottom.Title.Text = "Timer(s)";//底部标题

            for (int i = 0; i < 20; i++)
            {
                All_Equipment_Info.IMU_HardWare_Version[i] = 32;
                All_Equipment_Info.IMU_SoftWare_Version[i] = 32;
                All_Equipment_Info.IMU_Equipment_ID[i] = 32;
                All_Equipment_Info.AP_HardWare_Version[i] = 32;
                All_Equipment_Info.AP_SoftWare_Version[i] = 32;
                All_Equipment_Info.AP_Equipment_ID[i] = 32;
                All_Equipment_Info.LED_HardWare_Version[i] = 32;
                All_Equipment_Info.LED_SoftWare_Version[i] = 32;
                All_Equipment_Info.LED_Equipment_ID[i] = 32;
                All_Equipment_Info.MAG_HardWare_Version[i] = 32;
                All_Equipment_Info.MAG_SoftWare_Version[i] = 32;
                All_Equipment_Info.MAG_Equipment_ID[i] = 32;
                All_Equipment_Info.GPS_HardWare_Version[i] = 32;
                All_Equipment_Info.GPS_SoftWare_Version[i] = 32;
                All_Equipment_Info.GPS_Equipment_ID[i] = 32;
                All_Equipment_Info.HUB_Equipment_ID[i] = 32;
                All_Equipment_Info.HUB_HardWare_Version[i] = 32;
                All_Equipment_Info.HUB_SoftWare_Version[i] = 32;
                All_Equipment_Info.FDR_HardWare_Version[i] = 32;
                All_Equipment_Info.FDR_SoftWare_Version[i] = 32;
                All_Equipment_Info.FDR_Equipment_ID[i] = 32;
                All_Equipment_Info.DTU_HardWare_Version[i] = 32;
                All_Equipment_Info.DTU_SoftWare_Version[i] = 32;
                All_Equipment_Info.DTU_Equipment_ID[i] = 32;
                All_Equipment_Info.RTK_HardWare_Version[i] = 32;
                All_Equipment_Info.RTK_SoftWare_Version[i] = 32;
                All_Equipment_Info.RTK_Equipment_ID[i] = 32;
            }
            Init();
        }
        //private void tChart1_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (TChart1 != null)
        //    {
        //        double XMid = ((CursorTool)TChart1.Tools[0]).XValue;
        //        if (e.Delta > 0)
        //        {
        //            double OldXMin = TChart1.Axes.Bottom.Minimum;
        //            double OldXMax = TChart1.Axes.Bottom.Maximum;
        //            double NewXMin = (XMid * 0.5 + OldXMin) / (1.5);
        //            double NewXMax = (XMid * 0.5 + OldXMax) / (1.5);
        //            TChart1.Axes.Bottom.SetMinMax(NewXMin, NewXMax);
        //            TChart1.Axes.Bottom.Scroll(1.01, true);// 
        //        }
        //        else
        //        {
        //            double OldXMin = TChart1.Axes.Bottom.Minimum;
        //            double OldXMax = TChart1.Axes.Bottom.Maximum;
        //            double NewXMin = (-XMid * 0.5 + OldXMin) / (0.5);
        //            double NewXMax = (-XMid * 0.5 + OldXMax) / (0.5);
        //            TChart1.Axes.Bottom.SetMinMax(NewXMin, NewXMax);
        //            TChart1.Axes.Bottom.Scroll(1.01,true);// 
        //        }
        //    }
        //}
        private void Init()
        {

            MainGMap.Manager.Mode = AccessMode.ServerAndCache;
            GMapProvider.WebProxy = System.Net.WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            MainGMap.MapProvider = GMapProviders.BingSatelliteMap; //GoogleSatelliteMap; //OpenStreetMap GaoDeSatellite   BingSatelliteMap     OviSatelliteMap           BingSatelliteMap                 //地图提供商

            MainGMap.MinZoom = 2;  //最小缩放
            MainGMap.MaxZoom = 21; //最大缩放
            MainGMap.Zoom = 19;     //当前缩放
            
            MainGMap.ShowCenter = true; //显示中心十字点
            MainGMap.DragButton = MouseButton.Left; //左键拖拽地图
            MainGMap.Position = new PointLatLng(31.9235064, 118.9869604); //地图中心位置：南京
          
            //

            UAVMarker = new GMapMarker(pList_points);
            UAVMarker.Shape = new CustomMarkerRed();
            UAVMarker.Offset = new System.Windows.Point(-15.5, -38.9);
            ////初始化路径列表
            UAVRoute = new GMapRoute(new List<PointLatLng>());
            //MainGMap.Markers.Add(UAVRoute);
            //UAVRoute.Points.Add(pList_points);
            cursortool = new CursorTool(TChart1.Chart);
            cursortool.Pen.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);//直线颜色 
           
            //cursortool.FollowMouse = true;
            cursortool.Style = CursorToolStyles.Vertical;
            TChart1.Tools.Add(cursortool);////获取工具链
        }
        /// <summary>
        /// 计算最终数组的函数
        /// </summary>
        /// <param name="m_list_data1todata3"></param>
        /// <returns></returns>
        private double[,] Function_OneToThree_Data(List<string> m_list_data1todata3)
        {
            Int64 row_num = m_list_data1todata3.Count;
            string[] sArray = m_list_data1todata3[0].Split(' ');
            Int64 column_num = sArray.Length;
            double[,] data1todata3_value = new double[row_num, column_num - 2];
            for (int i = 0; i < (row_num); i++)//行数
            {
                string[] String_Array = m_list_data1todata3[i].Split(' ');
                for (int j = 0; j < (column_num - 2); j++)//列数
                {
                    data1todata3_value[i, j] = Convert.ToDouble(String_Array[j + 1]);
                }
            }
            return data1todata3_value;
        }
        public void Function_Wait()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Wait_Dlg = new WaitProgressWindow();
                    Wait_Dlg.ShowDialog();
                }));
            }));
            t.Start();
        }
        /// <summary>
        /// /计算数组的行数和列数
        /// </summary>
        /// <param name="m_list_data1todata3"></param>
        /// <returns></returns>
        private RowAndClumn Function_Get_ColumnAndRow(List<string> m_list_data1todata3)
        {
            RowAndClumn m_rowandcolumn_value = new RowAndClumn();
            m_rowandcolumn_value.m_row_value = m_list_data1todata3.Count;
            string[] sArray = m_list_data1todata3[0].Split(' ');
            m_rowandcolumn_value.m_column_value = sArray.Length;
            return m_rowandcolumn_value;
        }




        private void Function_PickUp_Data(int DataBag_Style, int Data_Style)
        {
            Whitch_DataBag_Style = DataBag_Style;//将数据包类别赋值给Whitch_DataBag_Style
            Thread t = new Thread(new ThreadStart(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        TChart1.Axes.Left.Automatic = true;
                        TChart1.Axes.Left.Increment = 0;
                        if (DataBag_Style == 1)
                        {

                            double data1_real_time = 0.0;
                            if (Data_Style == 237)//高八位
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart4.Series[0].Add(data1_real_time, (((short)AutoWing_UsedData1_Array[i, 37] >> 8) & 0x00FF));
                                }
                            }
                            else if (Data_Style == 337)//低八位
                            {
                                Get_AllModel_Timer.Attitude_Flight_Time = 0.0;
                                Get_AllModel_Timer.GPS_Flight_Time = 0.0;
                                Get_AllModel_Timer.Auto_Flight_Time = 0.0;
                                Get_AllModel_Timer.Reback_Flight_Time = 0.0;
                                Get_AllModel_Timer.Suspend_Flight_Time = 0.0;
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart4.Series[0].Add(data1_real_time, ((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF));
                                    if (((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF) == 2)//姿态
                                    {
                                        Get_AllModel_Timer.Attitude_Flight_Time = Get_AllModel_Timer.Attitude_Flight_Time + data1_timer_frequency;
                                    }
                                    if (((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF) == 3)//GPS
                                    {
                                        Get_AllModel_Timer.GPS_Flight_Time = Get_AllModel_Timer.GPS_Flight_Time + data1_timer_frequency;
                                    }
                                    if (((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF) == 4)//自主
                                    {
                                        Get_AllModel_Timer.Auto_Flight_Time = Get_AllModel_Timer.Auto_Flight_Time + data1_timer_frequency;
                                    }
                                    if (((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF) == 7)//返航
                                    {
                                        Get_AllModel_Timer.Reback_Flight_Time = Get_AllModel_Timer.Reback_Flight_Time + data1_timer_frequency;
                                    }
                                    if (((short)AutoWing_UsedData1_Array[i, 37] & 0x00FF) == 10)//悬停
                                    {
                                        Get_AllModel_Timer.Suspend_Flight_Time = Get_AllModel_Timer.Suspend_Flight_Time + data1_timer_frequency;
                                    }
                                }
                                m_zitai_timer.Value = (Int32)Get_AllModel_Timer.Attitude_Flight_Time;
                                m_GPS_timer.Value = (Int32)Get_AllModel_Timer.GPS_Flight_Time;
                                m_zizhu_timer.Value = (Int32)Get_AllModel_Timer.Auto_Flight_Time;
                                m_fanhang_timer.Value = (Int32)Get_AllModel_Timer.Reback_Flight_Time;
                                m_zanting_timer.Value = (Int32)Get_AllModel_Timer.Suspend_Flight_Time;
                            }
                            else if (Data_Style == 46)
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart1.Series[whitch_line].Add(data1_real_time, (AutoWing_UsedData1_Array[i, Data_Style] / 10.0));
                                }
                            }
                            else if (Data_Style == 38)
                            {
                                Function_ImuState_Show((int)AutoWing_UsedData1_Array[0, Data_Style]);
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart5.Series[0].Add(data1_real_time, (AutoWing_UsedData1_Array[i, Data_Style]));
                                }
                            }
                            else if (Data_Style == 20)
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart6.Series[0].Add(data1_real_time, AutoWing_UsedData1_Array[i, Data_Style]);
                                    TChart6.Series[1].Add(data1_real_time, AutoWing_UsedData1_Array[i, 21]);
                                    TChart6.Series[2].Add(data1_real_time, AutoWing_UsedData1_Array[i, 22]);
                                    TChart6.Series[3].Add(data1_real_time, AutoWing_UsedData1_Array[i, 23]);
                                    TChart6.Series[4].Add(data1_real_time, AutoWing_UsedData1_Array[i, 24]);
                                    TChart6.Series[5].Add(data1_real_time, AutoWing_UsedData1_Array[i, 25]);
                                }
                            }
                            else if (Data_Style == 777)//测试轨迹
                            {
                                for (int i = 0; i < 100; i++)
                                {
                                    DataRow newRow = newdtb.NewRow();
                                    newRow["name_x"] = AutoWing_UsedTracklng_Array[i].ToString();
                                    newRow["name_y"] = AutoWing_UsedTracklat_Array[i].ToString();
                                    newdtb.Rows.Add(newRow);
                                    k_add.DataSource = newdtb.DefaultView;
                                }

                            }
                            else if (Data_Style == 34)//十进制转换二进制读取 liziran
                            {
                                TChart7.Axes.Left.Automatic = false;
                                TChart7.Axes.Left.Maximum = 3;
                                TChart7.Axes.Left.Minimum = -1;
                                TChart7.Axes.Left.Increment = 1;
                                if (error_message == 1)
                                {
                                    short[] buffer;
                                    buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                                    for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                    {
                                        short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                                        data1_real_time = (data1_real_time + data1_timer_frequency);
                                        nature = (short)(nature & 0x0001);
                                        buffer[i] = nature;

                                        TChart7.Series[whitch_line].Add(data1_real_time, nature);
                                    }

                                    for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                                   {
                                            if (buffer[x] == 0)
                                            {
                                                Gps_State.Content = "Normal";
                                            }
                                            else
                                            {
                                                Gps_State.Content = "Abnormal";
                                                break;
                                            }
                                   }
                                    
                                }
                                else if (error_message == 2)
                                {
                                    short[] buffer;
                                    buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                                    for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                    {
                                        short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                                        data1_real_time = (data1_real_time + data1_timer_frequency);
                                        nature = (short)((nature >> 1) & 0x0001);
                                        buffer[i] = nature;
                                        TChart7.Series[whitch_line].Add(data1_real_time, nature);
                                    }
                                    for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                                    {
                                        if (buffer[x] == 0)
                                        {
                                            Mag_State.Content = "Normal";
                                        }
                                        else
                                        {
                                            Mag_State.Content = "Abnormal";
                                            break;
                                        }
                                    }
                                }
                                else if (error_message == 3)
                                {
                                    short[] buffer;
                                    buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                                    for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                    {
                                        short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                                        data1_real_time = (data1_real_time + data1_timer_frequency);
                                        nature = (short)((nature >> 2) & 0x0001);
                                        buffer[i] = nature;
                                        TChart7.Series[whitch_line].Add(data1_real_time, nature);
                                    }
                                    for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                                    {
                                        if (buffer[x] == 0)
                                        {
                                            Yaokongqi_state.Content = "Normal";
                                        }
                                        else
                                        {
                                            Yaokongqi_state.Content = "Abnormal";
                                            break;
                                        }
                                    }
                                }
                                else if (error_message == 4)
                                {
                                    short[] buffer;
                                    buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                                    for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                    {
                                        short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                                        data1_real_time = (data1_real_time + data1_timer_frequency);
                                        nature = (short)((nature >> 3) & 0x0003);
                                        buffer[i] = nature;
                                        TChart7.Series[whitch_line].Add(data1_real_time, nature);
                                    }
                                    for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                                    {
                                        if (buffer[x] == 0)
                                        {
                                            Battery_state.Content = "Normal";
                                        }
                                        else
                                        {
                                            Battery_state.Content = "Abnormal";
                                            break;
                                        }
                                    }
                                }
                                else if (error_message == 5)
                                {
                                    int y = 0;
                                    short[] buffer;
                                    buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                                    for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                    {
                                        short nature =(short) AutoWing_UsedData1_Array[i, Data_Style];
                                        data1_real_time = (data1_real_time + data1_timer_frequency);
                                        nature = (short)((nature >> 5) & 0x003);
                                        buffer[i] = nature;
                                        TChart7.Series[whitch_line].Add(data1_real_time, nature);
                                    }
                                    for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                                    {
                                        if (buffer[x] > 0)
                                        {
                                            Star_state.Content = "Normal";
                                            for (int i = x; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                            {
                                                if (buffer[i] == 0)
                                                {
                                                    Star_state.Content = "Abnormal";
                                                    y++;
                                                }
                                            }

                                        }
                                        if (y > 0)
                                            break;
                                        
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data1_real_time = (data1_real_time + data1_timer_frequency);
                                    TChart1.Series[whitch_line].Add(data1_real_time, AutoWing_UsedData1_Array[i, Data_Style]);
                                }
                            }

                            //wait_flag = 0;
                            //wait_count = 0;
                            //Wait_Dlg.Close();
                        }
                        else if (DataBag_Style == 2)
                        {
                            double data2_real_time = 0.0;
                            if (Data_Style == 0)
                            {
                                Function_Reback_ShowInfo((int)AutoWing_UsedData2_Array[0, 0], (int)AutoWing_UsedData2_Array[0, 1]);
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data2_real_time = (data2_real_time + data2_timer_frequency);
                                    TChart2.Series[0].Add(data2_real_time, AutoWing_UsedData2_Array[i, Data_Style]);
                                }
                            }
                            else if (Data_Style == 4)
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    Function_Version_Analyze((int)AutoWing_UsedData2_Array[i, 4]);
                                    Function_Version_Analyze((int)AutoWing_UsedData2_Array[i, 5]);
                                    //////////增加apcode显示33/////////////////////////////////////////
                                    int high_byte = 0;
                                    int low_byte = 0;
                                    high_byte = (((Int32)AutoWing_UsedData2_Array[i, 42] >> 8) & 0x00ff);
                                    low_byte = ((Int32)AutoWing_UsedData2_Array[i, 42] & 0x00ff);
                                    All_Equipment_Info.AP_Equipment_ID[high_byte] = (byte)low_byte;
                                    data2_real_time = (data2_real_time + data2_timer_frequency);
                                    TChart3.Series[0].Add(data2_real_time, AutoWing_UsedData2_Array[i, 4]);
                                    TChart3.Series[1].Add(data2_real_time, AutoWing_UsedData2_Array[i, 5]);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    data2_real_time = (data2_real_time + data2_timer_frequency);
                                    TChart1.Series[whitch_line].Add(data2_real_time, AutoWing_UsedData2_Array[i, Data_Style]);
                                }
                            }
                        }
                        else if (DataBag_Style == 4)
                        {
                            double data4_real_time = 0.0;
                            for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                            {
                                data4_real_time = (data4_real_time + data1_timer_frequency);
                                TChart1.Series[whitch_line].Add(data4_real_time, AutoWing_UsedData4_Array[i, Data_Style]);
                            }
                        
                        }
                    }
                    catch (Exception)
                    {
                        
                        //wait_flag = 0;
                        //wait_count = 0;
                        //Wait_Dlg.Close();
                    }

                }));
            }));
            t.Start();
        }
        private void Function_PickUp_Data2(int DataBag_Style, int Data_Style)
        {
            double data1_real_time = 0.0;
            if (DataBag_Style == 1)
            {
                if (Data_Style == 34)//十进制转换二进制读取 liziran
                {
                    TChart7.Axes.Left.Automatic = false;
                    TChart7.Axes.Left.Maximum = 3;
                    TChart7.Axes.Left.Minimum = -1;
                    TChart7.Axes.Left.Increment = 1;
                    if (error_message == 1)
                    {
                        short[] buffer;
                        buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                        for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                        {
                            short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                            data1_real_time = (data1_real_time + data1_timer_frequency);
                            nature = (short)(nature & 0x0001);
                            buffer[i] = nature;

                            TChart7.Series[whitch_line].Add(data1_real_time, nature);
                        }

                        for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                        {
                            if (buffer[x] == 0)
                            {
                                Gps_State.Content = "Normal";
                            }
                            else
                            {
                                Gps_State.Content = "Abnormal";
                                break;
                            }
                        }

                    }
                    else if (error_message == 2)
                    {
                        short[] buffer;
                        buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                        for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                        {
                            short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                            data1_real_time = (data1_real_time + data1_timer_frequency);
                            nature = (short)((nature >> 1) & 0x0001);
                            buffer[i] = nature;
                            TChart7.Series[whitch_line].Add(data1_real_time, nature);
                        }
                        for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                        {
                            if (buffer[x] == 0)
                            {
                                Mag_State.Content = "Normal";
                            }
                            else
                            {
                                Mag_State.Content = "Abnormal";
                                break;
                            }
                        }
                    }
                    else if (error_message == 3)
                    {
                        short[] buffer;
                        buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                        for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                        {
                            short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                            data1_real_time = (data1_real_time + data1_timer_frequency);
                            nature = (short)((nature >> 2) & 0x0001);
                            buffer[i] = nature;
                            TChart7.Series[whitch_line].Add(data1_real_time, nature);
                        }
                        for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                        {
                            if (buffer[x] == 0)
                            {
                                Yaokongqi_state.Content = "Normal";
                            }
                            else
                            {
                                Yaokongqi_state.Content = "Abnormal";
                                break;
                            }
                        }
                    }
                    else if (error_message == 4)
                    {
                        short[] buffer;
                        buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                        for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                        {
                            short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                            data1_real_time = (data1_real_time + data1_timer_frequency);
                            nature = (short)((nature >> 3) & 0x0003);
                            buffer[i] = nature;
                            TChart7.Series[whitch_line].Add(data1_real_time, nature);
                        }
                        for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                        {
                            if (buffer[x] == 0)
                            {
                                Battery_state.Content = "Normal";
                            }
                            else
                            {
                                Battery_state.Content = "Abnormal";
                                break;
                            }
                        }
                    }
                    else if (error_message == 5)
                    {
                        int y = 0;
                        short[] buffer;
                        buffer = new short[GetData1_RowAndColumn_Value.m_row_value];
                        for (int i = 0; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                        {
                            short nature = (short)AutoWing_UsedData1_Array[i, Data_Style];
                            data1_real_time = (data1_real_time + data1_timer_frequency);
                            nature = (short)((nature >> 5) & 0x003);
                            buffer[i] = nature;
                            TChart7.Series[whitch_line].Add(data1_real_time, nature);
                        }
                        for (int x = 0; x < GetData1_RowAndColumn_Value.m_row_value; x++)
                        {
                            if (buffer[x] > 0)
                            {
                                Star_state.Content = "Normal";
                                for (int i = x; i < GetData1_RowAndColumn_Value.m_row_value; i++)
                                {
                                    if (buffer[i] == 0)
                                    {
                                        Star_state.Content = "Abnormal";
                                        y++;
                                    }
                                }

                            }
                            if (y > 0)
                                break;

                        }
                    }
                }
            }

        }

        private void Function_Version_Analyze(int Recv_Version_Data)
        {
            byte High_Byte = Convert.ToByte(((Recv_Version_Data & 0x0000ff00) >> 8));//接收到的高8位
            byte Low_Byte = Convert.ToByte(Recv_Version_Data & 0x000000ff);//接收到的低8位
            int Equipment_Type = 0;//设备的类型（IMU、AP、LED、MAG、GPS、DTU）
            int Equipment_String_Num = 0;//具体填充的是哪一个数组
            byte Equipment_Value = 0;//往数组中填充的具体是什么值
            Equipment_Type = High_Byte;//获取到实际的Type值（0=IMU;1=AP;2=LED;3=MAG;4=GPS;5=FDR;6=DTU）
            Equipment_Value = Convert.ToByte((Low_Byte & 0x7f));//具体收到的值
            Equipment_String_Num = (High_Byte % 10);//具体对应的是数组的那个坑
            if (Equipment_Value == 0)
            {
                Equipment_Value = 32;
            }
            switch (Equipment_Type / 10)
            {
                case 0:                     //AP软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.AP_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.AP_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.AP_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.AP_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.AP_HardWare_Version);
                    m_ap_hard.Content = All_Equipment_Info.AP_HardWare_VersionShow.Replace("_", "__");

                    All_Equipment_Info.AP_Equipment_ID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.AP_Equipment_ID);
                    m_ap_codeinfo.Content = All_Equipment_Info.AP_Equipment_ID_VersionShow;//将ap的id赋值给要显示的变量

                    //        AP_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.AP_SoftWare_Version);
                    //        AP_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.AP_Equipment_ID);
                    break;
                case 1:						//IMU软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.IMU_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.IMU_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.IMU_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.IMU_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.IMU_HardWare_Version);
                    m_imu_hard.Content = All_Equipment_Info.IMU_HardWare_VersionShow.Replace("_", "__");
                    //       IMU_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.IMU_SoftWare_Version);
                    //       IMU_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.IMU_Equipment_ID);
                    break;
                case 2:						//GPS软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.GPS_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.GPS_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.GPS_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.GPS_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.GPS_HardWare_Version);
                    m_gps_hard.Content = All_Equipment_Info.GPS_HardWare_VersionShow.Replace("_", "__");
                    //      GPS_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.GPS_SoftWare_Version);
                    //      GPS_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.GPS_Equipment_ID);
                    break;
                case 3:						//HUB软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.HUB_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.HUB_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.HUB_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.HUB_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.HUB_HardWare_Version);
                    m_hub_hard.Content = All_Equipment_Info.HUB_HardWare_VersionShow.Replace("_", "__");
                    //     HUB_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.HUB_SoftWare_Version);
                    //     HUB_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.HUB_Equipment_ID);
                    break;
                case 4:					//mag软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.MAG_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.MAG_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.MAG_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.MAG_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.MAG_HardWare_Version);
                    m_mag_hard.Content = All_Equipment_Info.MAG_HardWare_VersionShow.Replace("_", "__");
                    //     MAG_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.MAG_SoftWare_Version);
                    //      MAG_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.MAG_Equipment_ID);
                    break;
                case 5:					//led软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.LED_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.LED_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.LED_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.LED_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.LED_HardWare_Version);
                    m_led_hard.Content = All_Equipment_Info.LED_HardWare_VersionShow.Replace("_", "__");
                    //     LED_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.LED_SoftWare_Version);
                    //     LED_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.LED_Equipment_ID);
                    break;
                case 6:					//FDR软硬件编号的解析
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.FDR_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.FDR_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.FDR_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.FDR_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.FDR_HardWare_Version);
                    m_fdr_hard.Content = All_Equipment_Info.FDR_HardWare_VersionShow.Replace("_", "__");
                    //     FDR_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.FDR_SoftWare_Version);
                    //    FDR_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.FDR_Equipment_ID);
                    break;
                case 7:					//DTU版本
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.DTU_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.DTU_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.DTU_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.DTU_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.DTU_HardWare_Version);
                    m_dtu_hard.Content = All_Equipment_Info.DTU_HardWare_VersionShow.Replace("_", "__");
                    //    DTU_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.DTU_SoftWare_Version);
                    //     DTU_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.DTU_Equipment_ID);
                    break;
                case 8:					//RTK版本
                    if (Equipment_String_Num < 20)
                    {
                        All_Equipment_Info.RTK_HardWare_Version[Equipment_String_Num] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 20 && Equipment_String_Num < 40)
                    {
                        All_Equipment_Info.RTK_SoftWare_Version[Equipment_String_Num - 20] = Equipment_Value;
                    }
                    else if (Equipment_String_Num >= 40)
                    {
                        All_Equipment_Info.RTK_Equipment_ID[Equipment_String_Num - 40] = Equipment_Value;
                    }
                    All_Equipment_Info.RTK_HardWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.RTK_HardWare_Version);
                    m_rtk_hard.Content = All_Equipment_Info.RTK_HardWare_VersionShow.Replace("_", "__");
                    //    RTK_SoftWare_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.RTK_SoftWare_Version);
                    //    RTK_EquipmentID_VersionShow = System.Text.Encoding.Default.GetString(All_Equipment_Info.RTK_Equipment_ID);
                    break;


                default:
                    break;
            }
        }

        private void Function_ImuState_Show(int AutoWingSensorState)
        {
            Imu_Check_Acc = (AutoWingSensorState >> 6) & 0x0003;
            Imu_Check_Gyro = (AutoWingSensorState >> 8) & 0x0003;
            Imu_Check_Mag = (AutoWingSensorState) & 0x0003;

            if (m_language_choose.SelectedIndex == 0)
            {
                m_imu_attvalue.Content = "正常";
                if (Imu_Check_Acc == 1 || Imu_Check_Acc == 3)
                {
                    m_imu_accvalue.Content = "异常";
                }
                else
                {
                    m_imu_accvalue.Content = "正常";
                }
                if (Imu_Check_Mag == 1 || Imu_Check_Mag == 3)
                {
                    m_imu_magvalue.Content = "异常";
                }
                else
                {
                    m_imu_magvalue.Content = "正常";
                }
                if (Imu_Check_Gyro == 1 || Imu_Check_Gyro == 3)
                {
                    m_imu_gryovalue.Content = "异常";
                }
                else
                {
                    m_imu_gryovalue.Content = "正常";
                }
            }
            else if (m_language_choose.SelectedIndex == 1)
            {
                m_imu_attvalue.Content = "Normal";
                if (Imu_Check_Acc == 1 || Imu_Check_Acc == 3)
                {
                    m_imu_accvalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_accvalue.Content = "Normal";
                }
                if (Imu_Check_Mag == 1 || Imu_Check_Mag == 3)
                {
                    m_imu_magvalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_magvalue.Content = "Normal";
                }
                if (Imu_Check_Gyro == 1 || Imu_Check_Gyro == 3)
                {
                    m_imu_gryovalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_gryovalue.Content = "Normal";
                }
            }

        }

        private void Function_Reback_ShowInfo(int Show_DataSelect_QUADE, int Show_tape_speedvalue)
        {
            byte High_Byte = (byte)((Show_DataSelect_QUADE & 0x0000ff00) >> 8);//接收到的高8位
            byte Low_Byte = (byte)(Show_DataSelect_QUADE & 0x000000ff);//接收到的低8位
            Flight_Style_Num = High_Byte;//机型选择编码1开始的
            YKQType_Num = (int)(Low_Byte & 0x01);//遥控器类型0开始的
            Battery_Num = (int)((Low_Byte & 0x06) >> 1);//电池类型0开始的
            IfReback_Num = (int)((Low_Byte & 0x80) >> 7);//是否回中0开始的
            if (m_language_choose.SelectedIndex == 0)
            {
                switch (Flight_Style_Num)//机型相关的
                {
                    case 0://默认情况把值赋值为2
                        Btn_FlightModel_Choose.Content = "四轴叉字逆";
                        break;
                    case 1://4叉字顺时针
                        Btn_FlightModel_Choose.Content = "四轴叉字顺";
                        break;
                    case 2://4叉字逆时针
                        Btn_FlightModel_Choose.Content = "四轴叉字逆";
                        break;
                    case 3://4十字顺时针
                        Btn_FlightModel_Choose.Content = "四轴十字顺";
                        break;
                    case 4://4十字逆时针
                        Btn_FlightModel_Choose.Content = "四轴十字逆";
                        break;
                    case 5://6叉字顺时针
                        Btn_FlightModel_Choose.Content = "六轴叉字顺";
                        break;
                    case 6://6叉字逆时针
                        Btn_FlightModel_Choose.Content = "六轴叉字逆";
                        break;
                    case 7://6十字顺时针
                        Btn_FlightModel_Choose.Content = "六轴十字顺";
                        break;
                    case 8://6十字逆时针
                        Btn_FlightModel_Choose.Content = "六轴十字逆";
                        break;
                    case 9://8V字顺时针
                        Btn_FlightModel_Choose.Content = "八轴叉字顺";
                        break;
                    case 10://8V字逆时针
                        Btn_FlightModel_Choose.Content = "八轴叉字逆";
                        break;
                    case 11://8一字顺时针
                        Btn_FlightModel_Choose.Content = "八轴十字顺";
                        break;
                    case 12://8一字逆时针
                        Btn_FlightModel_Choose.Content = "八轴十字逆";
                        break;
                    case 13://共轴叉字顺时针
                        Btn_FlightModel_Choose.Content = "共轴叉字顺";
                        break;
                    case 14://共轴叉字逆时针
                        Btn_FlightModel_Choose.Content = "共轴叉字逆";
                        break;
                    default:
                        break;
                }
                switch (YKQType_Num)//遥控器类型
                {
                    case 0://14SG
                        Btn_TelModel_Choose.Content = "14SG";
                        break;
                    case 1://天地飞
                        Btn_TelModel_Choose.Content = "其它";
                        break;
                    default:
                        break;
                }
                switch (Battery_Num)//电池类型
                {
                    case 0://6s电池
                        Btn_BatteryModel_Choose.Content = "6S";
                        break;
                    case 1://12s电池
                        Btn_BatteryModel_Choose.Content = "12S";
                        break;
                    case 2://4s电池
                        Btn_BatteryModel_Choose.Content = "4S";
                        break;
                    case 3://3s电池
                        Btn_BatteryModel_Choose.Content = "3S";
                        break;
                    default:
                        break;
                }
                switch (IfReback_Num)//是否回中
                {
                    case 0://回中
                        Ay_UsedIfBack_Index.Content = "回中";
                        break;
                    case 1://不回中
                        Ay_UsedIfBack_Index.Content = "不回中";
                        break;
                    default:
                        break;
                }
            }
            else if (m_language_choose.SelectedIndex == 1)
            {
                switch (Flight_Style_Num)//机型相关的
                {
                    case 0://默认情况把值赋值为2
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Anticlockwise";
                        break;
                    case 1://4叉字顺时针
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Clockwise";
                        break;
                    case 2://4叉字逆时针
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Anticlockwise";
                        break;
                    case 3://4十字顺时针
                        Btn_FlightModel_Choose.Content = "Four-rotors '+'Clockwise";
                        break;
                    case 4://4十字逆时针
                        Btn_FlightModel_Choose.Content = "Foue-rotors '+'Anticlockwise";
                        break;
                    case 5://6叉字顺时针
                        Btn_FlightModel_Choose.Content = "Six-rotors 'x'Clockwise";
                        break;
                    case 6://6叉字逆时针
                        Btn_FlightModel_Choose.Content = "Six-rotors 'x'Anticlockwise";
                        break;
                    case 7://6十字顺时针
                        Btn_FlightModel_Choose.Content = "Six-rotors '+'Clockwise";
                        break;
                    case 8://6十字逆时针
                        Btn_FlightModel_Choose.Content = "Six-rotors '+'Anticlockwise";
                        break;
                    case 9://8V字顺时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors 'x'Clockwise";
                        break;
                    case 10://8V字逆时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors 'x'Anticlockwise";
                        break;
                    case 11://8一字顺时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors '+'Clockwise";
                        break;
                    case 12://8一字逆时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors '+'Anticlockwise";
                        break;
                    case 13://共轴叉字顺时针
                        Btn_FlightModel_Choose.Content = "Coaxial 'x'Clockwise";
                        break;
                    case 14://共轴叉字逆时针
                        Btn_FlightModel_Choose.Content = "Coaxial 'x'Anticlockwise";
                        break;
                    default:
                        break;
                }
                switch (YKQType_Num)//遥控器类型
                {
                    case 0://14SG
                        Btn_TelModel_Choose.Content = "14SG";
                        break;
                    case 1://天地飞
                        Btn_TelModel_Choose.Content = "Others";
                        break;
                    default:
                        break;
                }
                switch (Battery_Num)//电池类型
                {
                    case 0://6s电池
                        Btn_BatteryModel_Choose.Content = "6S";
                        break;
                    case 1://12s电池
                        Btn_BatteryModel_Choose.Content = "12S";
                        break;
                    case 2://4s电池
                        Btn_BatteryModel_Choose.Content = "4S";
                        break;
                    case 3://3s电池
                        Btn_BatteryModel_Choose.Content = "3S";
                        break;
                    default:
                        break;
                }
                switch (IfReback_Num)//是否回中
                {
                    case 0://回中
                        Ay_UsedIfBack_Index.Content = "Return middle";
                        break;
                    case 1://不回中
                        Ay_UsedIfBack_Index.Content = "Don't return middle";
                        break;
                    default:
                        break;
                }
            }
            m_idle_value.Content = Show_tape_speedvalue.ToString() + "%";
        }
        private void Function_Load_Data(string path)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    string filename = path;
                    StreamReader test_data = File.OpenText(filename);
                    List<string> m_list_data1 = new List<string>();
                    List<string> m_list_data2 = new List<string>();
                    List<string> m_list_data3 = new List<string>();
                    List<string> m_list_data4 = new List<string>(); 

                    int Start_Flag = 0;
                    while (test_data.Peek() != -1)
                    {
                        string test_true_value = test_data.ReadLine();
                        if (Start_Flag == 0)
                        {
                            if (test_true_value.Contains("Data1") || test_true_value.Contains("Data2") || test_true_value.Contains("Data3") || test_true_value.Contains("Data4"))
                            {
                                Start_Flag = 1;
                            }
                        }
                        if (Start_Flag == 1)
                        {
                            if (test_true_value != "")
                            {
                                if (test_true_value.Contains("Data1:"))
                                {
                                    m_list_data1.Add(test_true_value);
                                }
                                if (test_true_value.Contains("Data2:"))
                                {
                                    m_list_data2.Add(test_true_value);
                                }
                                if (test_true_value.Contains("Data3:"))
                                {
                                    m_list_data3.Add(test_true_value);
                                }
                                if (test_true_value.Contains("Data4:"))
                                {
                                    m_list_data4.Add(test_true_value);
                                }
                            }
                        }
                    }
                    try
                    {
                        if (Start_Flag == 0)
                        {
                            test_data.Close();
                            wait_flag = 0;
                            wait_count = 0;
                            Wait_Dlg.Close();
                            if (m_language_choose.SelectedIndex == 0)
                            {
                                System.Windows.MessageBox.Show("请载入正确的数据文件");
                            }
                            else if (m_language_choose.SelectedIndex == 1)
                            {
                                System.Windows.MessageBox.Show("Please load the right file");
                            }

                            return;
                        }
                        else
                        {
                            test_data.Close();
                           
                            If_Load_Data_Success = true;
                            for (int i = 0; i < 20; i++)
                            {
                                All_Equipment_Info.IMU_HardWare_Version[i] = 32;
                                All_Equipment_Info.IMU_SoftWare_Version[i] = 32;
                                All_Equipment_Info.IMU_Equipment_ID[i] = 32;
                                All_Equipment_Info.AP_HardWare_Version[i] = 32;
                                All_Equipment_Info.AP_SoftWare_Version[i] = 32;
                                All_Equipment_Info.AP_Equipment_ID[i] = 32;
                                All_Equipment_Info.LED_HardWare_Version[i] = 32;
                                All_Equipment_Info.LED_SoftWare_Version[i] = 32;
                                All_Equipment_Info.LED_Equipment_ID[i] = 32;
                                All_Equipment_Info.MAG_HardWare_Version[i] = 32;
                                All_Equipment_Info.MAG_SoftWare_Version[i] = 32;
                                All_Equipment_Info.MAG_Equipment_ID[i] = 32;
                                All_Equipment_Info.GPS_HardWare_Version[i] = 32;
                                All_Equipment_Info.GPS_SoftWare_Version[i] = 32;
                                All_Equipment_Info.GPS_Equipment_ID[i] = 32;
                                All_Equipment_Info.HUB_Equipment_ID[i] = 32;
                                All_Equipment_Info.HUB_HardWare_Version[i] = 32;
                                All_Equipment_Info.HUB_SoftWare_Version[i] = 32;
                                All_Equipment_Info.FDR_HardWare_Version[i] = 32;
                                All_Equipment_Info.FDR_SoftWare_Version[i] = 32;
                                All_Equipment_Info.FDR_Equipment_ID[i] = 32;
                                All_Equipment_Info.DTU_HardWare_Version[i] = 32;
                                All_Equipment_Info.DTU_SoftWare_Version[i] = 32;
                                All_Equipment_Info.DTU_Equipment_ID[i] = 32;
                                All_Equipment_Info.RTK_HardWare_Version[i] = 32;
                                All_Equipment_Info.RTK_SoftWare_Version[i] = 32;
                                All_Equipment_Info.RTK_Equipment_ID[i] = 32;
                            }
                        }
                        ////////////计算数据包1的行数列数及对应的数组中的值////////////////////
                        GetData1_RowAndColumn_Value = Function_Get_ColumnAndRow(m_list_data1);
                        AutoWing_UsedData1_Array = new double[GetData1_RowAndColumn_Value.m_row_value, GetData1_RowAndColumn_Value.m_column_value];
                        AutoWing_UsedData1_Array = Function_OneToThree_Data(m_list_data1);
                        ////////////计算数据包2的行数列数及对应的数组中的值////////////////////
                        GetData2_RowAndColumn_Value = Function_Get_ColumnAndRow(m_list_data2);
                        AutoWing_UsedData2_Array = new double[GetData2_RowAndColumn_Value.m_row_value, GetData2_RowAndColumn_Value.m_column_value];
                        AutoWing_UsedData2_Array = Function_OneToThree_Data(m_list_data2);
                        ////////////计算数据包3的行数列数及对应的数组中的值////////////////////
                        GetData3_RowAndColumn_Value = Function_Get_ColumnAndRow(m_list_data3);
                        AutoWing_UsedData3_Array = new double[GetData3_RowAndColumn_Value.m_row_value, GetData3_RowAndColumn_Value.m_column_value];
                        AutoWing_UsedData3_Array = Function_OneToThree_Data(m_list_data3);
                        ////////////计算数据包4的行数列数及对应的数组中的值////////////////////
                        try
                        {
                            GetData4_RowAndColumn_Value = Function_Get_ColumnAndRow(m_list_data4);
                            AutoWing_UsedData4_Array = new double[GetData4_RowAndColumn_Value.m_row_value, GetData4_RowAndColumn_Value.m_column_value];
                            AutoWing_UsedData4_Array = Function_OneToThree_Data(m_list_data4);
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("No Data4");
                        }
                        //////////////////////////将经纬度转化为具体的距离值//////////////////////////
                        AutoWing_UsedTracklat_Array = new double[GetData1_RowAndColumn_Value.m_row_value];//纵坐标
                        AutoWing_UsedTracklng_Array = new double[GetData1_RowAndColumn_Value.m_row_value];//横坐标
                        AutoWing_UsedTrackRoll_Array = new double[GetData1_RowAndColumn_Value.m_row_value];//roll
                        AutoWing_UsedTrackPitch_Array = new double[GetData1_RowAndColumn_Value.m_row_value];//pitch
                        AutoWing_UsedTrackYaw_Array = new double[GetData1_RowAndColumn_Value.m_row_value];//yaw
                        double Orignal_lat = 0.0;
                        double Orignal_lon = 0.0;
                        double history_lat = 1000.0;
                        double history_lon = 1000.0;

                        int track_flag = 0;
                        track_count = 0;
                        for (int i = 0; i < (GetData1_RowAndColumn_Value.m_row_value - 1); i++)
                        {
                            if (AutoWing_UsedData1_Array[i, 29] > 20 && track_flag == 0)
                            {
                                track_flag = 1;
                                Orignal_lon = AutoWing_UsedData1_Array[i, 29];//记录初始的原始点
                                Orignal_lat = AutoWing_UsedData1_Array[i, 30];//记录初始的原始点
                                UpdateMeterPerDeg(Orignal_lat, Orignal_lon);
                            }
                            if (track_flag == 1 && AutoWing_UsedData1_Array[i, 29] > 20 && AutoWing_UsedData1_Array[(i + 1), 29] > 20)
                            {
                                double current_lat = (Int32)((AutoWing_UsedData1_Array[(i + 1), 30] - Orignal_lat) * m_dMeterPerDeg_Latitude);
                                double current_lon = (Int32)((AutoWing_UsedData1_Array[(i + 1), 29] - Orignal_lon) * m_dMeterPerDeg_Longitude);

                                if (history_lat != current_lat || history_lon != current_lon)
                                {
                                    history_lat = current_lat;
                                    history_lon = current_lon;
                                    AutoWing_UsedTracklat_Array[track_count] = (Int32)((AutoWing_UsedData1_Array[(i + 1), 30] - Orignal_lat) * m_dMeterPerDeg_Latitude);
                                    AutoWing_UsedTracklng_Array[track_count] = (Int32)((AutoWing_UsedData1_Array[(i + 1), 29] - Orignal_lon) * m_dMeterPerDeg_Longitude);
                                    if ((AutoWing_UsedData1_Array[i, 2] >= -3.1415926 && AutoWing_UsedData1_Array[i, 2] <= 3.1415926) && (AutoWing_UsedData1_Array[i, 1] >= -3.1415926 && AutoWing_UsedData1_Array[i, 1] <= 3.1415926) && (AutoWing_UsedData1_Array[i, 0] >= -3.1415926 && AutoWing_UsedData1_Array[i, 0] <= 3.1415926))
                                    {
                                        AutoWing_UsedTrackRoll_Array[track_count] = (-AutoWing_UsedData1_Array[i, 0] * 180) / 3.1415926;//roll
                                        AutoWing_UsedTrackPitch_Array[track_count] = (AutoWing_UsedData1_Array[i, 1] * 180) / 3.1415926;//pitch
                                        AutoWing_UsedTrackYaw_Array[track_count] = (-AutoWing_UsedData1_Array[i, 2] * 180) / 3.1415926;//yaw
                                    }
                                    else
                                    {
                                        AutoWing_UsedTrackRoll_Array[track_count] = 0.0;//roll
                                        AutoWing_UsedTrackPitch_Array[track_count] = 0.0;//pitch
                                        AutoWing_UsedTrackYaw_Array[track_count] = 0.0;//yaw
                                    }
                                    track_count++;
                                }
                            }
                            double show_lat = AutoWing_UsedData1_Array[i, 30];
                            double show_lon = AutoWing_UsedData1_Array[i, 29];

                            pList_points.Lat = show_lat;//记录各个点
                            pList_points.Lng = show_lon;//记录各个点
                            
                            UAVRoute.Points.Add(pList_points);
                        }
                        MainGMap.Markers.Add(UAVRoute);
                        
                        UAVMarker.Position = (pList_points);
                        MainGMap.Markers.Add(UAVMarker);
                        MainGMap.Position = new PointLatLng(AutoWing_UsedData1_Array[GetData1_RowAndColumn_Value.m_row_value - 1, 30], AutoWing_UsedData1_Array[GetData1_RowAndColumn_Value.m_row_value - 1, 29]); //地图中心位置：南京
                        wait_flag = 0;
                        wait_count = 0;
                        Wait_Dlg.Close();
                        if (m_language_choose.SelectedIndex == 0 )
                        {
                            M_File_ShowPath.Text = filename;
                            System.Windows.MessageBox.Show("数据载入成功");
                        }
                        else if (m_language_choose.SelectedIndex == 1)
                        {
                            M_File_ShowPath.Text = filename;
                            System.Windows.MessageBox.Show("The file load success");
                        }
                    }
                    catch (Exception e)
                    {
                        if (m_language_choose.SelectedIndex == 0)
                        {
                            System.Windows.MessageBox.Show("数据格式有误");
                        }
                        else if (m_language_choose.SelectedIndex == 0)
                        {
                            System.Windows.MessageBox.Show("Load file format is incorrect");
                        }

                    }


                }));
            }));
            t.Start();
        }

        /// <summary>
        /// /距离换算每经纬度
        /// </summary>
        private void UpdateMeterPerDeg(double Org_Latitude, double Org_Longitude)
        {
            const double GPS_ELLIPSOID_A = 6378137.0;
            const double GPS_ELLIPSOID_F = 0.00335281066475;
            const double GPS_ELLIPSOID_B = (GPS_ELLIPSOID_A * (1 - GPS_ELLIPSOID_F));
            double dPseudoLatitudeRad, dDistanceFromCE;
            double dSin, dCos;
            if (Org_Latitude == 90.0 || Org_Latitude == -90.0)
                dPseudoLatitudeRad = Org_Latitude * 2.0 * 3.14159265358979 / 360.0;
            else
                dPseudoLatitudeRad = Math.Atan((1.0 - GPS_ELLIPSOID_F) * (1.0 - GPS_ELLIPSOID_F) * Math.Tan(Org_Latitude * 2.0 * 3.14159265358979 / 360.0));
            dSin = Math.Sin(dPseudoLatitudeRad);
            dCos = Math.Cos(dPseudoLatitudeRad);
            dDistanceFromCE = GPS_ELLIPSOID_A * GPS_ELLIPSOID_B / Math.Sqrt(GPS_ELLIPSOID_A * GPS_ELLIPSOID_A * dSin * dSin + GPS_ELLIPSOID_B * GPS_ELLIPSOID_B * dCos * dCos);
            m_dMeterPerDeg_Latitude = (2.0 * 3.14159265358979 / 360.0) * dDistanceFromCE;
            m_dMeterPerDeg_Longitude = m_dMeterPerDeg_Latitude * dCos;
        }
        private void Work_Thread()
        {
            var clear = MainGMap.Markers.Where(p => p == UAVRoute);
            if (clear != null)
            {
                for (int i = 0; i < clear.Count(); i++)
                {
                    MainGMap.Markers.Remove(clear.ElementAt(i));
                    i--;
                }


            }

            UAVRoute = new GMapRoute(new List<PointLatLng>());
            #region 通信线程
            Task.Factory.StartNew(() =>
            {
                try
                {
                    long nextFrame = sw.ElapsedMilliseconds;		//已用秒数(msec)
                    const long period = 50;	//周期(msec)
                    while (start_play_flag == 1)
                    {
                        long tickCount = sw.ElapsedMilliseconds;	// 获取当前时间
                        // 跳转
                        if (tickCount < nextFrame)
                        {
                            if (nextFrame - tickCount > 1)
                            {
                                System.Threading.Thread.Sleep((int)(nextFrame - tickCount));	// Wait等待时间，使得nextFrame = tickCount 
                            }
                            continue;
                        }
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if (wait_flag == 1)
                            {
                                wait_count++;
                            }
                            if (wait_count > 160)
                            {
                                wait_flag = 0;
                                wait_count = 0;
                                Wait_Dlg.Close();
                            }

                            if (start_play_flag == 1)

                            {
                                if (track_play_count < track_count)
                                {
                                    //TChart1.Series[0].Add(AutoWing_UsedTracklng_Array[track_play_count], AutoWing_UsedTracklat_Array[track_play_count]);
                                    DataRow newRow = newdtb.NewRow();
                                    newRow["name_x"] = AutoWing_UsedTracklng_Array[track_play_count].ToString();
                                    newRow["name_y"] = AutoWing_UsedTracklat_Array[track_play_count].ToString();
                                    newdtb.Rows.Add(newRow);
                                    k_add.DataSource = newdtb.DefaultView;
                                    // newdtb.Rows.Clear();
                                    ////////////////////////////hud显示///////////////////////
                                   // m_hud_control.YawAngle = AutoWing_UsedTrackYaw_Array[track_play_count];
                                   // m_hud_control.PitchAngle = AutoWing_UsedTrackPitch_Array[track_play_count];
                                   // m_hud_control.RollAngle = AutoWing_UsedTrackRoll_Array[track_play_count];
                                    double show_lat = AutoWing_UsedData1_Array[track_play_count * (GetData1_RowAndColumn_Value.m_row_value / track_count), 30];
                                    double show_lon = AutoWing_UsedData1_Array[track_play_count * (GetData1_RowAndColumn_Value.m_row_value / track_count), 29];

                                    pList_points.Lat = show_lat;//记录各个点
                                    pList_points.Lng = show_lon;//记录各个点
                                    UAVRoute.Points.Add(pList_points);

                                    MainGMap.Markers.Add(UAVRoute);//把轨迹添加到地图中

                                    UAVMarker.Position = (pList_points);
                                    UAVRoute.RegenerateShape(MainGMap);//地图上的轨迹实时显示，实时更新

                                    track_play_count++;
                                }
                                else
                                {
                                    start_play_flag = 0;
                                    track_play_count = 0;
                                    //  newdtb.Rows.Clear();
                                    System.Windows.MessageBox.Show("回放结束");
                                    one.Width = new GridLength(0.12, GridUnitType.Star);
                                    two.Width = new GridLength(0.45, GridUnitType.Star);
                                    three.Width = new GridLength(0.43, GridUnitType.Star);
                                }
                            }


                        }));
                        nextFrame += period;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("(301) Massage Receive Error: " + ex.Message);
                }
            });
            #endregion
        }



        private void MinWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void MaxWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (maxtomin == 0)
            {
                maxtomin = 1;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                maxtomin = 0;
                this.WindowState = WindowState.Normal;
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Label_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (maxtomin == 0)
            {
                maxtomin = 1;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                maxtomin = 0;
                this.WindowState = WindowState.Normal;
            }
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            m_data_load.Foreground = Brushes.YellowGreen;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            m_data_load.Foreground = Brushes.White;
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clear = MainGMap.Markers.Where(p => p == UAVRoute);
            if (clear != null)
            {
                for (int i = 0; i < clear.Count(); i++)
                {
                    MainGMap.Markers.Remove(clear.ElementAt(i));
                    i--;
                }


            }
            start_play_flag = 0;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "FDR_DATA";			// Default file name
            dlg.DefaultExt = ".txt";				// Default file extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                wait_count = 0;
                wait_flag = 1;
                Function_Wait();
                string filename = dlg.FileName;
                Function_Load_Data(filename);
            }
            else
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请选择正确的文件");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("Please choose the right file");
                }
                return;
            }
            UAVRoute = new GMapRoute(new List<PointLatLng>());
        }

        private void Label_MouseEnter_1(object sender, MouseEventArgs e)
        {
            m_data_help.Foreground = Brushes.YellowGreen;
        }

        private void Label_MouseLeave_1(object sender, MouseEventArgs e)
        {
            m_data_help.Foreground = Brushes.White;
        }

        private void Label_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            PDF_Read_Window PDFWindow = new PDF_Read_Window(m_language_choose.SelectedIndex);
            PDFWindow.Show();

            //System.Diagnostics.Process.Start(@"C:\Program Files\DataAnalysis\AYDroneDataAnalysis\text_read_source.exe");　　　　//调用该命令，在程序启动时打开Excel程序
        }

        private void Label_MouseEnter_2(object sender, MouseEventArgs e)
        {
            M_Labelabout_Change.Foreground = Brushes.YellowGreen;
        }

        private void Label_MouseLeave_2(object sender, MouseEventArgs e)
        {
            M_Labelabout_Change.Foreground = Brushes.White;
        }

        private void Label_PreviewMouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            AboutWindow AWindow = new AboutWindow(m_language_choose.SelectedIndex);
            AWindow.Show();
        }
        private void Label_PreviewMouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// TChart1 波形点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        /// <param name="valueIndex"></param>
        /// <param name="e"></param>
        private void TChart1_ClickSeries(object sender, Steema.TeeChart.WPF.Styles.Series s, int valueIndex, MouseEventArgs e)
        {

            int Temp_valueIndex = 0;
            if (Whitch_DataBag_Style == 1)
            {
                if (GetData1_RowAndColumn_Value.m_row_value <= valueIndex)
                {
                    return;
                }
                else
                {
                    UAVMarker.Position = new PointLatLng(AutoWing_UsedData1_Array[valueIndex, 30], AutoWing_UsedData1_Array[valueIndex, 29]);
                    MainGMap.Position = new PointLatLng(AutoWing_UsedData1_Array[valueIndex, 30], AutoWing_UsedData1_Array[valueIndex, 29]);
                    MainGMap.Zoom = 19;
                    cursortool.XValue = valueIndex * data1_timer_frequency;//点击显示在每个时刻的竖线
                }
            }
            else if (Whitch_DataBag_Style == 2)
            {
                if (GetData2_RowAndColumn_Value.m_row_value <= valueIndex)
                {
                    return;
                }
                else
                {
                    Temp_valueIndex = (Int32)(valueIndex * 15 / 25);
                    UAVMarker.Position = new PointLatLng(AutoWing_UsedData1_Array[Temp_valueIndex, 30], AutoWing_UsedData1_Array[Temp_valueIndex, 29]);
                    MainGMap.Position = new PointLatLng(AutoWing_UsedData1_Array[Temp_valueIndex, 30], AutoWing_UsedData1_Array[Temp_valueIndex, 29]);
                    MainGMap.Zoom = 19;
                    cursortool.XValue = valueIndex * data2_timer_frequency;//点击显示在每个时刻的竖线
                }
            }
            else if (Whitch_DataBag_Style == 4)
            {
                if (GetData4_RowAndColumn_Value.m_row_value <= valueIndex)
                {
                    return;
                }
                else
                {
                    Temp_valueIndex = (Int32)(valueIndex * 15 / 25);
                    UAVMarker.Position = new PointLatLng(AutoWing_UsedData1_Array[Temp_valueIndex, 30], AutoWing_UsedData1_Array[Temp_valueIndex, 29]);
                    MainGMap.Position = new PointLatLng(AutoWing_UsedData1_Array[Temp_valueIndex, 30], AutoWing_UsedData1_Array[Temp_valueIndex, 29]);
                    MainGMap.Zoom = 19;
                    cursortool.XValue = valueIndex * data2_timer_frequency;//点击显示在每个时刻的竖线
                }
            }
            else
            {
                cursortool.XValue = 0;
            }

        }
        /// <summary>
        /// roll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            whitch_line = 0;
            if (m_roll.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Roll";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 0);
            }
            else {
                TChart1.Series[whitch_line].Clear();
            }
           
        }
       
        /// <summary>
        /// pitch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_1(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 1;
            if (m_pitch.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Pitch";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 1);
            }
            else {
                TChart1.Series[whitch_line].Clear();
            }
        }
        /// <summary>
        /// yaw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_2(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 2;
            if (m_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Yaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 2);
            }
            else {
                TChart1.Series[whitch_line].Clear();
            }
        }
        /// <summary>
        /// proproll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_3(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 3;
            if (m_telecontroller_roll.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "PropRoll";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 12);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            
        }
        /// <summary>
        /// proppitch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_4(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 4;
            if (m_telecontroller_pitch.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "PropPitch";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 13);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }
        }
        /// <summary>
        /// propyaw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_5(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 5;
            if (m_telecontroller_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "PropYaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 14);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "PropYaw";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 14);
        }
        /// <summary>
        /// propthrottle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_6(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 6;
            if (m_telecontroller_throttle.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "PropThrottle";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 15);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "PropThrottle";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 15);
        }
        /// <summary>
        /// rollrate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_7(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 7;
            if (m_gyro_roll.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RollRate";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 3);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }



            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RollRate";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
            //Function_PickUp_Data(1, 3);
        }
        /// <summary>
        /// pitchrate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_8(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 8;
            if (m_gyro_pitch.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "PitchRate";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 4);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }



            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "PitchRate";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
            //Function_PickUp_Data(1, 4);
        }
        /// <summary>
        /// yawrate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_9(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 9;
            if (m_gyro_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "YawRate";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 5);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "YawRate";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
            //Function_PickUp_Data(1, 5);
        }
        /// <summary>
        /// xacc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_10(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 10;
            if (m_accelerate_x.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Xacc";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
                Function_PickUp_Data(1, 6);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Xacc";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
            //Function_PickUp_Data(1, 6);
        }
        /// <summary>
        /// yacc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_11(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 11;
            if (m_accelerate_y.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Yacc";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
                Function_PickUp_Data(1, 7);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Yacc";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
            //Function_PickUp_Data(1, 7);
        }
        /// <summary>
        /// zacc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_12(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 12;
            if (m_accelerate_z.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Zacc";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
                Function_PickUp_Data(1, 8);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Zacc";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Acc(m/s2)";//底部标题  
            //Function_PickUp_Data(1, 8);
        }
        /// <summary>
        /// magx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_13(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 13;
            if (m_mag_x.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Magx";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 9);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Magx";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 9);
        }
        /// <summary>
        /// magy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_14(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 14;
            if (m_mag_y.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Magy";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 10);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Magy";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 10);
        }
        /// <summary>
        /// magz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_15(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 15;
            if (m_mag_z.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Magz";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 11);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Magz";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 11);
        }
        /// <summary>
        /// switchA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_16(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 16;
            if (m_telecontroller_A.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "SwitchA";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 16);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "SwitchA";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 16);
        }
        /// <summary>
        /// SwitchB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_17(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 17;
            if (m_telecontroller_B.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "SwitchB";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 17);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "SwitchB";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 17);
        }
        /// <summary>
        /// SwitchC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_18(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            whitch_line = 18;
            if (m_telecontroller_C.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "SwitchC";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 18);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "SwitchC";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 18);
        }
        /// <summary>
        /// SwitchD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_19(object sender, RoutedEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();

            whitch_line = 19;
            if (m_telecontroller_D.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "SwitchA";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 19);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "SwitchD";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 19);
        }
        /// <summary>
        /// 电机1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_20(object sender, MouseButtonEventArgs e)
        {
            //wait_count = 0;
            //wait_flag = 1;
            //Function_Wait();
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 5;
            for (int i = 0; i < 6; i++)
            {
                TChart6.Series[i].Clear();
            }
            TChart6.Zoom.Undo();
            TChart6.Header.Text = "Motors";//Tchart窗体标题
            TChart6.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(1, 20);
        }
        /// <summary>
        /// 电机2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TreeViewItem_PreviewMouseLeftButtonDown_21(object sender, MouseButtonEventArgs e)
        //{
        //    //wait_count = 0;
        //    //wait_flag = 1;
        //    //Function_Wait();
        //    if (If_Load_Data_Success == false)
        //    {
        //        if (m_language_choose.SelectedIndex == 0)
        //        {
        //            System.Windows.MessageBox.Show("请先载入FDR数据");
        //        }
        //        else if (m_language_choose.SelectedIndex == 1)
        //        {
        //            System.Windows.MessageBox.Show("First please load the FDR data");
        //        }
        //        return;
        //    }
        //    m_tab_show.SelectedIndex = 0;
        //    TChart1.Series[0].Clear();
        //    TChart1.Zoom.Undo();
        //    TChart1.Header.Text = "Motor2";//Tchart窗体标题
        //    TChart1.Axes.Left.Title.Text = "";//底部标题  
        //    Function_PickUp_Data(1, 21);
        //}
        /// <summary>
        /// 电机3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TreeViewItem_PreviewMouseLeftButtonDown_22(object sender, MouseButtonEventArgs e)
        //{
        //    //wait_count = 0;
        //    //wait_flag = 1;
        //    //Function_Wait();
        //    if (If_Load_Data_Success == false)
        //    {
        //        if (m_language_choose.SelectedIndex == 0)
        //        {
        //            System.Windows.MessageBox.Show("请先载入FDR数据");
        //        }
        //        else if (m_language_choose.SelectedIndex == 1)
        //        {
        //            System.Windows.MessageBox.Show("First please load the FDR data");
        //        }
        //        return;
        //    }
        //    m_tab_show.SelectedIndex = 0;
        //    TChart1.Series[0].Clear();
        //    TChart1.Zoom.Undo();
        //    TChart1.Header.Text = "Motor3";//Tchart窗体标题
        //    TChart1.Axes.Left.Title.Text = "";//底部标题  
        //    Function_PickUp_Data(1, 22);
        //}
        /// <summary>
        /// 电机4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TreeViewItem_PreviewMouseLeftButtonDown_23(object sender, MouseButtonEventArgs e)
        //{
        //    //wait_count = 0;
        //    //wait_flag = 1;
        //    //Function_Wait();
        //    if (If_Load_Data_Success == false)
        //    {
        //        if (m_language_choose.SelectedIndex == 0)
        //        {
        //            System.Windows.MessageBox.Show("请先载入FDR数据");
        //        }
        //        else if (m_language_choose.SelectedIndex == 1)
        //        {
        //            System.Windows.MessageBox.Show("First please load the FDR data");
        //        }
        //        return;
        //    }
        //    m_tab_show.SelectedIndex = 0;
        //    TChart1.Series[0].Clear();
        //    TChart1.Zoom.Undo();
        //    TChart1.Header.Text = "Motor4";//Tchart窗体标题
        //    TChart1.Axes.Left.Title.Text = "";//底部标题  
        //    Function_PickUp_Data(1, 23);
        //}
        /// <summary>
        /// 电机5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TreeViewItem_PreviewMouseLeftButtonDown_24(object sender, MouseButtonEventArgs e)
        //{
        //    //wait_count = 0;
        //    //wait_flag = 1;
        //    //Function_Wait();
        //    if (If_Load_Data_Success == false)
        //    {
        //        if (m_language_choose.SelectedIndex == 0)
        //        {
        //            System.Windows.MessageBox.Show("请先载入FDR数据");
        //        }
        //        else if (m_language_choose.SelectedIndex == 1)
        //        {
        //            System.Windows.MessageBox.Show("First please load the FDR data");
        //        }
        //        return;
        //    }
        //    m_tab_show.SelectedIndex = 0;
        //    TChart1.Series[0].Clear();
        //    TChart1.Zoom.Undo();
        //    TChart1.Header.Text = "Motor5";//Tchart窗体标题
        //    TChart1.Axes.Left.Title.Text = "";//底部标题  
        //    Function_PickUp_Data(1, 24);
        //}
        /// <summary>
        /// 电机6
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TreeViewItem_PreviewMouseLeftButtonDown_25(object sender, MouseButtonEventArgs e)
        //{
        //    //wait_count = 0;
        //    //wait_flag = 1;
        //    //Function_Wait();
        //    if (If_Load_Data_Success == false)
        //    {
        //        if (m_language_choose.SelectedIndex == 0)
        //        {
        //            System.Windows.MessageBox.Show("请先载入FDR数据");
        //        }
        //        else if (m_language_choose.SelectedIndex == 1)
        //        {
        //            System.Windows.MessageBox.Show("First please load the FDR data");
        //        }
        //        return;
        //    }
        //    m_tab_show.SelectedIndex = 0;
        //    TChart1.Series[0].Clear();
        //    TChart1.Zoom.Undo();
        //    TChart1.Header.Text = "Motor6";//Tchart窗体标题
        //    TChart1.Axes.Left.Title.Text = "";//底部标题  
        //    Function_PickUp_Data(1, 25);
        //}
        /// <summary>
        /// GPS正北方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_26(object sender, RoutedEventArgs e)
        {

            whitch_line = 20;
            if (m_gps_northvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Vn";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(1, 26);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Vn";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
            //Function_PickUp_Data(1, 26);
        }
        /// <summary>
        /// GPS正东方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_27(object sender, RoutedEventArgs e)
        {


            whitch_line = 21;
            if (m_gps_eastvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Ve";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题 
                Function_PickUp_Data(1, 27);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Ve";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题 
            //Function_PickUp_Data(1, 27);
        }
        /// <summary>
        /// GPS垂直方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_28(object sender, RoutedEventArgs e)
        {
            whitch_line = 22;
            if (m_gps_updownvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Vd";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题 
                Function_PickUp_Data(1, 28);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }



            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Vd";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题 
            //Function_PickUp_Data(1, 28);
        }
        /// <summary>
        /// GPS中的经度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_29(object sender, RoutedEventArgs e)
        {
            whitch_line = 23;
            if (m_gps_lon.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Lon";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题 
                Function_PickUp_Data(1, 29);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Lon";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题 
            //Function_PickUp_Data(1, 29);
        }
        /// <summary>
        /// GPS中的纬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_30(object sender, RoutedEventArgs e)
        {
            whitch_line = 24;
            if (m_gps_lat.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Lat";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题 
                Function_PickUp_Data(1, 30);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Lat";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题 
            //Function_PickUp_Data(1, 30);
        }
        /// <summary>
        /// GPS中的高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_31(object sender, RoutedEventArgs e)
        {
            whitch_line = 25;
            if (m_gps_height.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Alt";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "(m)";//底部标题 
                Function_PickUp_Data(1, 31);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Alt";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "(m)";//底部标题 
            //Function_PickUp_Data(1, 31);
        }
        /// <summary>
        /// GPS中的航向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_32(object sender, RoutedEventArgs e)
        {

            whitch_line = 26;
            if (m_gps_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Heading";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
                Function_PickUp_Data(1, 32);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Heading";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "Angle(rad)";//底部标题  
            //Function_PickUp_Data(1, 32);
        }
        /// <summary>
        /// GPS中的星数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_33(object sender, RoutedEventArgs e)
        {
            whitch_line = 27;
            if (m_gps_starnum.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_StarNum";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 33);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_StarNum";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 33);
        }
        /// <summary>
        /// GPS中的DOP参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_34(object sender, RoutedEventArgs e)
        {
            whitch_line = 28;
            if (m_gps_dop.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "GPS_Dop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "GPS_Dop";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 34);
        }
        /// <summary>
        /// 气压计原始高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_35(object sender, RoutedEventArgs e)
        {

            whitch_line = 29;
            if (m_barometer_height.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Altitude_Ori";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "m";//底部标题  
                Function_PickUp_Data(1, 35);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Altitude_Ori";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "m";//底部标题  
            //Function_PickUp_Data(1, 35);
        }
        /// <summary>
        /// 电池电压
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_36(object sender, RoutedEventArgs e)
        {

            whitch_line = 30;
            if (m_state_volt.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Battery_Volt";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "v";//底部标题  
                Function_PickUp_Data(1, 36);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Battery_Volt";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "v";//底部标题  
            //Function_PickUp_Data(1, 36);
        }
        /// <summary>
        /// 飞行模式高八位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_37(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 3;
            TChart4.Series[0].Clear();
            TChart4.Zoom.Undo();
            TChart4.Header.Text = "FlightModelH";//Tchart窗体标题
            TChart4.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(1, 237);
        }
        /// <summary>
        /// 飞行模式低八位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_38(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 3;
            TChart4.Series[0].Clear();
            TChart4.Zoom.Undo();
            TChart4.Header.Text = "FlightModelL";//Tchart窗体标题
            TChart4.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(1, 337);
        }
        /// <summary>
        /// IMU_State
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_39(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 4;
            TChart5.Series[0].Clear();
            TChart5.Zoom.Undo();
            TChart5.Header.Text = "IMU_State";//Tchart窗体标题
            TChart5.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(1, 38);
        }
        /// <summary>
        /// Pos_Number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_40(object sender, RoutedEventArgs e)
        {
            whitch_line = 32;
            if (m_state_posnum.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Pos_Number";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 39);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Pos_Number";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 39);
        }
        /// <summary>
        /// RTK参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_41(object sender, RoutedEventArgs e)
        {
            whitch_line = 33;
            if (m_state_RTK.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Health_State";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 40);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Health_State";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 40);
        }
        /// <summary>
        /// 机型设置相关参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_42(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 1;
            TChart2.Series[0].Clear();
            TChart2.Zoom.Undo();
            TChart2.Header.Text = "Frame_Type";//Tchart窗体标题
            TChart2.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(2, 0);
        }
        /// <summary>
        /// 任务包组数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_43(object sender, RoutedEventArgs e)
        {
            whitch_line = 34;
            if (m_group.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Group";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 8);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Group";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 8);


        }
        /// <summary>
        /// 任务点个数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_44(object sender, RoutedEventArgs e)
        {

            whitch_line = 35;
            if (m_size.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Size";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 9);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Size";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 9);
        }
        /// <summary>
        /// 当前第几个任务点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_45(object sender, RoutedEventArgs e)
        {
            whitch_line = 36;
            if (m_num.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Number";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 10);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Number";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 10);
        }
        /// <summary>
        /// 原始滚转角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_46(object sender, RoutedEventArgs e)
        {
            whitch_line = 37;
            if (m_attitude_control_roll.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefRoll";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 22);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefRoll";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 22);
        }
        /// <summary>
        /// 增稳滚转角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_47(object sender, RoutedEventArgs e)
        {

            whitch_line = 38;
            if (m_stabil_roll.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "StabRoll";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 24);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "StabRoll";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 24);
        }
        /// <summary>
        /// 原始俯仰角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_48(object sender, RoutedEventArgs e)
        {
            whitch_line = 39;
            if (m_origin_pitch.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefPitch";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 23);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefPitch";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 23);
        }
        /// <summary>
        /// 增稳俯仰角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_49(object sender, RoutedEventArgs e)
        {
            whitch_line = 40;
            if (m_stabil_pitch.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "StabPitch";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 25);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "StabPitch";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 25);
        }
        /// <summary>
        /// 原始航向角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_50(object sender, RoutedEventArgs e)
        {

            whitch_line = 41;
            if (m_yaw_control_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefYaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 26);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefYaw";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 26);
        }
        /// <summary>
        /// 增稳航向角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_51(object sender, RoutedEventArgs e)
        {
            whitch_line = 42;
            if (m_yaw_control_stabyaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "StabYaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 28);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "StabYaw";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 28);
        }
        /// <summary>
        /// 反馈航向角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_52(object sender, RoutedEventArgs e)
        {
            whitch_line = 43;
            if (m_yaw_control_fbyaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "FBYaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 27);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "FBYaw";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 27);
        }
        /// <summary>
        /// 原始目标经度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_53(object sender, RoutedEventArgs e)
        {
            whitch_line = 44;
            if (m_position_control_lon.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefDestLon";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 17);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefDestLon";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 17);
        }
        /// <summary>
        /// 原始目标纬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_54(object sender, RoutedEventArgs e)
        {
            whitch_line = 45;
            if (m_position_control_lat.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefDestLat";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 18);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefDestLat";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 18);
        }
        /// <summary>
        /// 原始位置参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_55(object sender, RoutedEventArgs e)
        {

            whitch_line = 46;
            if (m_position_control_index.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefPos";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 19);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefPos";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 19);
        }
        /// <summary>
        /// 原始正北方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_56(object sender, RoutedEventArgs e)
        {

            whitch_line = 47;
            if (m_position_control_northvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefVn";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 20);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefVn";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 20);
        }
        /// <summary>
        /// 原始正东方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_57(object sender, RoutedEventArgs e)
        {
            whitch_line = 48;
            if (m_position_control_eastvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefVe";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 21);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefVe";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 21);
        }
        /// <summary>
        /// 原始高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_58(object sender, RoutedEventArgs e)
        {
            whitch_line = 49;
            if (m_height_control_height.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefAlt";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 32);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefAlt";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 32);
        }
        /// <summary>
        /// 原始垂直方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_59(object sender, RoutedEventArgs e)
        {
            whitch_line = 50;
            if (m_height_control_updownvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RefAltVel";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 33);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "RefAltVel";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 33);
        }
        /// <summary>
        /// 反馈垂直方向速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_60(object sender, RoutedEventArgs e)
        {

            whitch_line = 51;
            if (m_height_control_fbvel.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "FBVd";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 34);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "FBVd";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 34);
        }
        /// <summary>
        /// 增稳控制器油门量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_61(object sender, RoutedEventArgs e)
        {

            whitch_line = 52;
            if (m_height_control_stabthrottle.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "StabThro";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 35);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "StabThro";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 35);
        }
        /// <summary>
        /// 滚转方向控制量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_62(object sender, RoutedEventArgs e)
        {
            whitch_line = 53;
            if (m_motor_control_roll.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "MotorRoll";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 29);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "MotorRoll";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 29);
        }
        /// <summary>
        /// 俯仰方向控制量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_63(object sender, RoutedEventArgs e)
        {

            whitch_line = 54;
            if (m_motor_control_pitch.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "MotorPitch";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 30);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "MotorPitch";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 30);
        }
        /// <summary>
        /// 航向方向控制量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseLeftButtonDown_64(object sender, RoutedEventArgs e)
        {

            whitch_line = 55;
            if (m_motor_control_yaw.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "MotorYaw";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(2, 31);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }



            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "MotorYaw";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(2, 31);
        }

        private void TreeViewItem_PreviewMouseLeftButtonDown_65(object sender, RoutedEventArgs e)
        {

            whitch_line = 56;
            if (m_state_noise.IsChecked.Value)
            {

                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Zacc_Noise";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 46);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }


            //if (If_Load_Data_Success == false)
            //{
            //    if (m_language_choose.SelectedIndex == 0)
            //    {
            //        System.Windows.MessageBox.Show("请先载入FDR数据");
            //    }
            //    else if (m_language_choose.SelectedIndex == 1)
            //    {
            //        System.Windows.MessageBox.Show("First please load the FDR data");
            //    }
            //    return;
            //}
            //start_play_flag = 0;
            //m_tab_show.SelectedIndex = 0;
            //TChart1.Series[0].Clear();
            //TChart1.Zoom.Undo();
            //TChart1.Header.Text = "Zacc_Noise";//Tchart窗体标题
            //TChart1.Axes.Left.Title.Text = "";//底部标题  
            //Function_PickUp_Data(1, 46);
        }

        private void TreeViewItem_PreviewMouseLeftButtonDown_66(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 2;
            TChart3.Series[0].Clear();
            TChart3.Series[1].Clear();
            TChart3.Zoom.Undo();
            TChart3.Header.Text = "Version_Show";//Tchart窗体标题
            TChart3.Axes.Left.Title.Text = "";//底部标题  
            Function_PickUp_Data(2, 4);
        }



        private void TreeViewItem_PreviewMouseLeftButtonDown_67(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            one.Width = new GridLength(0.12, GridUnitType.Star);
            two.Width = new GridLength(0.01, GridUnitType.Star);
            three.Width = new GridLength(0.87, GridUnitType.Star);
            m_tab_show.SelectedIndex = 6;
            int a = L_add.Series.Count;
            if (a > 0)
            {
                L_add.Series.Clear();
                L_add.Series.Add(m_add);
            }
            else
            {
                L_add.Series.Add(m_add);
            }
            start_play_flag = 1;
            Work_Thread();
            track_play_count = 0;
            newdtb.Rows.Clear();
       
            //   Function_PickUp_Data(1, 777);
        }





        private void m_language_choose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_language_choose.SelectedIndex == 0)
            {
                List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
                foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                {
                    dictionaryList.Add(dictionary);
                }
                string requestedCulture = @"zh-cn.xaml";
                ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                Thickness thick = new Thickness(11, 0, 0, 0);
                m_imu_accvalue.Margin = thick;
                thick = new Thickness(11, 0, 0, 0);
                m_imu_attvalue.Margin = thick;
                thick = new Thickness(13, 0, 0, 0);
                m_imu_magvalue.Margin = thick;
                thick = new Thickness(11, 0, 0, 0);
                m_imu_gryovalue.Margin = thick;

                thick = new Thickness(10, 0, 0, 0);
                Btn_FlightModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Btn_BatteryModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Btn_TelModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Ay_UsedIfBack_Index.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                m_idle_value.Margin = thick;


                m_imu_attvalue.Content = "正常";
                if (Imu_Check_Acc == 1 || Imu_Check_Acc == 3)
                {
                    m_imu_accvalue.Content = "异常";
                }
                else
                {
                    m_imu_accvalue.Content = "正常";
                }
                if (Imu_Check_Mag == 1 || Imu_Check_Mag == 3)
                {
                    m_imu_magvalue.Content = "异常";
                }
                else
                {
                    m_imu_magvalue.Content = "正常";
                }
                if (Imu_Check_Gyro == 1 || Imu_Check_Gyro == 3)
                {
                    m_imu_gryovalue.Content = "异常";
                }
                else
                {
                    m_imu_gryovalue.Content = "正常";
                }


                switch (Flight_Style_Num)//机型相关的
                {
                    case 0://默认情况把值赋值为2
                        Btn_FlightModel_Choose.Content = "四轴叉字逆";
                        break;
                    case 1://4叉字顺时针
                        Btn_FlightModel_Choose.Content = "四轴叉字顺";
                        break;
                    case 2://4叉字逆时针
                        Btn_FlightModel_Choose.Content = "四轴叉字逆";
                        break;
                    case 3://4十字顺时针
                        Btn_FlightModel_Choose.Content = "四轴十字顺";
                        break;
                    case 4://4十字逆时针
                        Btn_FlightModel_Choose.Content = "四轴十字逆";
                        break;
                    case 5://6叉字顺时针
                        Btn_FlightModel_Choose.Content = "六轴叉字顺";
                        break;
                    case 6://6叉字逆时针
                        Btn_FlightModel_Choose.Content = "六轴叉字逆";
                        break;
                    case 7://6十字顺时针
                        Btn_FlightModel_Choose.Content = "六轴十字顺";
                        break;
                    case 8://6十字逆时针
                        Btn_FlightModel_Choose.Content = "六轴十字逆";
                        break;
                    case 9://8V字顺时针
                        Btn_FlightModel_Choose.Content = "八轴叉字顺";
                        break;
                    case 10://8V字逆时针
                        Btn_FlightModel_Choose.Content = "八轴叉字逆";
                        break;
                    case 11://8一字顺时针
                        Btn_FlightModel_Choose.Content = "八轴十字顺";
                        break;
                    case 12://8一字逆时针
                        Btn_FlightModel_Choose.Content = "八轴十字逆";
                        break;
                    case 13://共轴叉字顺时针
                        Btn_FlightModel_Choose.Content = "共轴叉字顺";
                        break;
                    case 14://共轴叉字逆时针
                        Btn_FlightModel_Choose.Content = "共轴叉字逆";
                        break;
                    default:
                        break;
                }
                switch (YKQType_Num)//遥控器类型
                {
                    case 0://14SG
                        Btn_TelModel_Choose.Content = "14SG";
                        break;
                    case 1://天地飞
                        Btn_TelModel_Choose.Content = "其它";
                        break;
                    default:
                        break;
                }
                switch (Battery_Num)//电池类型
                {
                    case 0://6s电池
                        Btn_BatteryModel_Choose.Content = "6S";
                        break;
                    case 1://12s电池
                        Btn_BatteryModel_Choose.Content = "12S";
                        break;
                    case 2://4s电池
                        Btn_BatteryModel_Choose.Content = "4S";
                        break;
                    case 3://3s电池
                        Btn_BatteryModel_Choose.Content = "3S";
                        break;
                    default:
                        break;
                }
                switch (IfReback_Num)//是否回中
                {
                    case 0://回中
                        Ay_UsedIfBack_Index.Content = "回中";
                        break;
                    case 1://不回中
                        Ay_UsedIfBack_Index.Content = "不回中";
                        break;
                    default:
                        break;
                }

                m_zitai_timer.Argument = "姿态时间";
                m_GPS_timer.Argument = "GPS时间";
                m_zizhu_timer.Argument = "自主时间";
                m_fanhang_timer.Argument = "返航使劲";
                m_zanting_timer.Argument = "暂停时间";
            }
            else
            {
                List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
                foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                {
                    dictionaryList.Add(dictionary);
                }
                string requestedCulture = @"en-us.xaml";
                ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                Thickness thick = new Thickness(68, 0, 0, 0);
                m_imu_accvalue.Margin = thick;
                thick = new Thickness(11, 0, 0, 0);
                m_imu_attvalue.Margin = thick;
                thick = new Thickness(12, 0, 0, 0);
                m_imu_magvalue.Margin = thick;
                thick = new Thickness(13, 0, 0, 0);
                m_imu_gryovalue.Margin = thick;

                thick = new Thickness(-10, 0, 0, 0);
                Btn_FlightModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Btn_BatteryModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Btn_TelModel_Choose.Margin = thick;
                thick = new Thickness(10, 0, 0, 0);
                Ay_UsedIfBack_Index.Margin = thick;
                thick = new Thickness(15, 0, 0, 0);
                m_idle_value.Margin = thick;



                m_imu_attvalue.Content = "Normal";
                if (Imu_Check_Acc == 1 || Imu_Check_Acc == 3)
                {
                    m_imu_accvalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_accvalue.Content = "Normal";
                }
                if (Imu_Check_Mag == 1 || Imu_Check_Mag == 3)
                {
                    m_imu_magvalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_magvalue.Content = "Normal";
                }
                if (Imu_Check_Gyro == 1 || Imu_Check_Gyro == 3)
                {
                    m_imu_gryovalue.Content = "Abnormal";
                }
                else
                {
                    m_imu_gryovalue.Content = "Normal";
                }


                switch (Flight_Style_Num)//机型相关的
                {
                    case 0://默认情况把值赋值为2
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Anticlockwise";
                        break;
                    case 1://4叉字顺时针
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Clockwise";
                        break;
                    case 2://4叉字逆时针
                        Btn_FlightModel_Choose.Content = "Four-rotors 'x'Anticlockwise";
                        break;
                    case 3://4十字顺时针
                        Btn_FlightModel_Choose.Content = "Four-rotors '+'Clockwise";
                        break;
                    case 4://4十字逆时针
                        Btn_FlightModel_Choose.Content = "Foue-rotors '+'Anticlockwise";
                        break;
                    case 5://6叉字顺时针
                        Btn_FlightModel_Choose.Content = "Six-rotors 'x'Clockwise";
                        break;
                    case 6://6叉字逆时针
                        Btn_FlightModel_Choose.Content = "Six-rotors 'x'Anticlockwise";
                        break;
                    case 7://6十字顺时针
                        Btn_FlightModel_Choose.Content = "Six-rotors '+'Clockwise";
                        break;
                    case 8://6十字逆时针
                        Btn_FlightModel_Choose.Content = "Six-rotors '+'Anticlockwise";
                        break;
                    case 9://8V字顺时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors 'x'Clockwise";
                        break;
                    case 10://8V字逆时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors 'x'Anticlockwise";
                        break;
                    case 11://8一字顺时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors '+'Clockwise";
                        break;
                    case 12://8一字逆时针
                        Btn_FlightModel_Choose.Content = "Eight-rotors '+'Anticlockwise";
                        break;
                    case 13://共轴叉字顺时针
                        Btn_FlightModel_Choose.Content = "Coaxial 'x'Clockwise";
                        break;
                    case 14://共轴叉字逆时针
                        Btn_FlightModel_Choose.Content = "Coaxial 'x'Anticlockwise";
                        break;
                    default:
                        break;
                }
                switch (YKQType_Num)//遥控器类型
                {
                    case 0://14SG
                        Btn_TelModel_Choose.Content = "14SG";
                        break;
                    case 1://天地飞
                        Btn_TelModel_Choose.Content = "Others";
                        break;
                    default:
                        break;
                }
                switch (Battery_Num)//电池类型
                {
                    case 0://6s电池
                        Btn_BatteryModel_Choose.Content = "6S";
                        break;
                    case 1://12s电池
                        Btn_BatteryModel_Choose.Content = "12S";
                        break;
                    case 2://4s电池
                        Btn_BatteryModel_Choose.Content = "4S";
                        break;
                    case 3://3s电池
                        Btn_BatteryModel_Choose.Content = "3S";
                        break;
                    default:
                        break;
                }
                switch (IfReback_Num)//是否回中
                {
                    case 0://回中
                        Ay_UsedIfBack_Index.Content = "Return middle";
                        break;
                    case 1://不回中
                        Ay_UsedIfBack_Index.Content = "Don't return middle";
                        break;
                    default:
                        break;
                }
                m_zitai_timer.Argument = "Posture Time";
                m_GPS_timer.Argument = "GPS Time";
                m_zizhu_timer.Argument = "Auto Time";
                m_fanhang_timer.Argument = "Back Time";
                m_zanting_timer.Argument = "Pause Time";
            }

        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else e.Effects = DragDropEffects.None;
        }
        void Grid_DroEvent(object sender, DragEventArgs e)
        {
            try
            {
                start_play_flag = 0;
                wait_count = 0;
                wait_flag = 1;
                Function_Wait();
                string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                Function_Load_Data(fileName);
            }
            catch (Exception)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请载入正确的数据文件");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("Please load the right file");
                }
            }

        }
        private void Button_Click_fangda(object sender, RoutedEventArgs e)
        {
            if (MainGMap.Zoom < 22)
            {
                MainGMap.Zoom = MainGMap.Zoom + 1;
            }
            
        }
        private void Button_Click_suoxiao(object sender, RoutedEventArgs e)
        {
            if (MainGMap.Zoom > 1)
            {
                MainGMap.Zoom = MainGMap.Zoom - 1;
            }
        }
        //liziran  新增
        private void TreeViewItem_PreviewMouseLeftButtonDown_68(object sender, RoutedEventArgs e)
        {
            whitch_line = 1;
            if (m_ins_veloity_vn.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Vn";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 30);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }
        
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_69(object sender, RoutedEventArgs e)
        {
            whitch_line = 2;
            if (m_ins_veloity_ve.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Ve";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 31);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_70(object sender, RoutedEventArgs e)
        {
            whitch_line = 3;
            if (m_ins_veloity_vd.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Vd";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 32);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_71(object sender, RoutedEventArgs e)
        {
            whitch_line = 4;
            if (m_ins_longitude.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Lon";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题  
                Function_PickUp_Data(4, 33);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_72(object sender, RoutedEventArgs e)
        {
            whitch_line = 5;
            if (m_ins_latitude.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Lat";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题  
                Function_PickUp_Data(4, 34);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_73(object sender, RoutedEventArgs e)
        {
            whitch_line = 6;
            if (m_ins_gps_alt.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Alt";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "m";//底部标题  
                Function_PickUp_Data(4, 35);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_74(object sender, RoutedEventArgs e)
        {
            whitch_line = 63;
            if (m_ublox_veloity_vn.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Vn";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 6);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_75(object sender, RoutedEventArgs e)
        {
            whitch_line = 64;
            if (m_ublox_veloity_ve.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Ve";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 7);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_76(object sender, RoutedEventArgs e)
        {
            whitch_line = 65;
            if (m_ublox_veloity_vd.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Vd";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "vel(m/s)";//底部标题  
                Function_PickUp_Data(4, 8);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_77(object sender, RoutedEventArgs e)
        {
            whitch_line = 66;
            if (m_ublox_longitude.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Lon";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题  
                Function_PickUp_Data(4, 9);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_78(object sender, RoutedEventArgs e)
        {
            whitch_line = 67;
            if (m_ublox_latitude.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Lat";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "degree(°)";//底部标题  
                Function_PickUp_Data(4, 10);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_79(object sender, RoutedEventArgs e)
        {
            whitch_line = 68;
            if (m_ublox_gps_alt.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Alt";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "m";//底部标题  
                Function_PickUp_Data(4, 11);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_80(object sender, RoutedEventArgs e)
        {
            whitch_line = 69;
            if (m_ublox_star_num.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Star";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 12);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_81(object sender, RoutedEventArgs e)
        {
            whitch_line = 70;
            if (m_ublox_pdop.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Pdop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 13);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_82(object sender, RoutedEventArgs e)
        {
            whitch_line = 71;
            if (m_ublox_hdop.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Hdop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 14);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_83(object sender, RoutedEventArgs e)
        {
            whitch_line = 72;
            if (m_ublox_vdop.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Vdop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 15);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_84(object sender, RoutedEventArgs e)
        {
            whitch_line = 73;
            if (m_ublox_work_state.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_State";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 16);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_85(object sender, RoutedEventArgs e)
        {
            whitch_line = 74;
            if (m_ublox_year.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "Ublox_Year";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 17);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_86(object sender, RoutedEventArgs e)
        {
            whitch_line = 1;
            if (m_ublox_month.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Star";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 36);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_87(object sender, RoutedEventArgs e)
        {
            whitch_line = 2;
            if (m_ublox_day.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_pDop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 37);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_88(object sender, RoutedEventArgs e)
        {
            whitch_line = 3;
            if (m_ublox_hour.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_hDop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 38);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_89(object sender, RoutedEventArgs e)
        {
            whitch_line = 4;
            if (m_ublox_minute.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_gDop";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 39);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_90(object sender, RoutedEventArgs e)
        {
            whitch_line = 5;
            if (m_ublox_second.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_Sol";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 41);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_91(object sender, RoutedEventArgs e)
        {
            whitch_line = 6;
            if (m_ublox_sol.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 0;
                //TChart1.Series[0].Clear();
                TChart1.Zoom.Undo();
                TChart1.Header.Text = "RTK_PosType";//Tchart窗体标题
                TChart1.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(4, 42);
            }
            else
            {
                TChart1.Series[whitch_line].Clear();
            }

        }
       /* private void TreeViewItem_PreviewMouseLeftButtonDown_92(object sender, RoutedEventArgs e)
        {
            error_message = 1;
            whitch_line =0;
            if (m_disconnect.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
               
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 7;
                //TChart1.Series[0].Clear();
                TChart7.Zoom.Undo();
                TChart7.Header.Text = "GPS 0:Normal 1:Abnormal";//Tchart窗体标题
                TChart7.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart7.Series[whitch_line].Clear();
            }
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_93(object sender, RoutedEventArgs e)
        {
            error_message = 2;
            whitch_line = 1;
            if (m_mag2.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
               
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 7;
                //TChart1.Series[0].Clear();
                TChart7.Zoom.Undo();
                TChart7.Header.Text = "Mag 0:Normal 1:Abnormal";//Tchart窗体标题
                TChart7.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart7.Series[whitch_line].Clear();
            }
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_94(object sender, RoutedEventArgs e)
        {
            error_message = 3;
            whitch_line = 2;
            if (m_control.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
               
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 7;
                //TChart1.Series[0].Clear();
                TChart7.Zoom.Undo();
                TChart7.Header.Text = "Remote Control 0:Normal 1:Abnormal";//Tchart窗体标题
                TChart7.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart7.Series[whitch_line].Clear();
            }
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_95(object sender, RoutedEventArgs e)
        {
            error_message = 4;
            whitch_line = 3;
            if (m_battery.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
               
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 7;
                //TChart1.Series[0].Clear();
                TChart7.Zoom.Undo();
                TChart7.Header.Text = "Battery 0:Normal 1:Low 2:Danger";//Tchart窗体标题
                TChart7.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart7.Series[whitch_line].Clear();
            }
        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_96(object sender, RoutedEventArgs e)
        {
            error_message = 5;
            whitch_line = 4;
            if (m_star.IsChecked.Value)
            {
                if (If_Load_Data_Success == false)
                {
                    if (m_language_choose.SelectedIndex == 0)
                    {
                        System.Windows.MessageBox.Show("请先载入FDR数据");
                    }
                    else if (m_language_choose.SelectedIndex == 1)
                    {
                        System.Windows.MessageBox.Show("First please load the FDR data");
                    }
                    return;
                }
                
                start_play_flag = 0;
                m_tab_show.SelectedIndex = 7;
                //TChart1.Series[0].Clear();
                TChart7.Zoom.Undo();
                TChart7.Header.Text = "Star 0:No 1:Ordinary 2:Good";//Tchart窗体标题
                TChart7.Axes.Left.Title.Text = "";//底部标题  
                Function_PickUp_Data(1, 34);
            }
            else
            {
                TChart7.Series[whitch_line].Clear();
            }

        }
        private void TreeViewItem_PreviewMouseLeftButtonDown_97(object sender, MouseButtonEventArgs e)
        {
            if (If_Load_Data_Success == false)
            {
                if (m_language_choose.SelectedIndex == 0)
                {
                    System.Windows.MessageBox.Show("请先载入FDR数据");
                }
                else if (m_language_choose.SelectedIndex == 1)
                {
                    System.Windows.MessageBox.Show("First please load the FDR data");
                }
                return;
            }
            start_play_flag = 0;
            m_tab_show.SelectedIndex = 7;

            error_message = 1;
            whitch_line = 0;
            Function_PickUp_Data2(1, 34);

            error_message = 2;
            whitch_line = 1;
            Function_PickUp_Data2(1, 34);
            error_message = 3;
            whitch_line = 2;
            Function_PickUp_Data2(1, 34);
            error_message = 4;
            whitch_line = 3;
            Function_PickUp_Data2(1, 34);
            error_message = 5;
            whitch_line = 4;
            Function_PickUp_Data2(1, 34);
            TChart7.Series[0].Clear();
            TChart7.Series[1].Clear();
            TChart7.Series[2].Clear();
            TChart7.Series[3].Clear();
            TChart7.Series[4].Clear();
        }*/

     
        
    }
}
