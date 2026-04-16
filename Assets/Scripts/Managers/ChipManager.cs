using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance { get; private set; }
    public int chipCount;
    public TextMeshProUGUI jewlText;
    public AdManager adManager;
    public Button spendButton;
    public int spentChips;

    private const string CHIP_KEY = "ChipCount";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadChips();
        UpdateUI();
    }

    public void UpdateChipCount(int additionalChips)
    {
        chipCount += additionalChips;

        if (additionalChips < 0)
        {
            spentChips -= additionalChips;
        }

        SaveChips();
        UpdateUI();
    }

    private void UpdateUI()
    {
        jewlText.text = "Chipcoin: " + chipCount;
    }

    private void SaveChips()
    {
        PlayerPrefs.SetInt(CHIP_KEY, chipCount);
        PlayerPrefs.Save(); 
    }

    private void LoadChips()
    {
        chipCount = PlayerPrefs.GetInt(CHIP_KEY, 0); 
    }
}
