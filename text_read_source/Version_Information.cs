using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace text_read_source
{
    class Version_Information
    {
        /*=========================================IMU_Version==============================*/
        public byte[] IMU_HardWare_Version = new byte[20];//IMU的硬件版本空间
        public byte[] IMU_SoftWare_Version = new byte[20];//IMU的软件版本空间
        public byte[] IMU_Equipment_ID = new byte[20];//IMU的ID号空间
        public String IMU_HardWare_VersionShow;
        /*=========================================AP_Version================================*/
        public byte[] AP_HardWare_Version = new byte[20];//AP的硬件版本空间
        public byte[] AP_SoftWare_Version = new byte[20];//AP的软件版本空间
        public byte[] AP_Equipment_ID = new byte[20];//AP的ID号空间
        public String AP_HardWare_VersionShow;
        public String AP_Equipment_ID_VersionShow;
        /*=========================================LED_Version================================*/
        public byte[] LED_HardWare_Version = new byte[20];//LED的硬件版本空间
        public byte[] LED_SoftWare_Version = new byte[20];//LED的软件版本空间
        public byte[] LED_Equipment_ID = new byte[20];//LED的ID号空间
        public String LED_HardWare_VersionShow;
        /*=========================================MAG_Version================================*/
        public byte[] MAG_HardWare_Version = new byte[20];//MAG的硬件版本空间
        public byte[] MAG_SoftWare_Version = new byte[20];//MAG的软件版本空间
        public byte[] MAG_Equipment_ID = new byte[20];//MAG的ID号空间
        public String MAG_HardWare_VersionShow;
        /*=========================================GPS_Version================================*/
        public byte[] GPS_HardWare_Version = new byte[20];//GPS的硬件版本空间
        public byte[] GPS_SoftWare_Version = new byte[20];//GPS的软件版本空间
        public byte[] GPS_Equipment_ID = new byte[20];//GPS的ID号空间
        public String GPS_HardWare_VersionShow;
        /*=========================================HUB_Version================================*/
        public byte[] HUB_HardWare_Version = new byte[20];//GPS的硬件版本空间
        public byte[] HUB_SoftWare_Version = new byte[20];//GPS的软件版本空间
        public byte[] HUB_Equipment_ID = new byte[20];//GPS的ID号空间
        public String HUB_HardWare_VersionShow;
        /*=========================================FDR_Version================================*/
        public byte[] FDR_HardWare_Version = new byte[20];//FDR的硬件版本空间
        public byte[] FDR_SoftWare_Version = new byte[20];//FDR的软件版本空间
        public byte[] FDR_Equipment_ID = new byte[20];//FDR的ID号空间
        public String FDR_HardWare_VersionShow;
        /*=========================================DTU_Version================================*/
        public byte[] DTU_HardWare_Version = new byte[20];//RTK的硬件版本空间
        public byte[] DTU_SoftWare_Version = new byte[20];//RTK的软件版本空间
        public byte[] DTU_Equipment_ID = new byte[20];//RTK的ID号空间
        public String DTU_HardWare_VersionShow;
        /*=========================================RTK_Version================================*/
        public byte[] RTK_HardWare_Version = new byte[20];//RTK的硬件版本空间
        public byte[] RTK_SoftWare_Version = new byte[20];//RTK的软件版本空间
        public byte[] RTK_Equipment_ID = new byte[20];//RTK的ID号空间
        public String RTK_HardWare_VersionShow;
    }
}
