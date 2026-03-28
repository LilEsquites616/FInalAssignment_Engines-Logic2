using System.IO;
using TMPro;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [Header("UI Texts")]
    public TextMeshProUGUI gameOverHeader;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveSurvivedText;

    [Header("References")]
    public GameObject newHighScoreText;
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

        waveSurvivedText.text = $"Waves survived: {enemySpawner.currentWaveIndex + 1}";
    }
}
