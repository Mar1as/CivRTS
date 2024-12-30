using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Battalions
{
    List<Battalions> parent;
    List<GameObject> units;

    GameObject MainUnit;

    public GameObject mainUnit
    {
        get
        {
            /*
            if (ReferenceEquals(MainUnit, null))
            {
                MainUnit = GetManUnit();
            }*/
            return GetManUnit();
        }
        private set
        {
            MainUnit = value;
        }
    }

    TeamsConstructor player;
    public Battalions(List<Battalions> parent, List<GameObject> units, TeamsConstructor player)
    {
        this.player = player;
        this.parent = parent;
        Units = units;

        player.AddBattalion(this);

        player.uiManager.uiShowInfoAboutBattalion.CreateInPanels(this);
    }

    public Battalions(List<Battalions> parent)
    {
        this.parent = parent;
        units = new List<GameObject>();

    }

    public List<GameObject> Units
    {
        get { return units; }
        set
        {
            units = value;
            foreach (GameObject unit in units)
            {
                unit.GetComponent<UnitStats>().battalion = this;

            }

            Check();
        }
    }
    public void RemoveUnit(GameObject unit)
    {
        units.Remove(unit);
        Check();
    }

    public void Check()
    {

        if (units.Count < 1)
        {
            parent.Remove(this);
        }
        if (!ReferenceEquals(player, null))
        {
            //player.uiUnitsListCreateButton.UpdatePanel();
        }
    }

    GameObject GetManUnit()
    {
        foreach (GameObject gm in Units)
        {
            UnitStats unit = gm.GetComponent<UnitStats>();
            if (unit is TankUnitStats)
            {
                return gm;
            }
        }
        return Units[0];
    }

    /*
    bool ParentHasSameBattalion() //NEFUNGUJE
    {
        foreach (Battalions bat in parent) // Check if battalion with same units already exists
        {
            if (bat.units.Count == units.Count) // If number of units is the same
            {
                bool allUnitsMatch = true; //   Check if all units are the same
                foreach (GameObject gm in units) 
                {
                    if (!bat.units.Contains(gm)) // není tam => jiný battalion => vytvoøit nový battalion
                    {
                        allUnitsMatch = false;
                        break;
                    }
                }
                if (allUnitsMatch)
                {
                    Debug.Log("Nevytvoøit nový battalion, již existuje stejný.");
                    return true;
                }
            }
        }

        Debug.Log("Vytvoøit nový battalion, žádný stejný neexistuje.");
        return false;
    }*/
}