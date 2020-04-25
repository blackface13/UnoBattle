using StartApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    [Header("Draw Curve")]
    public AnimationCurve moveCurve;
    private string SavePolicy = "AcceptPolicy";
    public GameObject[] ObjectController;
    SceneLoad ScnLoad = new SceneLoad();
    // Start is called before the first frame update
    void Start()
    {
        //AD StarApp
        AdSdk.Instance.SetUserConsent(
 "pas",
 true,
 (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);

        //AD Admob
        ADS.Initialize();
        ADS.RequestBanner(0);
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

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(WaitForShowPolicy());
    }

    private IEnumerator WaitForShowPolicy()
    {
        yield return new WaitUntil(() => !ObjectController[0].activeSelf); //Chờ logo xuất hiện xong

        if (string.IsNullOrEmpty(PlayerPrefs.GetString(SavePolicy))) //Nếu chưa đồng ý với điều khoản hoặc chơi game lần đầu
        {
            ObjectController[1].SetActive(true); //Show policy
        }
        else
        {
            //DataUserController.LoadAll();
            ScnLoad.Change_scene("HomeScene");
        }
        //GameSystem.ControlFunctions.ShowMessage("gfgf");
    }

    /// <summary>
    /// Không chấp nhập điều khoản, out game
    /// </summary>
    public void ButtonCancel()
    {
        Application.Quit();
    }
    /// <summary>
    /// Chấp nhận điều khoản
    /// </summary>
    public void ButtonTickAcceptPolicy()
    {
        ObjectController[2].SetActive(!ObjectController[2].activeSelf);
        ObjectController[3].SetActive(ObjectController[2].activeSelf);
    }

    public void Accept()
    {
        //DataUserController.LoadAll();
        PlayerPrefs.SetString(SavePolicy, "True");
        ScnLoad.Change_scene("HomeScene");
    }
}
