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

        Debug.Log("ARMIES " + armies.Count);

        foreach (var armyObject in armies)
        {
            Debug.Log("ARMY " + armyObject.name);
            MainHexUnit armyUnit = armyObject.GetComponent<MainHexUnit>();
            if (armyUnit != null)
            {
                Debug.Log(armyUnit.dataHexUnit.maxSpeed);
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
            bool unitOnCell = currentLocation.dataHexCell.Unit != null;
            Player selectedUnitPlayer = armyUnit.dataHexUnit.PlayerOwner;
            Player destinationUnitPlayer = unitOnCell ? currentLocation.dataHexCell.Unit.dataHexUnit.PlayerOwner : null;
            UnitAtDestination unitAtD = UnitAtDestination.None;
            if (unitOnCell)
            {
                unitAtD = selectedUnitPlayer == destinationUnitPlayer ? UnitAtDestination.Ally : UnitAtDestination.Enemy;
            }
            // Use the FindPath method to calculate the path
            if (Vector3.Distance(currentLocation.gameObject.transform.position, targetLocation.gameObject.transform.position) > 3)
            {
                targetLocation = targetLocation.dataHexCell.neighbours[Random.Range(0, targetLocation.dataHexCell.neighbours.Length)];
            }
            CivGameManagerSingleton.Instance.hexGrid.FindPath(currentLocation, targetLocation, armyUnit.dataHexUnit.CurSpeed, unitAtD);

            // Check if a valid path exists
            MoveOrAttack(armyUnit, targetLocation);
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

        Debug.Log("ENEMY ARMIES " + enemyCities.Count);

        MainHexCell nearestLocation = null;
        float nearestDistance = float.MaxValue;
        
        /*
        foreach (var city in enemyCities)
        {
            Debug.Log(city + " " + currentLocation.dataHexCell.coordinates.ToStringOnSeparateLines());
            float distance = Vector3.Distance(currentLocation.dataHexCell.Position, city.dataCity.Location.dataHexCell.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLocation = city.dataCity.Location;

                Debug.Log("TRUE KOKOTGOISJGOISJ");
            }
        }*/

        foreach (var army in enemyArmies)
        {
            float distance = Vector3.Distance(currentLocation.dataHexCell.Position, army.dataHexUnit.Location.dataHexCell.Position);
            Debug.Log($"DIS {distance} < {nearestDistance} && {distance} < 10");
            if (distance < nearestDistance && distance < 200)
            {
                Debug.Log("TRUE KOKOT");
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

    void MoveOrAttack(MainHexUnit SelectedUnit, MainHexCell CurrentCell)
    {
        MainHexCell currentCell;
        try
        {
            currentCell = CurrentCell;

            if (SelectedUnit)
            {
                MainHexUnit unitOnCell = currentCell.dataHexCell.Unit;
                if (unitOnCell || currentCell.dataHexCell.featuresHexCell.SpecialIndex > 0)
                {
                    MainHexCell[] neighbors = SelectedUnit.dataHexUnit.Location.brainHexCell.GetAllNeighbors();

                    foreach (MainHexCell neighbor in neighbors)
                    {
                        var neighborUnit = neighbor.dataHexCell.Unit;
                        var neighborCity = neighbor.dataHexCell.City;
                        Debug.Log("Mìsto " + neighborCity);

                        if (neighborUnit != null && neighborUnit == unitOnCell && SelectedUnit.dataHexUnit.CurSpeed >= SelectedUnit.dataHexUnit.maxSpeed)
                        {
                            if (neighborUnit != SelectedUnit &&
                                neighborUnit.dataHexUnit.PlayerOwner != SelectedUnit.dataHexUnit.PlayerOwner)
                            {
                                Debug.Log("Útok na nepøítele!");
                                SelectedUnit.dataHexUnit.CurSpeed = 0;
                                SelectedUnit.Attack(SelectedUnit, neighborUnit);
                                return;
                            }
                            else if (neighborUnit == SelectedUnit)
                            {
                                Debug.Log("Klinutí na pøátelksou armádu");
                                return;
                            }

                        }
                        else if (neighborCity != null &&
                            neighborCity.dataCity.playerOwner != SelectedUnit.dataHexUnit.PlayerOwner)
                        {
                            if (SelectedUnit.dataHexUnit.CurSpeed >= SelectedUnit.dataHexUnit.maxSpeed)
                            {
                                Debug.Log("Útok na nepøátelské mìsto");
                                SelectedUnit.dataHexUnit.CurSpeed = 0;
                                neighborCity.dataCity.CurrentHealth -= 45;
                                return;
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                }
                DoMove(SelectedUnit);
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log("Jáj: " + ex);
        }
    }
    void DoMove(MainHexUnit selectedUnit)
    {
        if (CivGameManagerSingleton.Instance.hexGrid.HasPath)
        {
            Debug.Log("Move2");
            int tempSpeed = selectedUnit.dataHexUnit.CurSpeed;
            selectedUnit.Travel(CivGameManagerSingleton.Instance.hexGrid.GetPath(ref tempSpeed));
            selectedUnit.dataHexUnit.CurSpeed = tempSpeed;
            CivGameManagerSingleton.Instance.hexGrid.ClearPath();
        }
    }

}