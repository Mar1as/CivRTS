using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class DataHexUnit
{
    public MainHexUnit mainHexUnit { get; private set; }
    public ArmyHexUnit armyHexUnit { get; set; }

    public DataHexUnit(MainHexUnit mainHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
        armyHexUnit = new ArmyHexUnit(mainHexUnit);
        CivGameManagerSingleton.Instance.allUnits.Add(mainHexUnit);
        PlayerOwner = CivGameManagerSingleton.Instance.players[(int)Random.Range(0,CivGameManagerSingleton.Instance.players.Length)];

    }
    public DataHexUnit(MainHexUnit mainHexUnit, Player player, ArmyHexUnit army)
    {
        this.mainHexUnit = mainHexUnit;
        armyHexUnit = army;
        PlayerOwner = player;

    }
    public DataHexUnit(MainHexUnit mainHexUnit, ArmyHexUnit armyHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
        this.armyHexUnit = armyHexUnit;
    }

    public static MainHexUnit unitPrefab;

    public List<MainHexCell> pathToTravel;
    const float travelSpeed = 4f;
    const float rotationSpeed = 180f;

    Player playerOwner;
    Player PlayerOwner
    {
        get
        {
            return playerOwner;
        }
        set
        {
            if (playerOwner == null)
            {
                for (int i = 0; i < CivGameManagerSingleton.Instance.players.Length; i++)
                {
                    if (CivGameManagerSingleton.Instance.players[i] == value)
                    {
                        CivGameManagerSingleton.Instance.players[i].AddArmy(mainHexUnit.gameObject);
                        return;
                    }

                }
            }
            else if (playerOwner != value)
            {
                for (int i = 0; i < CivGameManagerSingleton.Instance.players.Length; i++)
                {
                    if (CivGameManagerSingleton.Instance.players[i] == playerOwner)
                    {
                        CivGameManagerSingleton.Instance.players[i].RemoveArmy(mainHexUnit.gameObject);
                    }
                    if (CivGameManagerSingleton.Instance.players[i] == value)
                    {
                        CivGameManagerSingleton.Instance.players[i].AddArmy(mainHexUnit.gameObject);
                    }
                }
            }
        }
    }

    MainHexCell location;
    public MainHexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.dataHexCell.Unit = null;
            }
            location = value;
            value.dataHexCell.Unit = mainHexUnit;
            mainHexUnit.transform.localPosition = value.dataHexCell.Position;
        }
    }

    float orientation;
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            mainHexUnit.transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }


    public void ValidateLocation()
    {
        mainHexUnit.transform.localPosition = location.dataHexCell.Position;
    }

    public bool IsValidDestination(MainHexCell cell)
    {
        return !cell.dataHexCell.waterScript.IsUnderwater && !cell.dataHexCell.Unit;
    }

    

    public IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].dataHexCell.Position;
        mainHexUnit.transform.localPosition = c;
        yield return LookAt(pathToTravel[1].dataHexCell.Position);

        float t = Time.deltaTime * travelSpeed;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = c;
            b = pathToTravel[i - 1].dataHexCell.Position;
            c = (b + pathToTravel[i].dataHexCell.Position) * 0.5f;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                mainHexUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                mainHexUnit.transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            t -= 1f;
        }

        a = c;
        b = pathToTravel[pathToTravel.Count - 1].dataHexCell.Position;
        c = b;
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            mainHexUnit.transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            mainHexUnit.transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        mainHexUnit.transform.localPosition = location.dataHexCell.Position;
        orientation = mainHexUnit.transform.localRotation.eulerAngles.y;

        ListPool<MainHexCell>.Add(pathToTravel);
        pathToTravel = null;
    }

    public IEnumerator LookAt(Vector3 point)
    {
        point.y = mainHexUnit.transform.localPosition.y;
        Quaternion fromRotation = mainHexUnit.transform.localRotation;
        Quaternion toRotation = Quaternion.LookRotation(point - mainHexUnit.transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;

            for (float t = Time.deltaTime * speed; t < 1f * speed; t += Time.deltaTime)
            {
                mainHexUnit.transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }

            mainHexUnit.transform.LookAt(point);
            orientation = mainHexUnit.transform.localRotation.eulerAngles.y;
        }
        
    }
}

public static class Bezier
{

    public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float r = 1f - t;
        return r * r * a + 2f * r * t * b + t * t * c;
    }

    public static Vector3 GetDerivative(
        Vector3 a, Vector3 b, Vector3 c, float t
    )
    {
        return 2f * ((1f - t) * (b - a) + t * (c - b));
    }
}
