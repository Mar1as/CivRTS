using System;
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

    [SerializeField] SpriteRenderer hexGraphics;
    Color origColor;

    public void UpdateHexGraphics()
    {
        Debug.Log("debil");

        if (origColor == null)
        {
            Debug.Log("Get orig");
            origColor = hexGraphics.color;
        }

        if (dataHexCell.city != null)
        {
            Debug.Log("ChangeHexGraphics");
            Debug.Log("LOL " + dataHexCell.city);
            Debug.Log("LOL " + dataHexCell.city.dataCity);
            Debug.Log("LOL " + dataHexCell.city.dataCity.playerOwner);
            Debug.Log("LOL " + dataHexCell.city.dataCity.playerOwner.faction.factionName);

            Color color = dataHexCell.city.dataCity.playerOwner.faction.factionColor;
            Debug.Log("Coloros " + color);
            hexGraphics.color = color;
        }
        else
        {
            Debug.Log("Change to orig");
            hexGraphics.color = origColor;
        }
    }
}
