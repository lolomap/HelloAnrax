using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;
using TMPro;

public class RewardedAdManager : MonoBehaviour
{
    private RewardedAdLoader rewardedAdLoader;
    private RewardedAd rewardedAd;

    public int coins;
    public TextMeshProUGUI coinsText;

    private void Awake()
    {
        SetupLoader();
        RequestRewardedAd();
    }

    private void SetupLoader()
    {
        rewardedAdLoader = new RewardedAdLoader();
        rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
        rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
    }

    public void RequestRewardedAd()
    {
        string adUnitId = "R-M-5441275-3"; // замените на "R-M-XXXXXX-Y"
        AdRequestConfiguration adRequestConfiguration = new AdRequestConfiguration.Builder(adUnitId).Build();
        rewardedAdLoader.LoadAd(adRequestConfiguration);
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show();
        }
    }

    public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
    {
        // The ad was loaded successfully. Now you can handle it.
        rewardedAd = args.RewardedAd;

        // Add events handlers for ad actions
        rewardedAd.OnAdClicked += HandleAdClicked;
        rewardedAd.OnAdShown += HandleAdShown;
        rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
        rewardedAd.OnAdImpression += HandleImpression;
        rewardedAd.OnAdDismissed += HandleAdDismissed;
        rewardedAd.OnRewarded += HandleRewarded;
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        // Ad {args.AdUnitId} failed for to load with {args.Message}
        // Attempting to load a new ad from the OnAdFailedToLoad event is strongly discouraged.
    }

    public void HandleAdDismissed(object sender, EventArgs args)
    {
        RequestRewardedAd();

        // Clear resources after an ad dismissed.
        DestroyRewardedAd();

        
    }

    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        // Called when rewarded ad failed to show.

        // Clear resources after an ad dismissed.
        DestroyRewardedAd();

        
    }

    public void HandleAdClicked(object sender, EventArgs args)
    {
        // Called when a click is recorded for an ad.
    }

    public void HandleAdShown(object sender, EventArgs args)
    {
        // Called when an ad is shown.
    }

    public void HandleImpression(object sender, ImpressionData impressionData)
    {
        // Called when an impression is recorded for an ad.
    }

    public void HandleRewarded(object sender, Reward args)
    {
        // Called when the user can be rewarded with {args.type} and {args.amount}.
        coins += 1;
        coinsText.text = "Coins: " + coins.ToString();
    }

    public void DestroyRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
    }
}