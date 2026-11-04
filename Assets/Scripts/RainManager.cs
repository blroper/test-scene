using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject Raindrop;

    private float spawnLimitXLeft = -500;
    private float spawnLimitXRight = 500;
    private float topEdgeY = 200f;
    private float lowerBoundY = -400f;
    private Camera mainCamera;

    public float fallSpeed = 1200f;
    public float spawnInterval = 0.02f;

    private List<GameObject> activeRaindrops = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        UpdateSpawnBounds();
        ScheduleNextSpawn();
    }

    void Update()
    {
        for (int i = activeRaindrops.Count - 1; i >= 0; i--)
        {
            if (activeRaindrops[i] == null)
            {
                activeRaindrops.RemoveAt(i);
                continue;
            }

            activeRaindrops[i].transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            if (activeRaindrops[i].transform.position.y < lowerBoundY)
            {
                Destroy(activeRaindrops[i]);
                activeRaindrops.RemoveAt(i);
            }
        }
    }

    void UpdateSpawnBounds()
    {
        if (mainCamera == null)
        {
            return;
        }

        float cameraZ = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 leftBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraZ));
        Vector3 rightBoundary = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraZ));

        spawnLimitXLeft = leftBoundary.x - 10f;
        spawnLimitXRight = rightBoundary.x + 10f;
    }

    void ScheduleNextSpawn()
    {
        Invoke(nameof(SpawnRaindrop), spawnInterval);
    }

    // Spawn raindrops along the top edge of the play area
    void SpawnRaindrop()
    {
        UpdateSpawnBounds();

        float spawnX = Random.Range(spawnLimitXLeft, spawnLimitXRight);
        Vector3 spawnPos = new Vector3(spawnX, topEdgeY, 0);

        GameObject drop = Instantiate(Raindrop, spawnPos, Raindrop.transform.rotation);
        activeRaindrops.Add(drop);

        ScheduleNextSpawn();
    }
}
