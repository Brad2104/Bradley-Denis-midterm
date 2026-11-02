using System.Collections;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    [Header("Hazard setup")]
    public GameObject hazardPrefab;        // Drag the hazard prefab here
    public bool isOriginal = true;         // Leave ON for the one hazard you place in the scene
    public int totalHazards = 3;           // How many hazards exist in total

    [Header("Teleporting")]
    public float teleportInterval = 3f;    // Seconds between teleports
    public float warningTime = 0.5f;       // Blinks this long before moving (set to 0 for no warning)
    public float blinkSpeed = 0.1f;

    [Header("Fairness")]
    public float minDistanceFromPlayer = 4f;
    public int maxPlacementTries = 30;

    [Header("Area (world coordinates)")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private SpriteRenderer sprite;
    private PlayerScript player;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = FindFirstObjectByType<PlayerScript>();

        // Only the original spawns the extra hazards
        if (isOriginal && hazardPrefab != null)
        {
            for (int i = 1; i < totalHazards; i++)
            {
                GameObject copy = Instantiate(hazardPrefab, transform.position, transform.rotation);
                HazardScript copyScript = copy.GetComponent<HazardScript>();

                // Copy this hazard's settings so the copies match
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
        // Random head start so the hazards don't all move at the same moment
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

    // Picks a random spot that isn't too close to the player
    Vector3 FindSpot()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerScript>();

        for (int i = 0; i < maxPlacementTries; i++)
        {
            Vector2 candidate = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            if (player != null && Vector2.Distance(candidate, player.transform.position) < minDistanceFromPlayer)
                continue;

            return new Vector3(candidate.x, candidate.y, transform.position.z);
        }

        // If nothing worked, just stay where we are
        return transform.position;
    }
}