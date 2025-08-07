using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    public SpaceStation station;
    public GameObject dronePrefab;
    public List<Drone> drones = new List<Drone>();
    public void SpawnDrone()
    {
        GameObject newDrone = Instantiate(dronePrefab, station.transform.position, Quaternion.identity);
        Drone droneScript = newDrone.GetComponent<Drone>();
        droneScript.Initialize(station);
        drones.Add(droneScript);
    }
}