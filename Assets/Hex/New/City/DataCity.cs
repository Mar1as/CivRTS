using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataCity : ITurnable
{
    
    private MainCity mainCity { get; set; }

    private string name;
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = value;
                //CivGameManagerSingleton.Instance.usedCityNames.Add(name);
                return;
            }

            name = value;
        }
    }

    int maxHealth = 100;
    int currentHealth;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            
            currentHealth = value;
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            if (currentHealth <= 0)
            {
                mainCity.Destroy();
            }
        }
    }

    public StatsCity Stats { get; private set; }
    public ProductionCity Production { get; private set; }

    public List<MainHexCell> CellsInBorder { get; private set; } = new List<MainHexCell>();
    public MainHexCell Location { get; private set; }
    private MainHexCell nextCellToConquer;

    public Player playerOwner { get; private set; }

    public DataCity(MainHexCell initialCell, MainCity mainCity, Player player)
    {
        this.mainCity = mainCity;
        Location = initialCell;

        Production = new ProductionCity(mainCity);
        Stats = new StatsCity(mainCity);

        if(player != null)
        {
            playerOwner = player;
        }
        else
        {
            playerOwner = CivGameManagerSingleton.Instance.players[Random.Range(0, CivGameManagerSingleton.Instance.players.Length)];
        }
        
        InitializeCity();
    }

   


    private void InitializeCity()
    {
        AddBordersOnCreation();
        FindNextExpansion();
        Name = playerOwner.faction.GetRandomCityName();
        Debug.Log(name);
        if (Location != null)
        {
            Location.dataHexCell.chunk.Refresh();
        }
    }

    public void ExpandCity()
    {
        if (nextCellToConquer == null) return;

        AddBorder(nextCellToConquer);
        FindNextExpansion();
    }

    public void ShrinkCity()
    {
        if (CellsInBorder.Count == 0) return;

        var lastCell = CellsInBorder[CellsInBorder.Count - 1];
        RemoveBorder(lastCell);
        FindNextExpansion();
    }

    public void Turn()
    {
        Stats.ProcessTurn();
        Production.ProcessTurn(Stats.CalculateProduction());

        CurrentHealth += 10;
    }

    public void SelfDestruct()
    {
        foreach (var cell in CellsInBorder)
        {
            cell.dataHexCell.featuresHexCell.UrbanLevel = 0;
            cell.dataHexCell.featuresHexCell.FarmLevel = 0;
            cell.dataHexCell.featuresHexCell.SpecialIndex = 0;
            cell.dataHexCell.city = null;
            cell.dataHexCell.wallsScript.Walled = false;

            RemoveBorder(cell);
        }

        Debug.Log("City has been destroyed.");
    }

    #region Border Management

    private void AddBordersOnCreation()
    {
        if (Location == null) return;

        AddBorder(Location);
        foreach (HexDirection direction in System.Enum.GetValues(typeof(HexDirection)))
        {
            MainHexCell neighbor = Location.brainHexCell.GetNeighbor(direction);
            if (neighbor != null && neighbor.dataHexCell.city == null)
            {
                if (Location.brainHexCell.GetEdgeType(neighbor) != HexEdgeType.Cliff)
                {
                    AddBorder(neighbor);
                }
            }
        }
    }

    private void AddBorder(MainHexCell cell)
    {
        if (cell == null || CellsInBorder.Contains(cell)) return;

        CellsInBorder.Add(cell);
        cell.dataHexCell.city = mainCity;
        cell.dataHexCell.wallsScript.Walled = true;
    }

    private void RemoveBorder(MainHexCell cell)
    {
        if (cell == null || !CellsInBorder.Contains(cell)) return;

        //CellsInBorder.Remove(cell);
        cell.dataHexCell.city = null;
        cell.dataHexCell.wallsScript.Walled = false;
    }

    #endregion

    #region Expansion Management

    private void FindNextExpansion()
    {
        nextCellToConquer = null;

        List<MainHexCell> validCells = new List<MainHexCell>();

        foreach (var cell in CellsInBorder)
        {
            foreach (HexDirection direction in System.Enum.GetValues(typeof(HexDirection)))
            {
                MainHexCell neighbor = cell.brainHexCell.GetNeighbor(direction);
                if (neighbor != null && neighbor.dataHexCell.city == null)
                {
                    if (cell.brainHexCell.GetEdgeType(neighbor) != HexEdgeType.Cliff)
                    {
                        validCells.Add(neighbor);
                    }
                }
            }
        }

        if (validCells.Count > 0)
        {
            nextCellToConquer = validCells[UnityEngine.Random.Range(0, validCells.Count)];
        }
    }

    #endregion

    public MainHexCell GetRandomOwnedProvince()
    {
        if (CellsInBorder.Count == 0) return null;

        return CellsInBorder[UnityEngine.Random.Range(0, CellsInBorder.Count)];
    }
}
