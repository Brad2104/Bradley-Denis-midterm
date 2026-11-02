using System.Collections;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    [Header("Wall setup")]
    public GameObject wallPrefab;          // Drag this wall's prefab here
    public bool isOriginal = true;         // Leave ON for the one wall you place in the scene
    public int totalWalls = 3;             // How many walls exist in total

    [Header("Teleporting")]
    public float teleportInterval = 2.5f;  // Seconds between teleports (was 7)
    public float warningTime = 1f;         // Wall blinks this long before moving
    public float blinkSpeed = 0.15f;

    [Header("Fairness")]
    public float minDistanceFromPlayer = 3f;
    public int maxPlacementTries = 30;

    [Header("Area (world coordinates)")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private SpriteRenderer sprite;
    private Collider2D wallCollider;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();

        // Only the original spawns the extra walls
        if (isOriginal && wallPrefab != null)
        {
            for (int i = 1; i < totalWalls; i++)
            {
                GameObject copy = Instantiate(wallPrefab, transform.position, transform.rotation);
                WallScript copyScript = copy.GetComponent<WallScript>();

                // Copy this wall's settings so the copies match
                copyScript.isOriginal = false;
                copyScript.teleportInterval = teleportInterval;
                copyScript.warningTime = warningTime;
                copyScript.blinkSpeed = blinkSpeed;
                copyScript.minDistanceFromPlayer = minDistanceFromPlayer;
                copyScript.maxPlacementTries = maxPlacementTries;
                copyScript.minX = minX;
                copyScript.maxX = maxX;
                copyScript.minY = minY;
                copyScript.maxY = maxY;

                copy.transform.position = FindSpot();
            }
        }

        StartCoroutine(TeleportLoop());
    }

    IEnumerator TeleportLoop()
    {
        // Random head start so the walls don't all move at the same moment
        yield return new WaitForSeconds(Random.Range(0f, teleportInterval * 0.5f));

        while (true)
        {
            yield return new WaitForSeconds(Mathf.Max(0f, teleportInterval - warningTime));

            // Blink briefly so the move never feels like a surprise
            float t = 0f;
            while (t < warningTime)
            {
                if (sprite != null) sprite.enabled = !sprite.enabled;
                yield return new WaitForSeconds(blinkSpeed);
                t += blinkSpeed;
            }
            if (sprite != null) sprite.enabled = true;

            transform.position = FindSpot();
        }
    }

    // Picks a random spot that isn't near the player, a coin, or another wall
    Vector3 FindSpot()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 size = wallCollider != null ? (Vector2)wallCollider.bounds.size : Vector2.one;

        for (int i = 0; i < maxPlacementTries; i++)
        {
            Vector2 candidate = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            if (player != null && Vector2.Distance(candidate, player.transform.position) < minDistanceFromPlayer)
                continue;

            Collider2D[] hits = Physics2D.OverlapBoxAll(candidate, size, transform.eulerAngles.z);
            bool blocked = false;
            foreach (Collider2D hit in hits)
            {
                if (hit != wallCollider) { blocked = true; break; }
            }
            if (blocked) continue;

            return new Vector3(candidate.x, candidate.y, transform.position.z);
        }

        // If nothing worked, just stay where we are
        return transform.position;
    }
}