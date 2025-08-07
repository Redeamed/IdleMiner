using UnityEngine;
using System.Collections.Generic;

public class SpaceStation : MonoBehaviour
{
    public List<Asteroid> asteroids = new List<Asteroid>();

    private Dictionary<ResourceType, int> resourceInventory = new Dictionary<ResourceType, int>();
    public int Money = 50;

    public Dictionary<ResourceType, int> ResourceInventory => resourceInventory;
    public void DepositResources(int amount, ResourceType resourceType)
    {
        if (!resourceInventory.ContainsKey(resourceType))
        {
            resourceInventory[resourceType] = 0;
        }

        resourceInventory[resourceType] += amount;
        Debug.Log($"Deposited {amount} of {resourceType.resourceName}. Total: {resourceInventory[resourceType]}");
    }

    public Asteroid GetNearestUnoccupiedAsteroid(Vector3 position)
    {
        if (asteroids == null || asteroids.Count == 0)
            return null;

        float closestDistance = Mathf.Infinity;
        Asteroid closest = null;

        foreach (var asteroid in asteroids)
        {
            if (asteroid == null || asteroid.IsDepleted || asteroid.isOccupied) continue;

            float dist = Vector3.Distance(position, asteroid.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = asteroid;
            }
        }

        return closest;
    }
    public void SellAllResources()
    {
        foreach (var res in resourceInventory)
        {
            Money += res.Value * res.Key.baseValue;
        }
        resourceInventory.Clear();
    }
}