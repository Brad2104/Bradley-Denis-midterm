using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class CoinScript : MonoBehaviour
{
    public int maxCoins = 10;            // Never more than this many coins at once
    public float respawnDelay = 0f;     // Seconds before a new coin appears
 
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;
 
    static int coinCount = 0;           // Shared by all coins
    bool collected = false;
 
    void Awake()
    {
        coinCount++;
    }
 
    void OnDestroy()
    {
        coinCount--;
    }
 
    // The player calls this function on the coin whenever they bump into it
    public void GetBumped()
    {
        if (collected) return;
        collected = true;
        StartCoroutine(SpawnNewCoin());
    }
 
    IEnumerator SpawnNewCoin()
    {
        // Hide this coin so it can't be collected again
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
 
        // Wait before spawning the new one (this is the controlled spawn rate)
        yield return new WaitForSeconds(respawnDelay);
 
        // This coin is still counted, so <= keeps the total at maxCoins
        if (coinCount <= maxCoins)
        {
            Vector3 pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z);
            GameObject newCoin = Instantiate(gameObject, pos, Quaternion.identity);
 
            // The copy was made while hidden, so turn it back on
            newCoin.GetComponent<SpriteRenderer>().enabled = true;
            newCoin.GetComponent<Collider2D>().enabled = true;
            newCoin.GetComponent<CoinScript>().collected = false;
        }
 
        // Destroy the collected coin
        Destroy(gameObject);
    }
}