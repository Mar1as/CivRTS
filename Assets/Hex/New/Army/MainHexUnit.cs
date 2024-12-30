using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using static UnityEditor.FilePathAttribute;

[System.Serializable]
public class MainHexUnit : MonoBehaviour
{
    [SerializeField]
    public DataHexUnit dataHexUnit;

    private void Start()
    {
        Inicilizace();

        Debug.Log("KONECC " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
    }
    private void Update()
    {
        Debug.Log("KONECC " + dataHexUnit.armyHexUnit.unitsInArmy.Count);

    }
    void OnEnable()
    {
        if (dataHexUnit.Location)
        {
            transform.localPosition = dataHexUnit.Location.dataHexCell.Position;
        }
    }

    public void Inicilizace()
    {
        if (dataHexUnit == null)
        {
            Debug.Log("KKT");
            dataHexUnit = new DataHexUnit(this);
        }
    }
    public void Inicilizace(Player player, ArmyHexUnit army)
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

    internal void Attack(MainHexUnit unit)
    {
        Debug.Log("ATTACK " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
        SceneSwap.passInfo[0] = new PassInformation(dataHexUnit.PlayerOwner, dataHexUnit.armyHexUnit);
        SceneSwap.passInfo[1] = new PassInformation(unit.dataHexUnit.PlayerOwner, unit.dataHexUnit.armyHexUnit);

        CivGameManagerSingleton.Instance.sceneSwap.LoadScene(1);
    }
    #endregion
}
