using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D RB;
    public float Speed = 3f;            // Units per second

    [Header("Score")]
    public TextMeshPro ScoreText;
    public int Score = 0;

    [Header("Screen wrap")]
    public float wrapX = 9f;
    public float wrapY = 5f;

    [Header("Health")]
    public int MaxHealth = 3;
    public int Health;
    public float InvincibilityTime = 1f;
    public TextMeshPro HealthText;          // optional
    public SpriteRenderer PlayerSprite;     // optional, used for the blink effect

    private bool invincible = false;
    private bool dead = false;

    void Start()
    {
        Health = MaxHealth;
        UpdateScore();
        UpdateHealth();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        Vector2 dir = Vector2.zero;

        if (Keyboard.current.rightArrowKey.isPressed) dir.x += 1;
        if (Keyboard.current.leftArrowKey.isPressed)  dir.x -= 1;
        if (Keyboard.current.upArrowKey.isPressed)    dir.y += 1;
        if (Keyboard.current.downArrowKey.isPressed)  dir.y -= 1;

        // Same speed in every direction, including diagonals
        RB.linearVelocity = dir.normalized * Speed;
    }

    void LateUpdate()
    {
        WrapAround();
    }

    void WrapAround()
    {
        Vector2 pos = RB.position;
        bool wrapped = false;

        if (pos.x > wrapX)       { pos.x = -wrapX; wrapped = true; }
        else if (pos.x < -wrapX) { pos.x = wrapX;  wrapped = true; }

        if (pos.y > wrapY)       { pos.y = -wrapY; wrapped = true; }
        else if (pos.y < -wrapY) { pos.y = wrapY;  wrapped = true; }

        if (wrapped)
            RB.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }

        CoinScript coin = other.gameObject.GetComponent<CoinScript>();
        if (coin != null)
        {
            coin.GetBumped();
            Score++;
            UpdateScore();

            // Coins add time
            if (timescript.Instance != null)
                timescript.Instance.AddTime(timescript.Instance.coinBonus);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (invincible || dead) return;

        // Hazard hit also costs time
        if (timescript.Instance != null)
            timescript.Instance.RemoveTime(timescript.Instance.hazardPenalty);

        Health -= amount;
        UpdateHealth();

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    // Gives health back, but never above the max
    public void Heal(int amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
        UpdateHealth();
    }

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        float timer = 0f;
        while (timer < InvincibilityTime)
        {
            if (PlayerSprite != null)
                PlayerSprite.enabled = !PlayerSprite.enabled;

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (PlayerSprite != null)
            PlayerSprite.enabled = true;

        invincible = false;
    }

    public void UpdateHealth()
    {
        if (HealthText != null)
            HealthText.text = "Health: " + Health;
    }

    public void UpdateScore()
    {
        if (ScoreText != null)
            ScoreText.text = "Score: " + Score;
    }

    public void Die()
    {
        if (dead) return;   // Stops it running twice if health and timer hit zero together
        dead = true;

        // Save scores so the Game Over scene can show them
        PlayerPrefs.SetInt("LastScore", Score);

        if (Score > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", Score);

        PlayerPrefs.Save();

        SceneManager.LoadScene("Game Over");
    }
}