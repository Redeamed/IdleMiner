using UnityEngine;

public class Drone : MonoBehaviour
{
    public int speedLevel = 0;
    public int cargoLevel = 0;

    public float speed = 2f;
    public int cargoCapacity = 10;
    public float miningRate = 2f; // units per second
    private float miningProgress = 0f;
    public float miningDistance = 1.0f; // Distance to stop before asteroid

    private Asteroid targetAsteroid;
    private Vector3 targetPosition;
    private int currentCargo = 0;
    private ResourceType carriedResourceType;

    private enum State { Idle, FlyingToAsteroid, Mining, Returning }
    private State currentState = State.Idle;

    private SpaceStation station;
    private LineRenderer laser;

    public void Initialize(SpaceStation stationRef)
    {
        station = stationRef;
        currentState = State.Idle;
        laser = GetComponent<LineRenderer>();
        laser.enabled = false;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                FindAsteroid();
                break;
            case State.FlyingToAsteroid:
                MoveToTarget();
                break;
            case State.Mining:
                MineAsteroid();
                break;
            case State.Returning:
                MoveToStation();
                break;
        }
    }

    void FindAsteroid()
    {
        targetAsteroid = station.GetNearestUnoccupiedAsteroid(transform.position);

        if (targetAsteroid != null)
        {
            targetAsteroid.isOccupied = true; // Claim it
            targetPosition = targetAsteroid.transform.position;
            currentState = State.FlyingToAsteroid;
        }
        else
        {
            currentState = State.Idle; // Nothing to mine
        }
    }

    void MoveToTarget()
    {
        RotateTowards(targetPosition);

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > miningDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            currentState = State.Mining;
        }
    }

    void MineAsteroid()
    {
        if (targetAsteroid == null || targetAsteroid.IsDepleted)
        {
            laser.enabled = false;
            currentState = State.Idle;
            return;
        }

        // Enable laser & update its endpoints
        if (!laser.enabled) laser.enabled = true;

        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, targetAsteroid.transform.position);

        miningProgress += miningRate * Time.deltaTime;

        if (miningProgress >= 1f)
        {
            int unitsToMine = Mathf.FloorToInt(miningProgress);
            int mined = targetAsteroid.Mine(unitsToMine);

            currentCargo += mined;
            miningProgress -= unitsToMine;

            if (currentCargo >= cargoCapacity || targetAsteroid.IsDepleted)
            {
                carriedResourceType = targetAsteroid.resourceType;
                laser.enabled = false;
                targetPosition = station.transform.position;
                currentState = State.Returning;
            }
        }
    }

    void MoveToStation()
    {
        RotateTowards(targetPosition);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetAsteroid != null)
                targetAsteroid.isOccupied = false;

            laser.enabled = false;

            if (carriedResourceType != null && currentCargo > 0)
                station.DepositResources(currentCargo, carriedResourceType);

            currentCargo = 0;
            targetAsteroid = null;
            carriedResourceType = null;

            currentState = State.Idle;
        }
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90f because "up" is Y+
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}