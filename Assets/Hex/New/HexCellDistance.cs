using TMPro;
using UnityEngine;

public class HexCellDistance
{
    MainHexCell mainHexCell;
    DataHexCell data { get => mainHexCell.dataHexCell; }

    public HexCellDistance(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

    public MainHexCell PathFrom { get; set; }

    public MainHexCell NextWithSamePriority { get; set; }

    public int SearchHeuristic { get; set; }

    private int distance;

    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            UpdateDistanceLabel();
        }
    }

    void UpdateDistanceLabel()
    {
        TextMeshProUGUI label = data.uiRect.GetComponent<TextMeshProUGUI>();
        label.text = distance == int.MaxValue ? "" : distance.ToString();
    }

    
}
