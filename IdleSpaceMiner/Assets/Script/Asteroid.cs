using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [HideInInspector]
    public bool isOccupied = false;

    public ResourceType resourceType;
    public int resourceAmount = 10;

    public bool IsDepleted => resourceAmount <= 0;

    public int Mine(int amount)
    {
        int mined = Mathf.Min(amount, resourceAmount);
        resourceAmount -= mined;
        if (IsDepleted)
        {
            gameObject.SetActive(false); // or trigger respawn later
        }
        return mined;
    }
}