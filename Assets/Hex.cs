using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public GameObject hexPrefab;  // Prefab hexagonu (3D model)
    public int gridWidth = 10;    // ���ka m��ky (po�et hexagon�)
    public int gridHeight = 10;   // V��ka m��ky (po�et hexagon�)
    public float hexRadius = 1f;  // Polom�r hexagonu

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
                // V�po�et pozice hexagonu
                Vector3 position = CalculateHexPosition(x, y);

                // Vytvo�en� hexagonu na dan� pozici
                GameObject hex = Instantiate(hexPrefab, position, hexPrefab.transform.rotation);

                // Pro lep�� p�ehlednost m��eme hexagony seskupit
                hex.transform.parent = this.transform;
                hex.name = "Hex " + x + " " + y;
            }
        }
    }

    // V�po�et pozice hexagonu v m��ce
    Vector3 CalculateHexPosition(int x, int y)
    {
        // V�po�et ���ky a v��ky hexagonu na z�klad� polom�ru
        float width = Mathf.Sqrt(3) * hexRadius;
        float height = 2 * hexRadius;

        // Posunut� ka�d�ho druh�ho ��dku o p�l hexagonu v osy X (s ploch�m vrcholem)
        float xPos = x * width;
        if (y % 2 == 1)
        {
            xPos += width / 2f;
        }

        // V�po�et vertik�ln� pozice (osy Z, proto�e jsme v 3D)
        float zPos = y * (height * 0.75f);

        // Vr�cen� v�sledn� pozice v 3D prostoru (osa X, Y, Z)
        return new Vector3(xPos, 0, zPos);
    }
}
