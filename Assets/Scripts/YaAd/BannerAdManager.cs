using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;
public class BannerAdManager : MonoBehaviour
{
    private Banner banner;

    public static string AdUnitId;


    private void Start()
    {
        RequestStickyBanner();
    }
    private int GetScreenWidthDp()
    {
        int screenWidth = (int)Screen.safeArea.width;
        return ScreenUtils.ConvertPixelsToDp(screenWidth);
    }

    private void RequestStickyBanner()
    {
        BannerAdSize bannerMaxSize = BannerAdSize.StickySize(GetScreenWidthDp());
        banner = new Banner(AdUnitId, bannerMaxSize, AdPosition.BottomCenter);

        AdRequest request = new AdRequest.Builder().Build();
        banner.LoadAd(request);
    }
}

