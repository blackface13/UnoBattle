using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Code._0.DTO.Models
{
    [Serializable]
    public class Player
    {
        public string UserID ;
        public string UserName ;
        public string PassCode ;//Dùng để đổi thiết bị
        public double NumberSpined ; //Số lượt đã quay vòng quay may mắn
        public string HWID ;
        public string Country;
        public byte UnoBGColorR = 5;
        public byte UnoBGColorG = 0;
        public byte UnoBGColorB = 63;
        public bool UnoSettingFastPush;
        public bool UnoSettingFastPass;
        public bool UnoSettingImgSupport;
        public int UnoBasicWinRound;
        public int UnoBasicLoseRound;
        public int UnoBasicPoint;
        public int UnoExtensionWinRound;
        public int UnoExtensionLoseRound;
        public int UnoExtensionPoint;
        public bool IsLoginFB;
        public int UnoTypePlay;//Kiểu chơi cơ bản hoặc nâng cao
        public bool IsChangeDevice ;//Biến này xác định có chuyển thiết bị hay ko. = true thì ko cho phép đồng bộ lên, chỉ cho phép đồng bộ xuống, sau khi đồng bộ xuống thì đẩy lại dữ liệu lên và set = false. Nếu = false thì ko phải là trạng thái đăng ký chuyển thiết bị, trùng IDHW mới cho phép đồng bộ
    }
}
