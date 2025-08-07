using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{

    public SpaceStation station;
    public DroneManager droneManager;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI resourcesText;

    public Button sellAllButton;
    public Button buyDroneButton;
    public Button upgradeSpeedButton;
    public Button upgradeCargoButton;

    [Header("Economy Settings")]
    public int droneBaseCost = 50;
    public int upgradeSpeedBaseCost = 25;
    public int upgradeCargoBaseCost = 25;

    private int droneCurrentCost;
    private int upgradeSpeedCurrentCost;
    private int upgradeCargoCurrentCost;

    void Start()
    {
        droneCurrentCost = droneBaseCost;
        upgradeSpeedCurrentCost = upgradeSpeedBaseCost;
        upgradeCargoCurrentCost = upgradeCargoBaseCost;

        sellAllButton.onClick.AddListener(SellAllResources);
        buyDroneButton.onClick.AddListener(BuyDrone);
        upgradeSpeedButton.onClick.AddListener(() => UpgradeDrone("Speed"));
        upgradeCargoButton.onClick.AddListener(() => UpgradeDrone("Cargo"));

        UpdateButtonTexts();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyText.text = $"Money: ${station.Money}";

        string resDisplay = "";
        foreach (var res in station.ResourceInventory)
        {
            resDisplay += $"{res.Key.resourceName}: {res.Value} | ";
        }
        resourcesText.text = resDisplay.TrimEnd(' ', '|');
    }

    void SellAllResources()
    {
        station.SellAllResources();
    }

    void BuyDrone()
    {
        int droneCount = droneManager.drones.Count;
        int droneCost = droneBaseCost * (droneCount + 1);

        if (station.Money >= droneCost)
        {
            station.Money -= droneCost;
            droneManager.SpawnDrone();
            UpdateButtonTexts();
        }
    }

    void UpgradeDrone(string type)
    {
        int baseCost = (type == "Speed") ? upgradeSpeedBaseCost : upgradeCargoBaseCost;

        // Calculate total cost to upgrade as many drones as possible
        int totalCost = 0;
        List<Drone> upgradableDrones = new List<Drone>();

        foreach (var drone in droneManager.drones)
        {
            int level = (type == "Speed") ? drone.speedLevel : drone.cargoLevel;
            int upgradeCost = Mathf.CeilToInt(baseCost * Mathf.Pow(1.5f, level));

            if (station.Money >= totalCost + upgradeCost)
            {
                totalCost += upgradeCost;
                upgradableDrones.Add(drone);
            }
            else
            {
                break; // stop when you can't afford more
            }
        }

        if (totalCost > 0)
        {
            station.Money -= totalCost;

            foreach (var drone in upgradableDrones)
            {
                switch (type)
                {
                    case "Speed":
                        drone.speedLevel++;
                        drone.speed += 0.5f;
                        break;
                    case "Cargo":
                        drone.cargoLevel++;
                        drone.cargoCapacity += 1;
                        break;
                }
            }
        }

        UpdateButtonTexts();
    }

    void UpdateButtonTexts()
    {
        int droneCount = droneManager.drones.Count;
        int droneCost = droneBaseCost * (droneCount + 1);

        buyDroneButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy Drone (${droneCost})";

        int speedTotal = CalculateTotalUpgradeCost("Speed", upgradeSpeedBaseCost);
        int cargoTotal = CalculateTotalUpgradeCost("Cargo", upgradeCargoBaseCost);

        upgradeSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Speed (${speedTotal})";
        upgradeCargoButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Cargo (${cargoTotal})";
    }

    int CalculateTotalUpgradeCost(string type, int baseCost)
    {
        int totalCost = 0;

        foreach (var drone in droneManager.drones)
        {
            int level = (type == "Speed") ? drone.speedLevel : drone.cargoLevel;
            int upgradeCost = Mathf.CeilToInt(baseCost * Mathf.Pow(1.5f, level));
            totalCost += upgradeCost;
        }

        return totalCost;
    }
}