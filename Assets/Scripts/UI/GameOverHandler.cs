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

    [Header("References")]
    private EnemySpawner enemySpawner;
    void Start()
    {
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }
    public void TriggerGameOver(bool didWin)
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        gameOverPanel.SetActive(true);

        if (didWin)
            gameOverHeader.text = "You win!";

        int score = ScoreManager.Instance.GetScore();
        scoreText.text = $"Score: {score}";

        waveSurvivedText.text = $"Waves survived: {enemySpawner.currentWaveIndex + 1}";

        int chipsEarned = score / 10;

        int currentChips = PlayerPrefs.GetInt("ChipCount", 0);

        currentChips += chipsEarned;

        earnedChipCoin.text = "You have eanred: " + chipsEarned + " Chipcoin";

        PlayerPrefs.SetInt("ChipCount", currentChips);
        PlayerPrefs.Save();
    }
}
