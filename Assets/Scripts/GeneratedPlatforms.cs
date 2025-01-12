using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;

    public int PLATFORMS_NUM = 8;
    private GameObject[] platforms;
    private Vector2[] positions;

    public float radius = 5.0f;
    private float angleTotal = 360.0f;


    private float[] currentAngles;
    public float speed = 10.0f;

    private void Awake()
    {
        platforms = new GameObject[PLATFORMS_NUM];
        currentAngles = new float[PLATFORMS_NUM];

        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            // Set the initial angle for each platform
            currentAngles[i] = i * angleTotal / PLATFORMS_NUM;

            // Calculate the initial position
            float angleRad = currentAngles[i] * Mathf.Deg2Rad;
            float x = transform.position.x + radius * Mathf.Sin(angleRad);
            float y = transform.position.y + radius * Mathf.Cos(angleRad);

            platforms[i] = Instantiate(platformPrefab, new Vector2(x, y), Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.GAME)
        {
            return;
        }
        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            // Increment the angle for each platform
            currentAngles[i] += speed * Time.deltaTime;

            // Keep the angle within 0-360 degrees
            if (currentAngles[i] > 360.0f) currentAngles[i] -= 360.0f;

            // Convert angle to radians and update position
            float angleRad = currentAngles[i] * Mathf.Deg2Rad;
            float x = transform.position.x + radius * Mathf.Sin(angleRad);
            float y = transform.position.y + radius * Mathf.Cos(angleRad);

            // Update platform position
            platforms[i].transform.position = new Vector2(x, y);
        }
    }
}
