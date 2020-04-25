using UnityEngine;
using System.Collections;
using StartApp;
using System;

public static class ADStartApp
{
	// Use this for initialization
	public static void ShowBanner()
	{
		AdSdk.Instance.ShowDefaultBanner(BannerAd.BannerPosition.Top);
	}
}
