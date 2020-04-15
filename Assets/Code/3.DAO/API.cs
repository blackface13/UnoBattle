/* Created: Bkack Face (bf.blackface@gmail.com)
 * API
 */
using Assets.Code._4.CORE;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using Assets.Code._0.DTO.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class API
{

    #region Khởi tạo variables 
    public static State APIState = State.LostConnected;
    public static string ServerRankingPut = "https://unobattle.xn--mten-1ua4066b.vn/ranking/putranking/";
    public static string ServerRankingGetGlobal = "https://unobattle.xn--mten-1ua4066b.vn/ranking/rankingglobal";
    public static string ServerRankingGetCountry = "https://unobattle.xn--mten-1ua4066b.vn/ranking/";
    public enum State
    {
        Waiting, //Trạng thái chờ kết nối server
        Success, //Thao tác thành công với server
        Failed, //Thao tác thất bại, hoặc server từ chối yêu cầu
        Connected, //Kết nối tới server thành công
        LostConnected, //Không thể kết nối tới server
        SyncFailedByHWID,//Không thể đồng bộ do đăng ký chơi trên thiết bị này nhưng đồng bộ trên thiết bị khác
    }
    #endregion

    public static IEnumerator APIGetRanking(string url)
    {
        APIState = State.Waiting;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                APIState = State.LostConnected;
                GameSystem.ControlFunctions.ShowMessage(Languages.lang[120]);//Ko ket noi dc toi server
                //GameSystem.ShowMessage(webRequest.error);
            }
            else
            {
                if (GameSystem.GlobalRankingList == null)
                    GameSystem.GlobalRankingList = new List<RankingModel>();

                GameSystem.GlobalRankingList.Clear();
                var result = webRequest.downloadHandler.text;
                if (!string.IsNullOrEmpty(result))
                {
                    result = result.Replace(@"[", "").Replace(@"]", "");
                    var listResult = Regex.Split(result, @"},{");
                    for (int i = 0; i < listResult.Length; i++)
                    {
                        if (!listResult[i].StartsWith("{"))
                            listResult[i] = "{" + listResult[i];
                        if (!listResult[i].EndsWith("}"))
                            listResult[i] = listResult[i] + "}";

                        GameSystem.GlobalRankingList.Add(new RankingModel());
                        GameSystem.GlobalRankingList[i] = JsonUtility.FromJson<RankingModel>(listResult[i]);
                    }
                }
                APIState = State.Success;
            }
        }
    }
    public static IEnumerator APIGet(string url)
    {
        //APIState = State.Waiting;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                APIState = State.LostConnected;
                GameSystem.ShowMessage(webRequest.error);
            }
            else
            {
                var result = webRequest.downloadHandler.text;
            }
        }
    }

    public static IEnumerator APIPut(string url, string jsonData)
    {
        //var inventoryData = new InventoryDataModel { UserID = DataUserController.User.UserID, HWID = SystemInfo.deviceUniqueIdentifier.ToString(), Data = Securitys.Encrypt(JsonUtility.ToJson(DataUserController.Inventory)), LastUpdate = 1234567 };
        //var jcon = JsonUtility.ToJson(inventoryData);

        UnityWebRequest www = UnityWebRequest.Put(url, jsonData);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            APIState = State.LostConnected;
            GameSystem.ShowMessage(www.error);
        }
        else
        {
            var result = www.downloadHandler.text;
            //var a = JsonUtility.FromJson<ResponseModel>(result);
            //Debug.Log(a.Res.ToString());
        }
    }

    public static IEnumerator APIPost(string url, string jsonData)
    {
        jsonData = "{\"userID\":\"fd45f\",\"userName\":null,\"golds\":0,\"gems\":50,\"inventorySlot\":0,\"battleWin\":0,\"battleLose\":0,\"numberSpined\":0,\"itemUserForBattle\":null,\"isAutoBattle\":false,\"enemyFutureMap\":null,\"difficultMap\":null,\"lastUpdate\":0,\"hwid\":null}";
        UnityWebRequest www = UnityWebRequest.Post("http://localhost:12345/player/tank/", "{\"player\":46546}");
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            APIState = State.LostConnected;
            GameSystem.ShowMessage(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}