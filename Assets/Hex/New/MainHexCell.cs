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

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        if (dataHexCell != null)
        {
            Debug.Log("go");
        }
        else
        {
            Debug.Log("no");
        }
    }
    private void Start()
    {
        origColor = hexGraphics.color;
    }
    private void Update()
    {
        //UpdateHexGraphics();
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

    public void KKKT(float delay)
    {
        StartCoroutine(UpdateHexGraphics(delay));
    }

    public IEnumerator UpdateHexGraphics(float delay)
    {
        yield return null;
        UpdateHexGraphicsPicovina();
    }


    public void UpdateHexGraphicsPicovina()
    {
        if(dataHexCell.city != null)
        {
            Debug.Log("Has city");
            Debug.Log("COLOR: " + dataHexCell.city.dataCity.playerOwner.faction.factionColor.ToString());

            Color newColor = dataHexCell.city.dataCity.playerOwner.faction.factionColor;

            newColor.a = origColor.a;

            hexGraphics.color = newColor;
        }
        else
        {
            hexGraphics.color = origColor;
        }
        /*
        // Inicializace origColor, pokud je null
        if (origColor == null)
        {
            origColor = hexGraphics.color;
        }

        // Pokud je pøedána barva (factionColor není null)
        if (factionColor.HasValue)
        {
            Debug.Log("Updating graphics with faction color.");

            // Zachování pùvodní prùhlednosti (alpha) origColor
            Color newColor = new Color(factionColor.Value.r, factionColor.Value.g, factionColor.Value.b, origColor.a);
            hexGraphics.color = newColor;
        }
        else
        {
            // Pokud není pøedána barva, použijeme pùvodní barvu
            Debug.Log("No faction color provided. Resetting to original color.");
            hexGraphics.color = origColor;
        }*/
    }
}
