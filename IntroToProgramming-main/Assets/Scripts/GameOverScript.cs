using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text currentScoreText;   // Drag your score text here
    public TMP_Text highScoreText;      // Drag your high score text here

    [Header("Restart")]
    public string gameScene = "Example 3";

    void Start()
    {
        int last = PlayerPrefs.GetInt("LastScore", 0);
        int high = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScoreText != null)
            currentScoreText.text = "Score: " + last;

        if (highScoreText != null)
        {
            highScoreText.text = (last >= high && last > 0)
                ? "New High Score: " + high
                : "High Score: " + high;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(gameScene);
        }
    }
}