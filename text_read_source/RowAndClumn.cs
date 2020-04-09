using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text_read_source
{
    public class RowAndClumn
    {
        public Int64 m_row_value = 0;
        public Int64 m_column_value = 0;
    }
    public class Model_Flight_Time
    {
        public double Attitude_Flight_Time = 0;//姿态飞行时间
        public double GPS_Flight_Time = 0;//GPS飞行时间
        public double Auto_Flight_Time = 0;//路点飞行时间
        public double Reback_Flight_Time = 0;//返航飞行时间
        public double Suspend_Flight_Time = 0;//暂停时间
    }
}
/**********************data1每一列数据*****************************************************/
////////姿态角数据：0-roll;1-pitch;2-yaw;12-proproll;13-proppitch;14-propyaw/////////////////////////////
////////传感器数据：3-rollrate;4-pitchrate;5-yawrate;6-xacc;7-yacc;8-zacc;9-magx;10-magy;11-magz/////////
////////遥控器量: 16-switchA;17-switchB;18-switchC;19-switchD;15-prop-thro///////////////////////////////
////////电机量：20-m1;21-m2;22-m3;23-m4;24-m5;25-m6//////////////////////////////////////////////////////
////////GPS数据1：26-GPS_Vn;27-GPS_Ve;28-GPS_Vd;29-GPS_Lon;30-GPS_Lat;31-GPS_Alt/////////////////////////
////////GPS数据2：32-GPS_Heading;33-GPS_StarNum;34-GPS_Dop///////////////////////////////////////////////
////////气压计:35-altitude_ori///////////////////////////////////////////////////////////////////////////
////////状态值:36-battery_volt;37-flightmode;38-IMU_State;39-pos_number;40-Health_State2;41-pos_x;42-pos_y;43-pos_z
/**********************data1每一列数据*****************************************************/

/**********************data2每一列数据*****************************************************/
/////////FrameType:0-frametype///////////////////////////////////////////////////////////////////////////
/////////GroupSizeNum数据读取:8-group;9-sizepo;10-num////////////////////////////////////////////////////
/////////姿态控制参数:17-RefDestLon;18-RefDestLat;19-RefPos;20-RefVn;21-RefVe;22-RefRoll;23-RefPitch;24-StabRoll;25-StabPitch;26-RefYaw;27-FBYaw;28-StabYaw
/////////高度控制参数:29-MotorRoll;30-MotorPitch;31-MotorYaw;32-RefAlt;33-RefAltVel;34-FBVd;35-StabThro/////////////////////////
/**********************data2每一列数据*****************************************************/
