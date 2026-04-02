using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Socre: " + score;
    }

    public int GetScore()
    {
        return score;
    }
}