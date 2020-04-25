using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;
using UnityEngine.UI;
using Assets.Code._0.DTO.Models;
using Assets.Code._2.BUS.Misc;
using Sirenix.OdinInspector;
using StartApp;

public class HomeController : MonoBehaviour
{
    #region Variables
    [TabGroup("Scene setting")]
    [LabelText("Hiệu ứng di chuyển object")]
    public AnimationCurve moveCurve;
    [LabelText("Scene setting")]
    [TabGroup("Scene setting")]
    public Text[] TextUI;
    [LabelText("Các object")]
    [TabGroup("Scene setting")]
    public GameObject[] ObjectController;

    [TabGroup("Information setting")]
    [Title("Cài đặt thông số")]
    [LabelText("Khoảng cách di chuyển các box nhỏ")]
    public float RangeMoveInformationBox;
    [TabGroup("Information setting")]
    [LabelText("Thời gian di chuyển box")]
    public float TimeMoveContentBox;
    [TabGroup("Information setting")]
    [LabelText("Nút lùi")]
    public Button BtnInforBoxPrev;
    [TabGroup("Information setting")]
    [LabelText("Nút tới")]
    public Button BtnInforBoxNext;
    [TabGroup("Information setting")]
    [Title("Thông tin về điểm")]
    [LabelText("Tiêu đề điểm cơ bản")]
    public Text BasicPointTitle;
    [TabGroup("Information setting")]
    [LabelText("Điểm cơ bản")]
    public Text BasicPointText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề điểm mở rộng")]
    public Text ExtensionPointTitle;
    [TabGroup("Information setting")]
    [LabelText("Điểm mở rộng")]
    public Text ExtensionPointText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề tổng điểm")]
    public Text TotalPointTitle;
    [TabGroup("Information setting")]
    [LabelText("Tổng điểm")]
    public Text TotalPointText;

    [TabGroup("Information setting")]
    [Title("Thông tin về chế độ cơ bản")]
    [LabelText("Tiêu đề thắng cơ bản")]
    public Text BasicWinTitle;
    [TabGroup("Information setting")]
    [LabelText("Số lần thắng")]
    public Text BasicWinText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề thua cơ bản")]
    public Text BasicLoseTitle;
    [TabGroup("Information setting")]
    [LabelText("Số lần thua")]
    public Text BasicLoseText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề tỉ lệ")]
    public Text BasicRatio;
    [TabGroup("Information setting")]
    [LabelText("Số tỉ lệ")]
    public Text BasicRatioText;

    [TabGroup("Information setting")]
    [Title("Thông tin về chế độ mở rộng")]
    [LabelText("Tiêu đề thắng mở rộng")]
    public Text ExtensionWinTitle;
    [TabGroup("Information setting")]
    [LabelText("Số lần thắng")]
    public Text ExtensionWinText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề thua mở rộng")]
    public Text ExtensionLoseTitle;
    [TabGroup("Information setting")]
    [LabelText("Số lần thua")]
    public Text ExtensionLoseText;
    [TabGroup("Information setting")]
    [LabelText("Tiêu đề tỉ lệ")]
    public Text ExtensionRatio;
    [TabGroup("Information setting")]
    [LabelText("Số tỉ lệ")]
    public Text ExtensionRatioText;

    [Title("Các object khác")]
    [TabGroup("Information setting")]
    [LabelText("Bảng điểm")]
    public GameObject ObjectInforBoxPoint;
    [TabGroup("Information setting")]
    [LabelText("Bảng chế độ cơ bản")]
    public GameObject ObjectInforBoxBasic;
    [TabGroup("Information setting")]
    [LabelText("Bảng chế độ mở rộng")]
    public GameObject ObjectInforBoxExtension;

    private int CurentInforBox;//Box thống kê hiện tại đang xem, 0=point, 1=basicmode, 2=ExtensionMode
    private bool IsEnableClickMoveInforBox = true;//Cho phép click di chuyển box hay ko

