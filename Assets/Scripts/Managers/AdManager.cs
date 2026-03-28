
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
public class AdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string adUnitAffix;
    private string bannerAdUnitPrefix = "Banner"; 
    public bool loadBannerOnStart = false;
    private string bannerAdUnitId;
    public Button showRewardAdButton;
    public ChipManager chipManager;
    public int chipsPerRewardAd;
    public AnalyticsManager analyticsManager;
    private void Awake()
    {
#if UNITY_IOS
    adUnitAffix = "_ios";
#elif UNITY_ANDROID
    adUnitAffix = "_Android";
#elif UNITY_EDITOR
    adUnitAffix = "_Android";
#endif
    bannerAdUnitId = bannerAdUnitPrefix + adUnitAffix;
    if (loadBannerOnStart)
        LoadBanner();
    }

    #region Interstitial/Reward Ads
    public void LoadAd(string adUnitPrefix)
    {
        string adUnitID = adUnitPrefix + adUnitAffix;
        Advertisement.Load(adUnitID,this);
    }
    public void OnUnityAdsAdLoaded(string placementID)
    {
        Debug.Log($"{placementID}");
    }
    public void OnUnityAdsFailedToLoad(string placementID, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"{placementID} failed to load: {error} - {message}");
    }
    public void ShowAd(string adUnitPrefix)
    {
        string adUnitID = adUnitPrefix + adUnitAffix;
        //analyticsManager.LogAdWatched(adUnitPrefix);
        Advertisement.Show(adUnitID, this);
        Advertisement.Banner.Hide();
    }
    public void OnUnityAdsShowFailure(string placementID, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"{placementID} failed to show: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string placementID)
    {
        Debug.Log($"{placementID} started");
    }

    public void OnUnityAdsShowClick(string placementID)
    {
        Debug.Log($"{placementID} clicked");
    }

    public void OnUnityAdsShowComplete(string placementID, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"{placementID} completed");
        if (placementID == "Rewarded_Android" || placementID == "Rewarded_iOS")
        {
            chipManager.UpdateChipCount(chipsPerRewardAd);
        }
        LoadBanner();
    }
    #endregion

    #region Banner Ads
    public void LoadBanner()
    {
        BannerLoadOptions loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(bannerAdUnitId, loadOptions);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        Advertisement.Banner.Show(bannerAdUnitId);
    }

    void OnBannerError(string message)
    {
        Debug.LogWarning("Banner failed to load: " + message);
    }
    #endregion
}
