using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArmyAI
{
    private Player player;

    public ArmyAI(Player player)
    {
        this.player = player;
    }

    public void ProcessTurn()
    {
        // Get all armies owned by the player
        List<GameObject> armies = player.Armies;

        foreach (var armyObject in armies)
        {
            MainHexUnit armyUnit = armyObject.GetComponent<MainHexUnit>();
            if (armyUnit != null)
            {
                DecideArmyAction(armyUnit);
            }
        }
    }

    private void DecideArmyAction(MainHexUnit armyUnit)
    {
        // Find the nearest enemy city or army
        MainHexCell currentLocation = armyUnit.dataHexUnit.Location;
        MainHexCell targetLocation = FindNearestEnemyLocation(currentLocation);

        if (targetLocation != null)
        {
            // Use the FindPath method to calculate the path
            CivGameManagerSingleton.Instance.hexGrid.FindPath(currentLocation, targetLocation, 50, UnitAtDestination.None);

            // Check if a valid path exists
            if (CivGameManagerSingleton.Instance.hexGrid.HasPath)
            {
                // Get the calculated path
                List<MainHexCell> path = CivGameManagerSingleton.Instance.hexGrid.GetPath();

                if (path != null && path.Count > 0)
                {
                    // Move the army along the calculated path
                    MoveArmyAlongPath(armyUnit, path);
                }
                else
                {
                    Debug.Log($"No valid path found for army at {currentLocation} to target at {targetLocation}.");
                }
            }
            else
            {
                Debug.Log($"No path exists for army at {currentLocation} to target at {targetLocation}.");
            }
        }
        else
        {
            Debug.Log("No enemy target found for army.");
        }
    }

    private MainHexCell FindNearestEnemyLocation(MainHexCell currentLocation)
    {
        // Find the nearest enemy city or army
        List<MainCity> enemyCities = CivGameManagerSingleton.Instance.allCities.FindAll(city => city.dataCity.playerOwner != player);
        List<MainHexUnit> enemyArmies = CivGameManagerSingleton.Instance.allUnits.FindAll(unit => unit.dataHexUnit.PlayerOwner != player);

        MainHexCell nearestLocation = null;
        float nearestDistance = float.MaxValue;

        foreach (var city in enemyCities)
        {
            Debug.Log(city);
            float distance = Vector3.Distance(currentLocation.dataHexCell.Position, city.dataCity.Location.dataHexCell.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLocation = city.dataCity.Location;
            }
        }

        foreach (var army in enemyArmies)
        {
            float distance = Vector3.Distance(currentLocation.dataHexCell.Position, army.dataHexUnit.Location.dataHexCell.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLocation = army.dataHexUnit.Location;
            }
        }

        return nearestLocation;
    }

    private void MoveArmyAlongPath(MainHexUnit armyUnit, List<MainHexCell> path)
    {
        // Move the army along the calculated path
        if (path.Count > 0)
        {
            armyUnit.Travel(path);
        }
    }
}