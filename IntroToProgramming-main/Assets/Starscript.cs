using System.Collections;
using UnityEngine;

public class Starscript : MonoBehaviour
{
    [Header("Healing")]
    public int healAmount = 1;
    public bool onlyWhenHurt = true;     // Star stays put if player is at full health

    [Header("Respawn")]
    public float respawnTime = 30f;
    public float safeDistance = 4f;

    [Header("Area (world coordinates)")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private PlayerScript player;
    private SpriteRenderer sprite;
    private Collider2D starCollider;
    private bool collected = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        starCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        TryGive(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryGive(other.gameObject);
    }

    void TryGive(GameObject other)
    {
        if (collected) return;

        PlayerScript p = other.GetComponent<PlayerScript>();
        if (p == null) return;

        // Don't waste the star if the player is already at full health
        if (onlyWhenHurt && p.Health >= p.MaxHealth) return;

        p.Heal(healAmount);
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        collected = true;

        if (sprite != null) sprite.enabled = false;
        if (starCollider != null) starCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        Teleport();
        if (sprite != null) sprite.enabled = true;
        if (starCollider != null) starCollider.enabled = true;

        collected = false;
    }

    public void Teleport()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerScript>();

        Vector3 newPos = transform.position;

        for (int i = 0; i < 10; i++)
        {
            newPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

            if (player == null || Vector3.Distance(newPos, player.transform.position) > safeDistance)
                break;
        }

        transform.position = newPos;
    }
}