    private float RankingRangeExpand = 732f;
    private float RankingPositionOriginal;
    private bool IsExpand = false;
    private List<GameObject> ObjectRanking = new List<GameObject>();
    #endregion

    #region Initialize
    void Start()
    {
        AdSdk.Instance.ShowSplash();
        ADS.ShowBanner();
        //ADS.RequestBanner(0);
        GameSystem.AnimCurve = moveCurve;

        #region Khởi tạo hoặc set Canvas thông báo cho Scene 
        try
        {
            GameSystem.MessageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        catch
        {
            GameSystem.Initialize(); //Khởi tạo này dành cho scene nào test ngay
            GameSystem.MessageCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            GameSystem.MessageCanvas.GetComponent<Canvas>().planeDistance = 1;
        }
        #endregion

        SetupText();
        UpdatePoint();
        SetupInformationBox();
        SetupButtonInforBox();
        if (GameSystem.UserPlayer.IsLoginFB)
        {
            BtnFBLogin();
        }
        RankingPositionOriginal = ObjectController[3].transform.localPosition.x;

        //Ranking
        StartCoroutine(API.APIGetRanking(API.ServerRankingGetGlobal));
        StartCoroutine(ShowRanking());

        //Nhập tên nếu tên đang trống
        if (string.IsNullOrEmpty(GameSystem.UserPlayer.UserName))
        {
            GameSystem.InitializePrefabUI(3, "InputNameCanvasUI");
        }

        TextUI[0].text = GameSystem.UserPlayer.UserName;
    }

    /// <summary>
    /// Set giao diện ngôn ngữ
    /// </summary>
    private void SetupText()
    {
        TextUI[2].text = Languages.lang[316];//Ranking
        BasicPointTitle.text = Languages.lang[356];
        ExtensionPointTitle.text = Languages.lang[357];
        TotalPointTitle.text = Languages.lang[358];

        BasicWinTitle.text = Languages.lang[359]; //"Thắng chế độ cơ bản: ";
        BasicLoseTitle.text = Languages.lang[360];//"Thua chế độ cơ bản: ";
        BasicRatio.text = Languages.lang[361];    //"Tỉ lệ: ";

        ExtensionWinTitle.text = Languages.lang[362]; //"Thắng chế độ cơ bản: ";
        ExtensionLoseTitle.text = Languages.lang[363];//"Thua chế độ cơ bản: ";
        ExtensionRatio.text = Languages.lang[361];    //"Tỉ lệ: ";
    }

    /// <summary>
    /// Khởi tạo và cài đặt box thống kê
    /// </summary>
    private void SetupInformationBox()
    {
        ObjectInforBoxPoint.transform.localPosition = Vector3.zero;
        ObjectInforBoxBasic.transform.localPosition = new Vector3(RangeMoveInformationBox, 0, 0);
        ObjectInforBoxExtension.transform.localPosition = new Vector3(RangeMoveInformationBox, 0, 0);
    }

    /// <summary>
    /// Cài đặt nút bấm inforbox
    /// </summary>
    private void SetupButtonInforBox()
    {
        BtnInforBoxPrev.onClick.AddListener(() =>
        {
            ButtonChangeInforBox(-1);
        });
        BtnInforBoxNext.onClick.AddListener(() =>
        {
            ButtonChangeInforBox(1);
        });
    }
    #endregion

    #region Functions
    /// <summary>
    /// Set điểm
    /// </summary>
    private void UpdatePoint()
    {
        //Điểm cơ bản
        BasicPointText.text = GameSystem.UserPlayer.UnoBasicPoint > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoBasicPoint)) : 0.ToString();
        //Điểm mở rộng
        ExtensionPointText.text = GameSystem.UserPlayer.UnoExtensionPoint > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoExtensionPoint)) : 0.ToString();
        //Tổng điểm
        TotalPointText.text = (GameSystem.UserPlayer.UnoBasicPoint + GameSystem.UserPlayer.UnoExtensionPoint > 0 ? (string.Format("{0:#,#}", (GameSystem.UserPlayer.UnoBasicPoint + GameSystem.UserPlayer.UnoExtensionPoint))) : 0.ToString());
        //Win chế độ cơ bản
        BasicWinText.text = GameSystem.UserPlayer.UnoBasicWinRound > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoBasicWinRound)) : 0.ToString();
        //Thua chế độ cơ bản
        BasicLoseText.text = GameSystem.UserPlayer.UnoBasicLoseRound > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoBasicLoseRound)) : 0.ToString();
        //Tỉ lệ win chế độ cơ bản
        BasicRatioText.text = (100 / (GameSystem.UserPlayer.UnoBasicWinRound + GameSystem.UserPlayer.UnoBasicLoseRound) * GameSystem.UserPlayer.UnoBasicWinRound).ToString() + "%";
        //Win chế độ mở rộng
        ExtensionWinText.text = GameSystem.UserPlayer.UnoExtensionWinRound > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoExtensionWinRound)) : 0.ToString();
        //Thua chế độ mở rộng
        ExtensionLoseText.text = GameSystem.UserPlayer.UnoExtensionLoseRound > 0 ? (string.Format("{0:#,#}", GameSystem.UserPlayer.UnoExtensionLoseRound)) : 0.ToString();
        //Tỉ lệ win chế độ mở rộng
        ExtensionRatioText.text = (100 / (GameSystem.UserPlayer.UnoExtensionWinRound + GameSystem.UserPlayer.UnoExtensionLoseRound) * GameSystem.UserPlayer.UnoExtensionWinRound).ToString() + "%";
    }

    public void GeneralFunctions(int type)
    {
        switch (type)
        {
            case 0://UI chơi game
                GameSystem.InitializePrefabUI(0, "UnoCardCanvasUI");
                StartCoroutine(WaitingCloseUI(0)); //Chờ đóng UI
                break;
            case 1://UI setting
                ADS.HideBanner();
                ObjectController[2].SetActive(true);
                StartCoroutine(WaitingCloseUI(1)); //Chờ đóng UI
                break;
            case 2://UI setting
                //ADS.RequestInterstitial();
                break;
            case 3://Introduction
                GameSystem.InitializePrefabUI(1, "UnoIntroductionCanvasUI");
                break;
            default: break;
        }
    }

    /// <summary>
    /// Hiển thị xếp hạng
    /// </summary>
    private IEnumerator ShowRanking()
    {
        yield return new WaitUntil(() => API.APIState != API.State.Waiting);
        if (API.APIState == API.State.Success)
        {
            GameSystem.DisposeAllObjectChild(ObjectController[5]);
            ObjectRanking.Clear();

            var count = GameSystem.GlobalRankingList.Count;
            for (int i = 0; i < count; i++)
            {
                ObjectRanking.Add((GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/UI/RankingUI"), ObjectController[5].transform.position, Quaternion.identity));
                ObjectRanking[i].transform.SetParent(ObjectController[5].transform, false);
                ObjectRanking[i].GetComponent<RankingUI>().Initialize((i + 1).ToString(), GameSystem.GlobalRankingList[i].userName, (GameSystem.GlobalRankingList[i].unoBasicPoint + GameSystem.GlobalRankingList[i].unoExtensionPoint).ToString());
                ObjectRanking[i].transform.localPosition = new Vector3(-7f, 0 - ObjectRanking[i].GetComponent<RectTransform>().sizeDelta.y * i, 0);
            }

            //Set lại chiều cao của bảng ranking
            ObjectController[5].GetComponent<RectTransform>().sizeDelta = new Vector2(ObjectController[5].GetComponent<RectTransform>().sizeDelta.x, count * ObjectRanking[0].GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    private IEnumerator WaitingCloseUI(int type)
    {
        switch (type)
        {
            case 0://Chơi game
                yield return new WaitUntil(() => GameSystem.ObjectUI[type] == null);
                //Success
                if (GameSystem.ObjectUI[type] == null)
                {
                    TextUI[1].text = Languages.lang[260] + ((GameSystem.UserPlayer.UnoBasicPoint + GameSystem.UserPlayer.UnoExtensionPoint) != 0 ? (string.Format("{0:#,#}", (GameSystem.UserPlayer.UnoBasicPoint + GameSystem.UserPlayer.UnoExtensionPoint))) : 0.ToString());
                    StartCoroutine(API.APIGetRanking(API.ServerRankingGetGlobal));
                    StartCoroutine(ShowRanking());
                    UpdatePoint();
                }
                break;
            case 1://Setting
                yield return new WaitUntil(() => !ObjectController[2].activeSelf);
                //Success
                if (!ObjectController[2].activeSelf)
                {
                    GameSystem.SaveUserData();
                    SetupText();
                }
                break;
        }
    }

    /// <summary>
    /// Open bảng ranking
    /// </summary>
    public void ViewRanking()
    {
        IsExpand = !IsExpand;
        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectController[3], ObjectController[3].transform.localPosition, new Vector2(IsExpand ? RankingPositionOriginal + RankingRangeExpand : RankingPositionOriginal, ObjectController[3].transform.localPosition.y), .5f, moveCurve));
        ObjectController[4].transform.localScale = new Vector3(1, IsExpand ? -1 : 1, 1);
    }

    /// <summary>
    /// Chuyển đổi chế độ xem thống kê box
    /// </summary>
    public void ButtonChangeInforBox(int valueInput)
    {
        var temp = CurentInforBox;
        var totalContentBox = 3;//Số lượng box thống kê
        if (IsEnableClickMoveInforBox)
        {
            CurentInforBox += valueInput;
            if (CurentInforBox < 0)
                CurentInforBox = 0;
            if (CurentInforBox > totalContentBox - 1)
                CurentInforBox = totalContentBox - 1;
            if (!CurentInforBox.Equals(temp))
            {
                switch (CurentInforBox)
                {
                    case 0:
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxPoint, ObjectInforBoxPoint.transform.localPosition, new Vector2(0, 0), TimeMoveContentBox, moveCurve));
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxBasic, ObjectInforBoxBasic.transform.localPosition, new Vector2(RangeMoveInformationBox, 0), TimeMoveContentBox, moveCurve));
                        break;
                    case 1:
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxPoint, ObjectInforBoxPoint.transform.localPosition, new Vector2(0 - RangeMoveInformationBox, 0), TimeMoveContentBox, moveCurve));
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxBasic, ObjectInforBoxBasic.transform.localPosition, new Vector2(0, 0), TimeMoveContentBox, moveCurve));
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxExtension, ObjectInforBoxExtension.transform.localPosition, new Vector2(RangeMoveInformationBox, 0), TimeMoveContentBox, moveCurve));
                        break;
                    case 2:
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxExtension, ObjectInforBoxExtension.transform.localPosition, new Vector2(0, 0), TimeMoveContentBox, moveCurve));
                        StartCoroutine(GameSystem.MoveObjectCurve(true, ObjectInforBoxBasic, ObjectInforBoxBasic.transform.localPosition, new Vector2(0 - RangeMoveInformationBox, 0), TimeMoveContentBox, moveCurve));
                        break;
                    default: break;
                }
                WaitForEnableMoveBox();
            }
        }
    }

    /// <summary>
    /// Chờ đợi box thống kê move xong mới cho click move tiếp
    /// </summary>
    private IEnumerator WaitForEnableMoveBox()
    {
        IsEnableClickMoveInforBox = false;
        yield return new WaitForSeconds(TimeMoveContentBox);
        IsEnableClickMoveInforBox = true;
    }
    #endregion

    #region FB login
    public void BtnFBLogin()
    {
        if (!FB.IsInitialized)
        {
            //IsLoginFB = true;
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            StartCoroutine(LoginingFB());
            // Already initialized, signal an app activation App Event
            //FB.ActivateApp();
            //List<string> perms = new List<string>() { "public_profile", "email" };
            //FB.LogInWithReadPermissions(perms, AuthCallback);
            //FB.API("me?fields=id", Facebook.Unity.HttpMethod.GET, GetFacebookID);
            //FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
            //FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, FbGetPicture);
            //GameSystem.ControlFunctions.ShowMessage("2");
        }
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            //Debug.Log(aToken.UserId);
            //str_string[0] = aToken.UserId;
            //IDictionary a = (Dictionary<string, object>)result.ResultDictionary["id"];

            //if (bol[2])
            //{
            //    if (mod.check_uname(str_string[0]) == "")//Kiểm tra ID đã reg acc chưa
            //    {
            //        str_string[1] = str_string[0];
            //        mod.reg_account(str_string[0], mod.random_key(10), "");//Reg acc dựa trên ID
            //        num_int[2] = 1;//type Reg acc
            //        bol[3] = true;
            //    }
            //    else
            //    {
            //        PlayerPrefs.SetString("uid", mod.enc(mod.id_member_temp, mod.key));//Lưu ID member
            //        obj[33].SetActive(true);//Show loading
            //        scene.Change_scene("Main");
            //        num_int[2] = 0;//Type loading = 0
            //    }
            //}
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                //Debug.Log(perm);
            }
        }
        else
        {
            //IsLoginFB = false;
            //Debug.Log("User cancelled login");
        }
    }
    void GetFacebookID(IResult result)
    {
        string fbID = result.ResultDictionary["id"].ToString();
        //string fbName = result.ResultDictionary["name"].ToString();
        GameSystem.UserPlayer.UserID = fbID;
        GameSystem.SaveUserData();
        //Debug.Log("fbName: " + result);
    }
    void GetFacebookName(IResult result)
    {
        string fbName = result.ResultDictionary["name"].ToString();
        GameSystem.UserPlayer.UserName = fbName;
        TextUI[0].text = fbName.ToString();
        ObjectController[0].SetActive(false);
        GameSystem.UserPlayer.IsLoginFB = true;
        GameSystem.SaveUserData();
    }

    //private IEnumerator UserImage()
    //{
    //    WWW url = new WWW("https" + "://graph.facebook.com/" + FB.UserId.ToString() + "/picture?type=large");
    //    Texture2D textFb2 = new Texture2D(128, 128, TextureFormat.DXT1, false); //TextureFormat must be DXT5
    //    yield return url;
    //    url.LoadImageIntoTexture(textFb2);
    //    UserImg = textFb2;
    //}

    /// <summary>
    /// Get hình ảnh facebook
    /// </summary>
    /// <param name="result"></param>
    private void FbGetPicture(IGraphResult result)
    {
        if (result.Texture != null)
            ObjectController[1].GetComponent<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        GameSystem.UserPlayer.IsLoginFB = true;
        GameSystem.SaveUserData();
    }

    private IEnumerator LoginingFB()
    {
        FB.ActivateApp();
        List<string> perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
        FB.API("me?fields=id", Facebook.Unity.HttpMethod.GET, GetFacebookID);
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
        FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, FbGetPicture);
        yield return null;
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            //FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            StartCoroutine(LoginingFB());
            //List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
            //List<string> perms = new List<string>() { "public_profile", "email" };
            //FB.LogInWithReadPermissions(perms, AuthCallback);
            //FB.API("me?fields=id", Facebook.Unity.HttpMethod.GET, GetFacebookID);
            //FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFacebookName);
            //FB.API("me/picture?type=square&height=128&width=128", HttpMethod.GET, FbGetPicture);
        }
        else
        {
            // bol[2] = false;
            //Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
            if (!GameSystem.UserPlayer.IsLoginFB)
            {
                GameSystem.UserPlayer.IsLoginFB = true;
                GameSystem.SaveUserData();
                var scnLoad = new SceneLoad();
                scnLoad.Change_scene("HomeScene");
            }
            // StartCoroutine(WaitGetData());
        }
    }

    private IEnumerator WaitGetData()
    {
        yield return new WaitForSeconds(2);

        if (ObjectController[0].activeSelf)
            BtnFBLogin();
    }
    #endregion
}
