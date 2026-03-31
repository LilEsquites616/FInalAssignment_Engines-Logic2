using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
public class ChipManager : MonoBehaviour
{
    public int chipCount;
    public TextMeshProUGUI jewlText;
    public AdManager adManager;
    public Button spendButton;
    public int spentChips; 
    private void Awake()
    {
       
    }
    public void UpdateChipCount(int additionalChips)
    {
        chipCount+=additionalChips;
        jewlText.text = "Chipcoin: " + chipCount;

        if (additionalChips <0)
        {
            spentChips-=additionalChips;
        }

       /*  if (gemCount <=0)
            adManager.LoadAd("Reward");
            adManager.showRewardAdButton.interactable = true;
            spendButton.interactable = false;

        if (gemCount >0)
            adManager.showRewardAdButton.interactable = false; 
            if (gemCount >= 5)
                spendButton.interactable = true; */

    }
}
