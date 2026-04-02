
using UnityEngine;
using UnityEngine.Advertisements;

public class AdInitializer : MonoBehaviour,IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameID;
    [SerializeField] private string IOSGameID;
    [SerializeField] private bool testMode = true;

    private string gameID;

    private void InitializeAds()
    {
#if UNITY_IOS
        gameID = IOSGameID;
#elif UNITY_ANDROID
        gameID = androidGameID;
#elif UNITY_EDITOR
        gameID = androidGameID;
#endif

        if(!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameID, testMode, this);
        }
    }
    void Awake()
    {
        InitializeAds();
    }

    void Update()
    {
        
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads successfully initialized");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"Unity Ads Initialization Failed: {error} - {message}");
    }
}