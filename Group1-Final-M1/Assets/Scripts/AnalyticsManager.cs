using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;
public class AnalyticsManager : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();
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
   /*  public void SendButtonClickEvent(string buttonType) //GEMS CLICKED EXERCISE
    {
        AnalyticsService.Instance.RecordEvent(new GemClickedEvent()
        {
            ButtonType = buttonType
        });
    }

    public void LogGemPurchase(int amount)
    {
        AnalyticsService.Instance.RecordEvent(new GemsPurchasedEvent()
        {
            GemAmount = amount
        });
    }

     public void LogAdWatched(string adType)
    {
        AnalyticsService.Instance.RecordEvent(new AdWatchedEvent()
        {
            AdType = adType
        });
    } */
}
