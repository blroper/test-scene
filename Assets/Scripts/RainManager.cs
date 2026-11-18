using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    public GameObject Raindrop;
    [Header("Character Rig")]
    public Transform characterRig;
    public float spawnWidth = 200f;
    public float topOffset = 200f;
    public float lowerOffset = -200f;

    private float spawnLimitXLeft = -500;
    private float spawnLimitXRight = 500;
    private float topEdgeY = 100f;
    private float lowerBoundY = -50f;
    private Camera mainCamera;

    public float fallSpeed = 0.0001f;
    public float spawnInterval = 0.02f;

    private List<GameObject> activeRaindrops = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        if (characterRig == null)
        {
            GameObject rig = GameObject.FindWithTag("Player");
            if (rig != null)
            {
                characterRig = rig.transform;
            }
        }

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
        if (characterRig != null)
        {
            spawnLimitXLeft = characterRig.position.x - spawnWidth;
            spawnLimitXRight = characterRig.position.x + spawnWidth;
            topEdgeY = characterRig.position.y + topOffset;
            lowerBoundY = characterRig.position.y + lowerOffset;
            return;
        }

        if (mainCamera == null)
        {
            return;
        }

        float cameraZ = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 leftBoundary = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraZ));
        Vector3 rightBoundary = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraZ));

        spawnLimitXLeft = leftBoundary.x - 10f;
        spawnLimitXRight = rightBoundary.x + 10f;
        topEdgeY = mainCamera.transform.position.y + 100f;
        lowerBoundY = mainCamera.transform.position.y - 300f;
    }

    void ScheduleNextSpawn()
    {
        Invoke(nameof(SpawnRaindrop), spawnInterval);
    }

    // Spawn raindrops around the CharacterRig
    void SpawnRaindrop()
    {
        UpdateSpawnBounds();

        float spawnX = Random.Range(spawnLimitXLeft, spawnLimitXRight);
        float spawnY = Random.Range(lowerBoundY, topEdgeY);
        float spawnZ = characterRig != null ? characterRig.position.z : 0f;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);

        GameObject drop = Instantiate(Raindrop, spawnPos, Raindrop.transform.rotation);

        Rigidbody dropRigidbody = drop.GetComponent<Rigidbody>();
        if (dropRigidbody != null)
        {
            dropRigidbody.isKinematic = true;
            dropRigidbody.useGravity = false;
            dropRigidbody.linearVelocity = Vector3.zero;
        }

        activeRaindrops.Add(drop);

        ScheduleNextSpawn();
    }
}
