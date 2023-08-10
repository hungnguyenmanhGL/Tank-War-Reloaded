using HAVIGAME.UI;
using UnityEngine;
using UnityEngine.UI;

public class AdvertisingPanel : UIFrame {
    [Header("[References]")]
    [SerializeField] private Button btnShowAppOpenAd;
    [SerializeField] private Button btnShowInterstitialAd;
    [SerializeField] private Button btnShowRewardedAd;
    [SerializeField] private Button btnShowBannerAd;
    [SerializeField] private Button btnHideBannerAd;
    [SerializeField] private Button btnShowMRectAd;
    [SerializeField] private Button btnHideMRectAd;

    private void Start() {
        btnShowAppOpenAd.onClick.AddListener(ShowAppOpenAd);
        btnShowInterstitialAd.onClick.AddListener(ShowInterstitialAd);
        btnShowRewardedAd.onClick.AddListener(ShowRewardedAd);
        btnShowBannerAd.onClick.AddListener(ShowBannerAd);
        btnHideBannerAd.onClick.AddListener(HideBannerAd);
        btnShowMRectAd.onClick.AddListener(ShowMRectAd);
        btnHideMRectAd.onClick.AddListener(HideMRectAd);
    }

    private void ShowAppOpenAd() {
        GameAdvertising.TryShowAppOpenAd(null, true);
    }

    private void ShowInterstitialAd() {
        GameAdvertising.TryShowInterstitialAd(null, true);
    }

    private void ShowRewardedAd() {
        GameAdvertising.TryShowRewardedAd();
    }

    private void ShowBannerAd() {
        GameAdvertising.TryShowBannerAd(GameAdvertising.BannerPosition.BottomCenter);
    }

    private void HideBannerAd() {
        GameAdvertising.TryHideBannerAd();
    }

    private void ShowMRectAd() {
        GameAdvertising.TryShowMediumRectangleAd(GameAdvertising.MRectPosition.TopCenter);
    }

    private void HideMRectAd() {
        GameAdvertising.TryHideMediumRectangleAd();
    }
}
