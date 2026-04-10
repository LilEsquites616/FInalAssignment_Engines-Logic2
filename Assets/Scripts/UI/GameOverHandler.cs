using System.IO;
using TMPro;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public static GameOverHandler Instance { get; private set; }
    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [Header("UI Texts")]
    public TextMeshProUGUI gameOverHeader;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveSurvivedText;
    public TextMeshProUGUI earnedChipCoin;
    public TextMeshProUGUI chipcoin;

    [Header("References")]
    private EnemySpawner enemySpawner;
    public PlayerHealth playerHealth;
    [Header("Revive Settings")]
    public int reviveCost = 50;

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
    }
    void Start()
    {
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }
    public void TriggerGameOver(bool didWin)
    {
        if (ModsManager.Instance!=null)
            ModsManager.Instance.ResetAllPowers();
        Time.timeScale = 0f;
        Cursor.visible = true;
        gameOverPanel.SetActive(true);

        if (didWin)
            gameOverHeader.text = "You win!";

        int score = ScoreManager.Instance.GetScore();
        scoreText.text = $"Score: {score}";

        waveSurvivedText.text = $"Waves survived: {enemySpawner.currentWaveIndex + 1}";
        if (AnalyticsManager.Instance!=null)
            AnalyticsManager.Instance.LogWaveReached(enemySpawner.currentWaveIndex + 1);

        int chipsEarned = score / 10;

        int currentChips = PlayerPrefs.GetInt("ChipCount", 0);

        currentChips += chipsEarned;

        earnedChipCoin.text = "You have eanred: " + chipsEarned + " Chipcoin";
        chipcoin.text = "Chipcoin: " + currentChips;
        PlayerPrefs.SetInt("ChipCount", currentChips);
        PlayerPrefs.Save();
    }

    public void RevivePlayer()
    {
        int currentChips = PlayerPrefs.GetInt("ChipCount", 0);

        if (currentChips < reviveCost)
        {
            Debug.Log("Not enough ChipCoin to revive.");
            return;
        }

        currentChips -= reviveCost;
        PlayerPrefs.SetInt("ChipCount", currentChips);
        PlayerPrefs.Save();

        Debug.Log("Player revived! Remaining ChipCoin: " + currentChips);

        Time.timeScale = 1f;
        Cursor.visible = false;
        gameOverPanel.SetActive(false);
        playerHealth.currentHealth = playerHealth.maxHealth;

    }
}
