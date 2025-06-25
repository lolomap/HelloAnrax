using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace YaAd
{
    public class BannerAdManager : MonoBehaviour
    {
        private Banner _banner;
        private BannerAdSize _bannerSize;

        public static string AdUnitId = "R-M-14387385-1";


        private void Awake()
        {
            RequestStickyBanner();
        }
        private static int GetScreenWidthDp()
        {
            int screenWidth = (int)Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }

        private void RequestStickyBanner()
        {
            _bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 50);
            _banner = new(AdUnitId, _bannerSize, AdPosition.TopCenter);

            _banner.OnAdLoaded += HandleAdLoaded;
            
            AdRequest request = new AdRequest.Builder().Build();
            _banner.LoadAd(request);
        }

        private void HandleAdLoaded(object sender, EventArgs args)
        {
            _banner.Show();
        }
    }
}

