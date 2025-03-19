using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class DataHexUnit : ITurnable
{
    public MainHexUnit mainHexUnit { get; private set; }
    [SerializeField]
    public DataHexUnitArmy ArmyHexUnit { get; set; }
    public DataHexUnitArmy armyHexUnit
    {
        get
        {
            return ArmyHexUnit;
        }
        set
        {
            ArmyHexUnit = value;
            Debug.Log("Zm�na: " + ArmyHexUnit.unitsInArmy.Count);
        }
    }

    public DataHexUnit(MainHexUnit mainHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
        armyHexUnit = new DataHexUnitArmy();
        CivGameManagerSingleton.Instance.allUnits.Add(mainHexUnit);
        PlayerOwner = CivGameManagerSingleton.Instance.players[(int)Random.Range(0,CivGameManagerSingleton.Instance.players.Length)];

        CurSpeed = maxSpeed;

    }
    public DataHexUnit(MainHexUnit mainHexUnit, Player player, DataHexUnitArmy army)
    {
        Debug.Log(mainHexUnit);
        Debug.Log(this.mainHexUnit);
        this.mainHexUnit = mainHexUnit;
        Debug.Log(mainHexUnit);
        Debug.Log(this.mainHexUnit);
        armyHexUnit = army;
        PlayerOwner = player;

        Debug.Log("VelikostK: " + armyHexUnit.unitsInArmy.Count);

        CurSpeed = maxSpeed;
    }
    public DataHexUnit(MainHexUnit mainHexUnit, DataHexUnitArmy armyHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
        this.armyHexUnit = armyHexUnit;

        CurSpeed = maxSpeed;
    }

    public static MainHexUnit unitPrefab;

    public List<MainHexCell> pathToTravel;
    const float travelSpeed = 4f;
    const float rotationSpeed = 180f;

    public int maxSpeed = 15;
    private int curSpeed = 15;
    public int CurSpeed { get => curSpeed;
        set
        {
            curSpeed = value;
            if(curSpeed < 1) curSpeed = 1;
            Debug.Log("CUR " + curSpeed);
        }
    }

    [SerializeField]
    Player playerOwner;

    public Player PlayerOwner
    {
        get
        {
            return playerOwner;
        }
        set
        {
            playerOwner = value;/*
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
            }*/
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
            if (value == null || value.dataHexCell == null)
            {
                Debug.LogError("Location or dataHexCell is null.");
                return;
            }

            Debug.Log("Kok: " + value);
            value.dataHexCell.Unit = mainHexUnit;

            if (mainHexUnit == null)
            {
                Debug.LogError("mainHexUnit is null.");
                return;
            }

            if (value.dataHexCell.Position == null)
            {
                Debug.LogError("Position is null or not initialized.");
                return;
            }
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
        return !cell.dataHexCell.waterScript.IsUnderwater; //&& !cell.dataHexCell.Unit;
    }

    

    public IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].dataHexCell.Position;
        mainHexUnit.transform.localPosition = c;
        Debug.Log("Za��n� " + pathToTravel.Count);
        yield return LookAt(pathToTravel[1].dataHexCell.Position);
        Debug.Log("POKRA�UJE");
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

        if (angle > 30f)
        {
            float speed = rotationSpeed / angle;
            Debug.Log("Speed: " + speed);

            for (float t = Time.deltaTime * speed; t < 0.6f * speed; t += Time.deltaTime)
            {
                Debug.Log("T: " + t);
                mainHexUnit.transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }

            mainHexUnit.transform.LookAt(point);
            orientation = mainHexUnit.transform.localRotation.eulerAngles.y;
        }
        
    }

    public void Turn()
    {
        CurSpeed = maxSpeed;
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
