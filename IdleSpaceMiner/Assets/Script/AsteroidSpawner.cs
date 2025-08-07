using UnityEngine;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public SpaceStation station;
    public ResourceType[] resourceTypes;

    public int numberOfRings = 3;
    public float ringSpacing = 3f;    // distance between rings
    public float minAsteroidSpacing = 2f;  // desired gap between asteroids on ring

    void Start()
    {
        SpawnAsteroidRings();
    }

    void SpawnAsteroidRings()
    {
        for (int ringIndex = 1; ringIndex <= numberOfRings; ringIndex++)
        {
            float ringRadius = ringIndex * ringSpacing;
            float circumference = 2f * Mathf.PI * ringRadius;
            int asteroidCount = Mathf.FloorToInt(circumference / minAsteroidSpacing);

            float angleStep = 360f / asteroidCount;

            for (int i = 0; i < asteroidCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * ringRadius;

                GameObject newAsteroid = Instantiate(asteroidPrefab, pos, Quaternion.identity, transform);
                Asteroid asteroidScript = newAsteroid.GetComponent<Asteroid>();
                asteroidScript.resourceType = resourceTypes[Random.Range(0, resourceTypes.Length)];
                asteroidScript.resourceAmount = Random.Range(50, 150);

                station.asteroids.Add(asteroidScript);
            }
        }
    }
}