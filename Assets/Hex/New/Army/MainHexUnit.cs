using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using static UnityEditor.FilePathAttribute;
using TMPro;

[System.Serializable]
public class MainHexUnit : MonoBehaviour
{
    [SerializeField]
    public DataHexUnit dataHexUnit;

    [SerializeField]
    BarText text;

    private void Start()
    {
        Inicilizace();

        Debug.Log("KOKOT " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
        ChangeColorOfModel();
    }
    private void Update()
    {
        Debug.Log("KONECC " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
        text.ChangeText(dataHexUnit.armyHexUnit.unitsInArmy.Count.ToString());
    }
    void OnEnable()
    {
        if (dataHexUnit.Location)
        {
            transform.localPosition = dataHexUnit.Location.dataHexCell.Position;
        }
        ChangeColorOfModel();
    }

    public void Inicilizace()
    {
        if (dataHexUnit == null)
        {
            Debug.Log("KKT");
            dataHexUnit = new DataHexUnit(this);
        }
    }
    public void Inicilizace(Player player, DataHexUnitArmy army)
    {
        if (dataHexUnit == null)
        {
            dataHexUnit = new DataHexUnit(this, player, army);
        }
        else if (dataHexUnit.mainHexUnit == null)
        {
            dataHexUnit = new DataHexUnit(this, player, army);
        }
        else
        {
            Debug.Log("CO?");
        }
    }

    public void Die()
    {
        dataHexUnit.Location.dataHexCell.Unit = null;
        Destroy(gameObject);
    }

    public void Travel(List<MainHexCell> path)
    {
        dataHexUnit.Location = path[path.Count - 1];
        dataHexUnit.pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(dataHexUnit.TravelPath());
    }

    

    #region Save Load
    public void Save(BinaryWriter writer)
    {
        dataHexUnit.Location.dataHexCell.coordinates.Save(writer);
        writer.Write(dataHexUnit.Orientation);
    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(
            Instantiate(DataHexUnit.unitPrefab), grid.GetCell(new Vector3(coordinates.X,coordinates.Y,coordinates.Z)), orientation
        );
    }

    #endregion


    internal void Attack(MainHexUnit unit)
    {
        Debug.Log("ATTACK " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
        SceneSwap.passInfo[0] = new PassInformation(dataHexUnit.PlayerOwner, dataHexUnit.armyHexUnit);
        SceneSwap.passInfo[1] = new PassInformation(unit.dataHexUnit.PlayerOwner, unit.dataHexUnit.armyHexUnit);

        CivGameManagerSingleton.Instance.sceneSwap.LoadScene(1);
    }

    void ChangeColorOfModel()
    {
        Debug.Log("Change1");
        if (dataHexUnit.PlayerOwner.faction)
        {
            Debug.Log("Change2");
            MeshRenderer[] models = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer model in models)
            {
                model.material.color = dataHexUnit.PlayerOwner.faction.factionColor;
            }
        }
    }
}
