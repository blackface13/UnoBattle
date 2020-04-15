using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Code._4.CORE.UnoCard
{
    public class UnoCardSettings : MonoBehaviour
    {
        public Text[] TextUI;
        public GameObject[] ObjectController;
        public Image CurentColor;

        private void Start()
        {
            SetupTextUI();
            GetParameterSetting();
            CurentColor.color = new Color32(GameSystem.UserPlayer.UnoBGColorR, GameSystem.UserPlayer.UnoBGColorG, GameSystem.UserPlayer.UnoBGColorB, 1);
            ObjectController[3].GetComponent<Image>().color = new Color32(GameSystem.UserPlayer.UnoBGColorR, GameSystem.UserPlayer.UnoBGColorG, GameSystem.UserPlayer.UnoBGColorB, 255);
            ADS.HideBanner();
        }

        /// <summary>
        /// Gán ngôn ngữ
        /// </summary>
        private void SetupTextUI()
        {
            TextUI[0].text = Languages.lang[309];// = "Style Options";
            TextUI[1].text = Languages.lang[310];// = "Fast select card";
            TextUI[2].text = Languages.lang[320];// = "Background color";
            TextUI[3].text = Languages.lang[67];// = "Cancel";
            TextUI[4].text = Languages.lang[323];// = "Fast pass round";
            TextUI[5].text = Languages.lang[324];// = "Hỗ trợ hiển thị lá bài được đánh";
            TextUI[6].text = Languages.lang[59];// = "ngôn ngữ";
        }

        /// <summary>
        /// Lấy dữ liệu setting
        /// </summary>
        private void GetParameterSetting()
        {
            ObjectController[1].SetActive(GameSystem.UserPlayer.UnoSettingFastPush);
            ObjectController[4].SetActive(GameSystem.UserPlayer.UnoSettingFastPass);
            ObjectController[5].SetActive(GameSystem.UserPlayer.UnoSettingImgSupport);
        }

        /// <summary>
        /// Các hàm chung
        /// </summary>
        public void GeneralFunctions(int type)
        {
            switch (type)
            {
                case 0://Đóng form
                    //DataUserController.SaveUserInfor();
                    ObjectController[0].SetActive(false);
                    ADS.RequestBanner(0);
                    break;
                case 1://Đóng UI color picker
                    ObjectController[2].SetActive(false);
                    break;
                case 2://Lưu và đóng UI color picker
                    Color32 getColor = new Color(CurentColor.color.r, CurentColor.color.g, CurentColor.color.b);
                    GameSystem.UserPlayer.UnoBGColorR = getColor.r;
                    GameSystem.UserPlayer.UnoBGColorG = getColor.g;
                    GameSystem.UserPlayer.UnoBGColorB = getColor.b;
                    ObjectController[3].GetComponent<Image>().color = new Color32(GameSystem.UserPlayer.UnoBGColorR, GameSystem.UserPlayer.UnoBGColorG, GameSystem.UserPlayer.UnoBGColorB, 255);
                    ObjectController[2].SetActive(false);
                    break;
                case 5://Chức năng Fast push
                    GameSystem.UserPlayer.UnoSettingFastPush = !GameSystem.UserPlayer.UnoSettingFastPush;
                    GetParameterSetting();
                    break;
                case 6://Chức năng chọn màu
                    ObjectController[2].SetActive(true);
                    break;
                case 7://Chức năng Fast pass
                    GameSystem.UserPlayer.UnoSettingFastPass = !GameSystem.UserPlayer.UnoSettingFastPass;
                    GetParameterSetting();
                    break;
                case 8://Chức năng hỗ trợ hiển thị
                    GameSystem.UserPlayer.UnoSettingImgSupport = !GameSystem.UserPlayer.UnoSettingImgSupport;
                    GetParameterSetting();
                    break;
            }
        }

        public void SettingLanguage(int type)
        {
            GameSystem.Settings.Language = type;
            Languages.SetupLanguage(type);
            GameSystem.SaveUserData();

            SetupTextUI();
        }
    }
}
