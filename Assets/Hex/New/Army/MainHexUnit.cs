using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

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

        ChangeColorOfModel();
    }
    private void Update()
    {
        text.ChangeText(dataHexUnit.armyHexUnit.unitsInArmy.Count.ToString());

        if(!dataHexUnit.armyHexUnit.HasUnitsInArmy()) Die();
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


    internal void Attack(MainHexUnit attUnit, MainHexUnit defUnit)
    {
        //Debug.Log($"att: {attUnit.dataHexUnit.PlayerOwner.faction.name} {attUnit.dataHexUnit.armyHexUnit.unitsInArmy[0].name}");
        //Debug.Log($"def: {defUnit.dataHexUnit.PlayerOwner.faction.name} {defUnit.dataHexUnit.armyHexUnit.unitsInArmy[0].name}");
        
        
        Debug.Log("ATTACK " + dataHexUnit.armyHexUnit.unitsInArmy.Count);
        SceneSwap.passInfo[0] = new PassInformation(attUnit.dataHexUnit.PlayerOwner, attUnit.dataHexUnit.armyHexUnit, PlayerStateInBattle.Attacker, attUnit.gameObject);
        SceneSwap.passInfo[1] = new PassInformation(defUnit.dataHexUnit.PlayerOwner, defUnit.dataHexUnit.armyHexUnit, PlayerStateInBattle.Defender, defUnit.gameObject);

        

        CivGameManagerSingleton.Instance.sceneSwap.LoadScene(0,true);
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
