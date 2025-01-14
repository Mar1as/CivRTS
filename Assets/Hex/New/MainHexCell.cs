using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class MainHexCell : MonoBehaviour
{
    [SerializeField]
    public DataHexCell dataHexCell;
    public BrainHexCell brainHexCell;

    [SerializeField]
    public BarText barText;

    private void Start()
    {
        
    }
    public void Inicilizace()
    {
        dataHexCell = new DataHexCell(this);
        brainHexCell = new BrainHexCell(this);
    }

    public void RefreshPosition()
    {
        Debug.Log("Refresh");
        Vector3 position = transform.localPosition;
        position.y = dataHexCell.Elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

        transform.localPosition = position;

        Vector3 uiPosition = dataHexCell.uiRect.localPosition;
        uiPosition.z = -position.y;
        dataHexCell.uiRect.localPosition = uiPosition;
    }

    public void UpdateBarText()
    {
        if (barText != null)
        {
            if (dataHexCell.featuresHexCell.SpecialIndex > 0)
            {
                var stats = dataHexCell.city.dataCity.Stats;

                barText.ChangeText(
                    name: dataHexCell.city.dataCity.Name,
                    level: $"{stats.level}",
                    population: $"{stats.CalculatePopulation()}",
                    production: $"{stats.CalculateProduction()}",
                    food: $"{stats.CalculateFood()}",
                    levelProgress: (float)stats.levelProgress / stats.levelUpRequirement
                );
            }
        }
    }
}
