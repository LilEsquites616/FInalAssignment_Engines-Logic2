using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    async void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            await UnityServices.InitializeAsync();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConsentGiven()
    {
        EndUserConsent.SetConsentState(new()
        {
            AdsIntent = ConsentStatus.Granted,
            AnalyticsIntent = ConsentStatus.Granted
        });
    }

    public void DenyConsent()
    {
        EndUserConsent.SetConsentState(new()
        {
            AdsIntent = ConsentStatus.Denied,
            AnalyticsIntent = ConsentStatus.Denied
        });
    }

    public void LogChipPurchase(int amount)
    {
        AnalyticsService.Instance.RecordEvent(new ChipsPurchasedEvent()
        {
            ChipAmount = amount
        });
    }

    public void LogAdWatched(string adType)
    {
        AnalyticsService.Instance.RecordEvent(new AdWatchedEvent()
        {
            AdType = adType
        });
    }

    public void LogWaveReached(int waveNumber)
    {
        AnalyticsService.Instance.RecordEvent(new WaveReachedEvent()
        {
            waveNumber = waveNumber
        });
    }
    public void LogModBought(string modType)
    {
        AnalyticsService.Instance.RecordEvent(new ModBoughtEvent()
        {
            ModType = modType
        });
    }

}