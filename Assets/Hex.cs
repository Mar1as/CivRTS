using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public GameObject hexPrefab;  // Prefab hexagonu (3D model)
    public int gridWidth = 10;    // Šíøka møížky (poèet hexagonù)
    public int gridHeight = 10;   // Výška møížky (poèet hexagonù)
    public float hexRadius = 1f;  // Polomìr hexagonu

    void Start()
    {
        GenerateHexGrid();
    }

    void GenerateHexGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Výpoèet pozice hexagonu
                Vector3 position = CalculateHexPosition(x, y);

                // Vytvoøení hexagonu na dané pozici
                GameObject hex = Instantiate(hexPrefab, position, hexPrefab.transform.rotation);

                // Pro lepší pøehlednost mùžeme hexagony seskupit
                hex.transform.parent = this.transform;
                hex.name = "Hex " + x + " " + y;
            }
        }
    }

    // Výpoèet pozice hexagonu v møížce
    Vector3 CalculateHexPosition(int x, int y)
    {
        // Výpoèet šíøky a výšky hexagonu na základì polomìru
        float width = Mathf.Sqrt(3) * hexRadius;
        float height = 2 * hexRadius;

        // Posunutí každého druhého øádku o pùl hexagonu v osy X (s plochým vrcholem)
        float xPos = x * width;
        if (y % 2 == 1)
        {
            xPos += width / 2f;
        }

        // Výpoèet vertikální pozice (osy Z, protože jsme v 3D)
        float zPos = y * (height * 0.75f);

        // Vrácení výsledné pozice v 3D prostoru (osa X, Y, Z)
        return new Vector3(xPos, 0, zPos);
    }
}
