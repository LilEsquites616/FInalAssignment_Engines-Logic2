using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
public class GemManager : MonoBehaviour
{
    public int gemCount;
    public TextMeshProUGUI jewlText;
    public AdManager adManager;
    public Button spendButton;
    public int spentGems; 
    private void Awake()
    {
         if (gemCount >= 5)
            spendButton.interactable = true;
        else
            spendButton.interactable = false;
    }
    public void UpdateGemCount(int additionalGems)
    {
        gemCount+=additionalGems;
        jewlText.text = "" + gemCount;

        if (additionalGems <0)
        {
            spentGems-=additionalGems;

            if (spentGems==50)
            {
                spentGems=0;
                adManager.LoadAd("Interstitial");
                adManager.ShowAd("Interstitial");
            }
        }

        if (gemCount <=0)
            adManager.LoadAd("Reward");
            adManager.showRewardAdButton.interactable = true;
            spendButton.interactable = false;

        if (gemCount >0)
            adManager.showRewardAdButton.interactable = false; 
            if (gemCount >= 5)
                spendButton.interactable = true;

    }
}
