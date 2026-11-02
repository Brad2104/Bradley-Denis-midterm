using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class timescript : MonoBehaviour
{
    public TMP_Text TimerText;              // Drag your timer text object here
    public float startTime = 60f;           // 1 minute
    public float coinBonus = 30f;           // Seconds added per coin
    public float hazardPenalty = 15f;       // Seconds removed per hazard hit
    public string gameOverScene = "Game Over";

    public static timescript Instance;

    private float timeLeft;
    private bool finished = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = startTime;
        UpdateText();
    }

    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            UpdateText();
            finished = true;

            PlayerScript p = FindFirstObjectByType<PlayerScript>();
            if (p != null) p.Die();
            else SceneManager.LoadScene(gameOverScene);
            return;
        }

        UpdateText();
    }

    public void AddTime(float seconds)
    {
        timeLeft += seconds;
        UpdateText();
    }

    public void RemoveTime(float seconds)
    {
        timeLeft -= seconds;
        UpdateText();
    }

    void UpdateText()
    {
        if (TimerText == null) return;

        float shown = Mathf.Max(timeLeft, 0f);
        int minutes = Mathf.FloorToInt(shown / 60f);
        int seconds = Mathf.FloorToInt(shown % 60f);
        TimerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        // Turns red when 10 seconds or less remain
        TimerText.color = timeLeft <= 10f ? Color.red : Color.white;
    }
}